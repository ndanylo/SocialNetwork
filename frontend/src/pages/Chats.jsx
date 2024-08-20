import React, { useEffect } from "react";
import {
  Container,
  Heading,
  Box,
  Text,
  Flex,
  SimpleGrid,
} from "@chakra-ui/react";
import chatService from "../services/chatService";
import CreateGroupChatModal from "../components/Chat/Modal/CreateGroupChatModal";
import Chat from "../components/Chat/Chat";
import ChatListItem from "../components/Chat/ChatListItem";

const Chats = ({
  token,
  connection,
  userInfo,
  handleError,
  chatInfo,
  setChatInfo,
  baseReceiveMessageHandler,
}) => {
  useEffect(() => {
    if (connection) {
      const fetchChats = async () => {
        try {
          const fetchedChats = await chatService.getUserChats(handleError);
          if (fetchedChats) {
            setChatInfo((prevState) => ({ ...prevState, chats: fetchedChats }));
          }
        } catch (error) {
          console.error("Error fetching chats", error);
        }
      };

      connection.off("ReceiveMessage");
      connection.on("ReceiveMessage", specificReceiveMessageHandler);
      fetchChats();

      return () => {
        connection.off("ReceiveMessage");
        connection.on("ReceiveMessage", baseReceiveMessageHandler);
      };
    }
  }, [token, connection]);

  const specificReceiveMessageHandler = (message) => {
    updateChatLastMessage(message);
  };

  const updateChatLastMessage = (receivedMessage) => {
    setChatInfo((prevState) => ({
      ...prevState,
      chats: prevState.chats.map((chat) =>
        chat.id === receivedMessage.chatId
          ? {
              ...chat,
              lastMessage: receivedMessage,
              unreadMessagesCount: chat.unreadMessagesCount + 1,
            }
          : chat
      ),
    }));
  };

  const openChat = (chat) => {
    const foundChat = chatInfo.chats.find((c) => c.id === chat.id);
    if (foundChat) {
      setChatInfo((prevState) => ({
        ...prevState,
        selectedChat: foundChat,
        chats: prevState.chats.map((c) =>
          c.id === chat.id ? { ...c, unreadMessagesCount: 0 } : c
        ),
      }));
      markChatMessagesAsRead(chat.id);
    }
  };

  const closeChat = () => {
    setChatInfo((prevState) => ({
      ...prevState,
      selectedChat: null,
    }));
  };

  const handleCreateGroupChat = (newChat) => {
    setChatInfo((prevState) => ({
      ...prevState,
      chats: [...prevState.chats, newChat],
    }));
  };

  const leaveChat = async (chatId) => {
    try {
      await chatService.leaveChat(chatId, handleError);
      setChatInfo((prevState) => ({
        ...prevState,
        chats: prevState.chats.filter((chat) => chat.id !== chatId),
      }));
    } catch (error) {
      console.error("Error leaving chat", error);
    }
  };

  const markChatMessagesAsRead = async (chatRoomId) => {
    try {
      await chatService.markMessagesAsRead(chatRoomId, handleError);
    } catch (error) {
      console.error("Error marking chat messages as read:", error);
    }
  };

  const formatTimestamp = (timestamp) => {
    const date = new Date(timestamp);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
  };

  return (
    <Container maxW="container.xl" py={8}>
      <Heading as="h1" size="xl" mb={4} color="teal.500">
        Your Chats
      </Heading>
      <CreateGroupChatModal
        userId={userInfo.userId}
        onCreate={handleCreateGroupChat}
      />
      <Flex mt={8}>
        <Box
          w="35%"
          h="80vh"
          bg="white"
          p={4}
          rounded="md"
          shadow="md"
          mr={4}
          overflowY="auto"
        >
          <Heading as="h2" size="md" mb={4}>
            Chat List
          </Heading>
          <SimpleGrid columns={1} gap={4}>
            {Array.isArray(chatInfo.chats) && chatInfo.chats.length > 0 ? (
              chatInfo.chats.map((chat) => (
                <ChatListItem
                  key={chat.id}
                  chat={chat}
                  openChat={openChat}
                  leaveChat={leaveChat}
                  formatTimestamp={formatTimestamp}
                  userId={userInfo.userId}
                />
              ))
            ) : (
              <Text>No chats available</Text>
            )}
          </SimpleGrid>
        </Box>
        <Box w="65%" bg="white" p={4} rounded="md" shadow="md" overflowY="auto">
          {chatInfo.selectedChat ? (
            <Chat
              previousReceiveMessageHandler={specificReceiveMessageHandler}
              token={token}
              userInfo={userInfo}
              closeChat={closeChat}
              connection={connection}
              handleError={handleError}
              setChatInfo={setChatInfo}
              chatInfo={chatInfo}
            />
          ) : (
            <Text>Select a chat to start messaging</Text>
          )}
        </Box>
      </Flex>
    </Container>
  );
};

export default Chats;
