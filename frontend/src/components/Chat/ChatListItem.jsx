import React from "react";
import { Box, useColorModeValue } from "@chakra-ui/react";
import ChatListItemHeader from "./ChatListItemHeader";
import ChatListItemLastMessage from "./ChatListItemLastMessage";
import ChatListItemLeaveButton from "./ChatListItemLeaveButton";

const ChatListItem = ({
  chat,
  openChat,
  leaveChat,
  formatTimestamp,
  userId,
}) => {
  const cardHoverBg = useColorModeValue("gray.100", "gray.600");

  return (
    <Box
      bg="white"
      rounded="md"
      shadow="sm"
      p={4}
      _hover={{ bg: cardHoverBg }}
      cursor="pointer"
      onClick={() => openChat(chat)}
    >
      <ChatListItemHeader chat={chat} />
      <ChatListItemLastMessage
        lastMessage={chat.lastMessage}
        formatTimestamp={formatTimestamp}
        userId={userId}
      />
      <ChatListItemLeaveButton
        isGroupChat={chat.isGroupChat}
        leaveChat={leaveChat}
        chatId={chat.id}
      />
    </Box>
  );
};

export default ChatListItem;
