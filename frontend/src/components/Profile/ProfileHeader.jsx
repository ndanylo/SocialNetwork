import React from "react";
import { Heading, Text, Button, Icon, HStack, VStack } from "@chakra-ui/react";
import {
  AtSignIcon,
  EmailIcon,
  EditIcon,
  AddIcon,
  ViewIcon,
  ChatIcon,
} from "@chakra-ui/icons";
import { Link as RouterLink } from "react-router-dom";

const ProfileHeader = ({ user, isUserProfile, handleAddFriend, openChat }) => {
  return (
    <VStack spacing={4} align="start" flex={{ base: "none", md: "1" }}>
      <Heading mb="8" color="teal.500">
        Profile
      </Heading>
      <Text fontWeight="bold" fontSize="2xl" color="gray.700">
        {user.firstName} {user.lastName}
      </Text>
      <HStack spacing={4} align="center">
        <Icon as={AtSignIcon} color="teal.500" />
        <Text fontSize="lg" color="gray.700">
          {user.userName}
        </Text>
      </HStack>
      <HStack spacing={4} align="center">
        <Icon as={EmailIcon} color="teal.500" />
        <Text fontSize="lg" color="gray.700">
          {user.email}
        </Text>
      </HStack>
      {isUserProfile ? (
        <Button mt="4" colorScheme="teal" leftIcon={<EditIcon />}>
          Edit Profile
        </Button>
      ) : (
        <Button
          mt="4"
          colorScheme="green"
          onClick={handleAddFriend}
          leftIcon={<AddIcon />}
        >
          Add to Friends
        </Button>
      )}
      <HStack>
        {!isUserProfile && (
          <Button
            mt="4"
            colorScheme="blue"
            onClick={(e) => {
              e.stopPropagation();
              openChat(user);
            }}
            leftIcon={<ChatIcon />}
          >
            Join Chat
          </Button>
        )}
        <Button
          mt="4"
          colorScheme="blue"
          as={RouterLink}
          to={`/friends/${user.id}`}
          leftIcon={<ViewIcon />}
        >
          Show Friends({user.friendsCount})
        </Button>
      </HStack>
    </VStack>
  );
};

export default ProfileHeader;
