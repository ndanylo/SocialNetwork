import React, { useEffect, useState, useCallback } from "react";
import { ChakraProvider } from "@chakra-ui/react";
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from "react-router-dom";
import Login from "./components/Auth/Login";
import Home from "./pages/Home";
import Profile from "./pages/Profile";
import Users from "./pages/Users";
import PostDetails from "./components/Post/PostDetails";
import Chats from "./pages/Chats";
import Register from "./components/Auth/Register";
import Friends from "./pages/Friends";
import NavBar from "./components/NavBar/NavBar";
import userService from "./services/userService";
import chatService from "./services/chatService";
import useTokenState from "./services/useTokenState";
import { configureAxiosInterceptors } from "./services/axiosInterceptor";
import { jwtDecode } from "jwt-decode";
import { HubConnectionBuilder } from "@microsoft/signalr";
import ErrorPage from "./pages/ErrorPage";
import useErrorBoundary from "./hooks/useErrorBoundary";
import config from "./config.json";

const SignalR_URL = config.SignalR_URL;

function App() {
  const {
    token,
    handleSetToken,
    userId,
    getToken,
    removeToken,
    isTokenExpired,
  } = useTokenState();
  const [connection, setConnection] = useState(null);
  const [userState, setUserState] = useState({
    userId: null,
    receivedFriendRequests: [],
    friendRequestsCount: 0,
  });

  const [chatState, setChatState] = useState({
    chats: [],
    selectedChat: null,
    unreadMessagesCount: 0,
  });

  const [axiosConfigured, setAxiosConfigured] = useState(false);
  const { hasError, errorMessage, removeError, handleError } =
    useErrorBoundary();

  useEffect(() => {
    setUserState((prevState) => ({ ...prevState, userId }));
  }, [userId]);

  useEffect(() => {
    if (getToken()) {
      configureAxiosInterceptors({ token })
        .then(() => setAxiosConfigured(true))
        .catch((error) =>
          console.error("Error configuring axios interceptors:", error)
        );
    }
  }, [token]);

  useEffect(() => {
    const createHubConnection = () => {
      try {
        const newConnection = new HubConnectionBuilder()
          .withUrl(`${SignalR_URL}/chat`, {
            accessTokenFactory: () => getToken(),
          })
          .withAutomaticReconnect()
          .build();

        newConnection
          .start()
          .then(() => setConnection(newConnection))
          .catch((error) =>
            console.error("Error connecting to SignalR", error)
          );
      } catch (error) {
        console.log("Error creating SignalR connection:", error);
      }
    };

    if (getToken()) {
      createHubConnection();
      const decodedToken = jwtDecode(getToken());
      setUserState((prevState) => ({
        ...prevState,
        userId: decodedToken.nameid,
      }));
    } else {
      setConnection(null);
      setUserState((prevState) => ({ ...prevState, userId: null }));
    }
  }, [token]);

  const baseReceiveMessageHandler = useCallback((message) => {
    setChatState((prevState) => ({
      ...prevState,
      chats: prevState.chats.map((chat) =>
        chat.id === message.chatId
          ? {
              ...chat,
              unreadMessagesCount: (chat.unreadMessagesCount || 0) + 1,
            }
          : chat
      ),
    }));
  }, []);

  useEffect(() => {
    if (connection) {
      connection.on("ReceiveMessage", baseReceiveMessageHandler);

      connection.on("FriendRequestReceived", (user) => {
        setUserState((prevState) => ({
          ...prevState,
          receivedFriendRequests: Array.isArray(
            prevState.receivedFriendRequests
          )
            ? [...prevState.receivedFriendRequests, user]
            : [user],
        }));
      });

      connection.on("FriendRequestCancelled", (userId) => {
        setUserState((prevState) => ({
          ...prevState,
          receivedFriendRequests: prevState.receivedFriendRequests.filter(
            (request) => request.id !== userId
          ),
        }));
      });

      const fetchChats = async () => {
        try {
          const chats = await chatService.getUserChats(handleError);
          setChatState((prevState) => ({ ...prevState, chats }));
        } catch (error) {
          console.error("Error fetching user chats:", error);
        }
      };

      const fetchFriendRequests = async () => {
        try {
          const requests = await userService.fetchReceivedFriendRequests(
            handleError
          );
          setUserState((prevState) => ({
            ...prevState,
            receivedFriendRequests: requests,
          }));
        } catch (error) {
          console.error("Error fetching friend requests:", error);
        }
      };

      if (getToken()) {
        fetchChats();
        fetchFriendRequests();
      }

      return () => {
        connection.off("ReceiveMessage", baseReceiveMessageHandler);
        connection.off("FriendRequestReceived");
        connection.off("FriendRequestCancelled");
      };
    }
  }, [connection, token]);

  useEffect(() => {
    const calculateUnreadMessages = () => {
      return Array.isArray(chatState.chats)
        ? chatState.chats.reduce(
            (total, chat) => total + (chat.unreadMessagesCount || 0),
            0
          )
        : 0;
    };
    setChatState((prevState) => ({
      ...prevState,
      unreadMessagesCount: calculateUnreadMessages(),
    }));
  }, [chatState.chats]);

  useEffect(() => {
    if (userState.receivedFriendRequests) {
      setUserState((prevState) => ({
        ...prevState,
        friendRequestsCount: prevState.receivedFriendRequests.length,
      }));
    }
  }, [userState.receivedFriendRequests]);

  const handleLogout = () => {
    removeToken();
  };

  const openChatByUser = async (user) => {
    try {
      const chat = await chatService.getChatByUser(user.id, handleError);
      setChatState((prevState) => ({
        ...prevState,
        selectedChat: chat,
      }));
    } catch (error) {
      handleError(error);
      console.error("Error fetching or creating chat", error);
    }
  };

  const closeChat = () => {
    setChatState((prevState) => ({ ...prevState, selectedChat: null }));
  };

  if (!axiosConfigured && !isTokenExpired()) {
    return <div>Loading...</div>;
  }

  if (hasError) {
    return <ErrorPage errorMessage={errorMessage} removeError={removeError} />;
  }

  return (
    <ChakraProvider>
      <Router>
        <NavBar
          chatInfo={chatState}
          userInfo={userState}
          token={token}
          isTokenExpired={isTokenExpired}
          removeToken={removeToken}
          connection={connection}
          onLogout={handleLogout}
          handleError={handleError}
          getToken={getToken}
        />
        <Routes>
          <Route
            path="/auth"
            element={
              <Login
                setToken={handleSetToken}
                token={token}
                handleError={handleError}
              />
            }
          />
          <Route
            path="/home"
            element={
              token ? (
                <Home token={token} handleError={handleError} />
              ) : (
                <Navigate to="/auth" />
              )
            }
          />
          <Route
            path="/profile/:selectedUserId"
            element={
              token ? (
                <Profile
                  token={token}
                  userInfo={userState}
                  chatInfo={chatState}
                  openChat={openChatByUser}
                  closeChat={closeChat}
                  connection={connection}
                  handleError={handleError}
                  setChatInfo={setChatState}
                />
              ) : (
                <Navigate to="/auth" />
              )
            }
          />
          <Route
            path="/users"
            element={
              token ? (
                <Users
                  token={token}
                  userInfo={userState}
                  connection={connection}
                  chatInfo={chatState}
                  setChatInfo={setChatState}
                  closeChat={closeChat}
                  openChat={openChatByUser}
                  handleError={handleError}
                />
              ) : (
                <Navigate to="/auth" />
              )
            }
          />
          <Route
            path="/chats"
            element={
              token ? (
                <Chats
                  token={token}
                  connection={connection}
                  userInfo={userState}
                  setChatInfo={setChatState}
                  chatInfo={chatState}
                  handleError={handleError}
                  baseReceiveMessageHandler={baseReceiveMessageHandler}
                />
              ) : (
                <Navigate to="/auth" />
              )
            }
          />
          <Route
            path="/friends/:profileUserId"
            element={
              token ? (
                <Friends
                  token={token}
                  connection={connection}
                  userInfo={userState}
                  setReceivedFriendRequests={(requests) =>
                    setUserState((prevState) => ({
                      ...prevState,
                      receivedFriendRequests: requests,
                    }))
                  }
                  handleError={handleError}
                />
              ) : (
                <Navigate to="/auth" />
              )
            }
          />
          <Route
            path="/post/:postId"
            element={
              token ? <PostDetails token={token} /> : <Navigate to="/auth" />
            }
          />
          <Route path="/register" element={<Register />} />
          <Route
            path="/"
            element={<Navigate to={token ? "/home" : "/auth"} />}
          />
          <Route
            path="*"
            element={
              <ErrorPage
                errorMessage="Page not found"
                removeError={removeError}
              />
            }
          />
        </Routes>
      </Router>
    </ChakraProvider>
  );
}

export default App;
