import React, { useEffect, useState } from "react";
import { Box, Container } from "@chakra-ui/react";
import { useParams } from "react-router-dom";
import FriendList from "../components/Friends/FriendList";
import ReceivedFriendRequests from "../components/Friends/ReceivedFriendRequests";
import SentFriendRequests from "../components/Friends/SentFriendRequests";
import userService from "../services/userService";

const Friends = ({
  token,
  userInfo,
  connection,
  setReceivedFriendRequests,
  handleError,
}) => {
  const { profileUserId } = useParams();
  const [friendState, setFriendState] = useState({
    friends: [],
    sentFriendRequests: [],
  });

  useEffect(() => {
    const fetchFriendsAndRequests = async () => {
      try {
        await fetchFriends(profileUserId);
        if (userInfo.userId === profileUserId) {
          await fetchSentFriendRequests();
        }
      } catch (error) {
        console.error("Error fetching friends and friend requests", error);
      }
    };

    fetchFriendsAndRequests();
  }, [token, userInfo.userId, profileUserId]);

  useEffect(() => {
    if (connection) {
      connection.on("FriendRemoved", (friendId) => {
        setFriendState((prevState) => ({
          ...prevState,
          friends: prevState.friends.filter((friend) => friend.id !== friendId),
        }));
      });

      connection.on("FriendRequestAccepted", (acceptedUserId) => {
        setFriendState((prevState) => {
          const acceptedFriend = prevState.sentFriendRequests.find(
            (request) => request.id === acceptedUserId
          );
          if (acceptedFriend) {
            return {
              ...prevState,
              friends: [...prevState.friends, acceptedFriend],
              sentFriendRequests: prevState.sentFriendRequests.filter(
                (request) => request.id !== acceptedUserId
              ),
            };
          }
          return prevState;
        });
      });

      connection.on("FriendRequestDeclined", (userId) => {
        setFriendState((prevState) => ({
          ...prevState,
          sentFriendRequests: prevState.sentFriendRequests.filter(
            (request) => request.id !== userId
          ),
        }));
      });

      return () => {
        connection.off("FriendRemoved");
        connection.off("FriendRequestAccepted");
        connection.off("FriendRequestDeclined");
      };
    }
  }, [connection, token, profileUserId]);

  const fetchFriends = async (profileUserId) => {
    const fetchedFriends = await userService.getFriends(
      profileUserId,
      handleError
    );
    if (fetchedFriends) {
      setFriendState((prevState) => ({
        ...prevState,
        friends: fetchedFriends,
      }));
    }
  };

  const fetchSentFriendRequests = async () => {
    const fetchedSentRequests = await userService.getSentFriendRequests(
      handleError
    );
    if (fetchedSentRequests) {
      setFriendState((prevState) => ({
        ...prevState,
        sentFriendRequests: fetchedSentRequests,
      }));
    }
  };

  const handleAcceptFriendRequest = async (userId) => {
    try {
      await userService.acceptFriendRequest(userId, handleError);
      const acceptedFriend = userInfo.receivedFriendRequests.find(
        (user) => user.id === userId
      );
      if (acceptedFriend) {
        setReceivedFriendRequests(
          userInfo.receivedFriendRequests.filter((user) => user.id !== userId)
        );
        setFriendState((prevState) => ({
          ...prevState,
          friends: [...prevState.friends, acceptedFriend],
          sentFriendRequests: prevState.sentFriendRequests.filter(
            (user) => user.id !== userId
          ),
        }));
      }
    } catch (error) {
      console.error("Error accepting friend request", error);
    }
  };

  const handleDeleteFriend = async (friendId) => {
    try {
      await userService.deleteFriend(friendId, handleError);
      setFriendState((prevState) => ({
        ...prevState,
        friends: prevState.friends.filter((friend) => friend.id !== friendId),
      }));
    } catch (error) {
      console.error("Error deleting friend", error);
    }
  };

  const handleCancelFriendRequest = async (userId) => {
    try {
      await userService.cancelFriendRequest(userId, handleError);
      setFriendState((prevState) => ({
        ...prevState,
        sentFriendRequests: prevState.sentFriendRequests.filter(
          (user) => user.id !== userId
        ),
      }));
    } catch (error) {
      console.error("Error cancelling friend request", error);
    }
  };

  const handleDeclineFriendRequest = async (userId) => {
    try {
      await userService.declineFriendRequest(userId, handleError);
      setReceivedFriendRequests(
        userInfo.receivedFriendRequests.filter((user) => user.id !== userId)
      );
    } catch (error) {
      console.error("Error declining friend request", error);
    }
  };

  return (
    <Container maxW="container.xl" py={8}>
      <Box p={5}>
        <FriendList
          friends={friendState.friends}
          userId={userInfo.userId}
          profileUserId={profileUserId}
          handleDeleteFriend={handleDeleteFriend}
        />
        {userInfo.userId === profileUserId && (
          <Box mt={8}>
            <ReceivedFriendRequests
              receivedFriendRequests={userInfo.receivedFriendRequests}
              handleAcceptFriendRequest={handleAcceptFriendRequest}
              handleDeclineFriendRequest={handleDeclineFriendRequest}
            />
            <SentFriendRequests
              sentFriendRequests={friendState.sentFriendRequests}
              handleCancelFriendRequest={handleCancelFriendRequest}
            />
          </Box>
        )}
      </Box>
    </Container>
  );
};

export default Friends;
