import React from "react";
import { Flex, Heading, Icon, Badge } from "@chakra-ui/react";
import { FaUsers } from "react-icons/fa";

const ChatListItemHeader = ({ chat }) => {
  return (
    <Flex justify="space-between" align="center">
      <Flex align="center">
        <Icon as={FaUsers} mr={2} />
        <Heading as="h3" size="sm" fontWeight="bold">
          {chat.name}
        </Heading>
      </Flex>
      {chat.unreadMessagesCount > 0 && (
        <Badge colorScheme="red" ml={2}>
          {chat.unreadMessagesCount} unread
        </Badge>
      )}
    </Flex>
  );
};

export default ChatListItemHeader;
