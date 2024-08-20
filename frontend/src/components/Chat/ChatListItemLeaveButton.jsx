import React from "react";
import { Button, useColorModeValue } from "@chakra-ui/react";
import { FaSignOutAlt } from "react-icons/fa";

const ChatListItemLeaveButton = ({ isGroupChat, leaveChat, chatId }) => {
  const leaveButtonBg = useColorModeValue("red.500", "red.300");
  const leaveButtonHoverBg = useColorModeValue("red.600", "red.400");

  return (
    isGroupChat && (
      <Button
        mt={4}
        size="sm"
        bg={leaveButtonBg}
        color="white"
        leftIcon={<FaSignOutAlt />}
        _hover={{ bg: leaveButtonHoverBg }}
        onClick={(e) => {
          e.stopPropagation();
          leaveChat(chatId);
        }}
      >
        Leave Chat
      </Button>
    )
  );
};

export default ChatListItemLeaveButton;
