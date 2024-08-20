import React from "react";
import { Avatar, HStack, Heading, VStack, Text } from "@chakra-ui/react";
import { Link } from "react-router-dom";
import { convertBytesToBase64Image } from "../../utils/imageUtils";

const PostHeader = ({ user, createdAt }) => {
  if (!user) {
    return (
      <HStack spacing="4" align="start">
        <Avatar />
        <VStack align="start" spacing="1" flex="1">
          <Heading size="sm">Loading...</Heading>
          <Text fontSize="sm" color="gray.500">
            {new Date(createdAt).toLocaleString()}
          </Text>
        </VStack>
      </HStack>
    );
  }

  return (
    <HStack spacing="4" align="start">
      <Avatar
        name={user.userName}
        src={convertBytesToBase64Image(user.avatar)}
      />
      <VStack align="start" spacing="1" flex="1">
        <Link to={`/profile/${user.id}`} style={{ textDecoration: "none" }}>
          <Heading size="sm" _hover={{ color: "blue.500" }} cursor="pointer">
            {user.userName}
          </Heading>
        </Link>
        <Text fontSize="sm" color="gray.500">
          {new Date(createdAt).toLocaleString()}
        </Text>
      </VStack>
    </HStack>
  );
};

export default PostHeader;
