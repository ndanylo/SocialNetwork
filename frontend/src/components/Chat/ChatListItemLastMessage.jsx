import React from "react";
import { Box, Flex, Text, IconButton } from "@chakra-ui/react";
import { FaRegCommentDots } from "react-icons/fa";
import { CheckIcon } from "@chakra-ui/icons";

const ChatListItemLastMessage = ({ lastMessage, formatTimestamp, userId }) => {
  if (!lastMessage || !lastMessage.user) {
    return <Text color="gray.500">No messages yet</Text>;
  }

  const isMessageRead = lastMessage.readBy && lastMessage.readBy.length > 0;
  const isUserMessage = userId === lastMessage.user.id;

  return (
    <Box color="gray.600" mt={2}>
      <Text>{formatTimestamp(lastMessage.timestamp)}</Text>
      <Flex justify="space-between" align="center">
        <Text>
          <FaRegCommentDots style={{ display: "inline", marginRight: "8px" }} />
          {lastMessage.content}
        </Text>
        {isUserMessage && (
          <IconButton
            size="xs"
            icon={
              isMessageRead ? (
                <Box>
                  <CheckIcon color="green.500" />
                  <CheckIcon color="green.500" marginLeft="-5px" />
                </Box>
              ) : (
                <CheckIcon />
              )
            }
            aria-label="Read by status"
          />
        )}
      </Flex>
    </Box>
  );
};

export default ChatListItemLastMessage;
