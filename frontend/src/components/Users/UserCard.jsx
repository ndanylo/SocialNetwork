import React from "react";
import {
  Box,
  Avatar,
  Text,
  IconButton,
  useBreakpointValue,
} from "@chakra-ui/react";
import { ChatIcon } from "@chakra-ui/icons";
import { convertBytesToBase64Image } from "../../utils/imageUtils";

const UserCard = ({ user, onProfileClick, openChat }) => {
  const isSmallScreen = useBreakpointValue({ base: true, md: false });

  return (
    <Box
      p={6}
      bg="white"
      shadow="lg"
      borderRadius="lg"
      textAlign="center"
      position="relative"
      cursor="pointer"
      onClick={() => onProfileClick(user.id)}
      transition="transform 0.2s"
      _hover={{ transform: "scale(1.05)", shadow: "xl" }}
    >
      {!isSmallScreen && (
        <IconButton
          aria-label="Send Message"
          icon={<ChatIcon />}
          colorScheme="teal"
          position="absolute"
          top={2}
          left={2}
          onClick={(e) => {
            e.stopPropagation();
            openChat(user);
          }}
        />
      )}
      <Avatar
        src={convertBytesToBase64Image(user.avatar)}
        alt={user.firstName}
        borderRadius="full"
        boxSize="100px"
        objectFit="cover"
        mb={4}
      />
      <Text
        fontWeight="bold"
        fontSize="xl"
        mb={2}
      >{`${user.firstName} ${user.lastName}`}</Text>
      <Text fontSize="sm" color="gray.500" mb={4}>
        City: {user.city}
      </Text>
      <Text fontSize="sm" color="gray.500" mb={4}>
        Email: {user.email}
      </Text>
    </Box>
  );
};

export default UserCard;
