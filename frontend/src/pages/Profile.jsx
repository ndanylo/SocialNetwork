import React, { useEffect, useState } from "react";
import { Container, Box, Text, useToast } from "@chakra-ui/react";
import { useParams } from "react-router-dom";
import Chat from "../components/Chat/Chat";
import userService from "../services/userService";
import useResetSelectedChatOnRouteChange from "../hooks/useResetSelectedChatOnRouteChange";
import ProfileHeader from "../components/Profile/ProfileHeader";
import AvatarSection from "../components/Profile/AvatarSection";
import CreatePostForm from "../components/Profile/CreatePostForm";
import PostListSection from "../components/Profile/PostListSection";

const Profile = ({
  token,
  userInfo,
  chatInfo,
  openChat,
  closeChat,
  connection,
  handleError,
  setChatInfo,
}) => {
  const toast = useToast();
  const [user, setUser] = useState(null);
  const { selectedUserId } = useParams();

  useResetSelectedChatOnRouteChange(setChatInfo);

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const userData = await userService.fetchUserProfile(
          selectedUserId,
          handleError
        );
        if (userData) {
          setUser(userData);
        }
      } catch (error) {
        console.error("Error fetching user profile", error);
      }
    };

    fetchUser();
  }, [token, selectedUserId, handleError]);

  const handleAddFriend = async () => {
    try {
      await userService.handleAddFriend(selectedUserId, handleError);
    } catch (error) {
      toast({
        title: "Error adding friend",
        description: error.response?.data || error.message,
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  if (!user) {
    return <Text>Loading...</Text>;
  }

  const isUserProfile = userInfo?.userId === user.id;

  if (chatInfo?.selectedChat) {
    return (
      <Chat
        chat={chatInfo.selectedChat}
        token={token}
        chatInfo={chatInfo}
        connection={connection}
        closeChat={closeChat}
        userId={userInfo.userId}
        handleError={handleError}
      />
    );
  }

  return (
    <Container maxW="container.xl" py={8}>
      <Box className="min-h-screen p-8" bg="gray.100">
        <Box
          flex="1"
          p="6"
          bg="white"
          shadow="md"
          borderRadius="md"
          mb={{ base: "4", md: "0" }}
          display="flex"
          flexDirection={{ base: "column", md: "row" }}
        >
          <ProfileHeader
            user={user}
            isUserProfile={isUserProfile}
            handleAddFriend={handleAddFriend}
            openChat={openChat}
          />
          <AvatarSection user={user} />
        </Box>
        {isUserProfile && (
          <CreatePostForm
            userId={selectedUserId}
            handleError={handleError}
            setUser={setUser}
          />
        )}
        <PostListSection
          user={user}
          userId={userInfo.userId}
          handleError={handleError}
          setUser={setUser}
        />
      </Box>
    </Container>
  );
};

export default Profile;
