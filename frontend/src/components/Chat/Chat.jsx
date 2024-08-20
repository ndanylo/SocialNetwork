import React, { useState, useEffect, useRef, useLayoutEffect } from "react";
import {
  Button,
  Input,
  Heading,
  CloseButton,
  Box,
  Text,
  Flex,
  useDisclosure,
  useColorModeValue,
} from "@chakra-ui/react";
import Message from "./Message";
import ReadByModal from "./Modal/ReadByModal";
import { useToast } from "@chakra-ui/react";
import chatService from "../../services/chatService";

const Chat = ({
  chatInfo,
  setChatInfo,
  closeChat,
  userInfo,
  connection,
  handleError,
  previousReceiveMessageHandler,
}) => {
  const initialChatState = {
    chatId: null,
    messages: [],
    message: "",
    readByList: [],
    hasMoreMessages: true,
    loadingMoreMessages: false,
    lastMessageId: null,
  };

  const [chatState, setChatState] = useState(initialChatState);
  const { isOpen, onOpen, onClose } = useDisclosure();
  const messagesEndRef = useRef(null);
  const containerRef = useRef(null);
  const toast = useToast();
  const bg = useColorModeValue("gray.50", "gray.800");
  const [initialLoadComplete, setInitialLoadComplete] = useState(false);

  const fetchMessages = async (chatId, count = 8) => {
    try {
      setChatState((prevState) => ({
        ...prevState,
        loadingMoreMessages: true,
      }));
      const fetchedMessages = await chatService.getMessagesByChatId(
        chatId,
        chatState.messages.length,
        count,
        handleError
      );
      setChatState((prevState) => {
        const updatedMessages = [...fetchedMessages, ...prevState.messages];
        return {
          ...prevState,
          messages: updatedMessages,
          hasMoreMessages: fetchedMessages.length === count,
          loadingMoreMessages: false,
          lastMessageId:
            fetchedMessages.length > 0
              ? fetchedMessages[fetchedMessages.length - 1].id
              : prevState.lastMessageId,
        };
      });

      if (fetchedMessages.length > 0) {
        setTimeout(() => {
          const messageId = `message-${
            fetchedMessages[fetchedMessages.length - 1].id
          }`;
          const messageElement =
            document.getElementById(messageId) ||
            document.querySelector('[id^="message-"]');

          if (messageElement) {
            messageElement.scrollIntoView({
              behavior: "smooth",
              block: "start",
            });
          }
        }, 500);
      }
    } catch (error) {
      console.error("Error fetching chat messages:", error);
      setChatState((prevState) => ({
        ...prevState,
        loadingMoreMessages: false,
      }));
    }
  };

  const markChatMessagesAsRead = async (chatRoomId) => {
    try {
      await chatService.markMessagesAsRead(chatRoomId, handleError);
    } catch (error) {
      console.error("Error marking chat messages as read:", error);
    }
  };

  const specificReceiveMessageHandler = async (message) => {
    setChatState((prevState) => ({
      ...prevState,
      messages: [...prevState.messages, message],
    }));

    if (message.chatId === chatState.chatId) {
      try {
        await chatService.markMessagesAsRead(chatState.chatId);
      } catch (error) {
        console.error("Ошибка при пометке сообщений как прочитанных:", error);
      }
    }

    scrollToBottom();
  };

  const handleSetMessageRead = (chatId, messageId, user) => {
    setChatState((prevState) => ({
      ...prevState,
      messages: prevState.messages.map((message) =>
        message.id === messageId
          ? { ...message, readBy: [...message.readBy, user] }
          : message
      ),
    }));

    setChatInfo((prevChatInfo) => ({
      ...prevChatInfo,
      chats: prevChatInfo.chats.map((chat) =>
        chat.id === chatId && chat.lastMessage.id === messageId
          ? {
              ...chat,
              lastMessage: {
                ...chat.lastMessage,
                readBy: [...chat.lastMessage.readBy, user],
              },
            }
          : chat
      ),
    }));
  };

  useEffect(() => {
    if (
      chatInfo.selectedChat &&
      chatInfo.selectedChat.id !== chatState.chatId
    ) {
      setChatState({
        ...initialChatState,
        chatId: chatInfo.selectedChat.id,
      });
      setInitialLoadComplete(false);
    }
  }, [chatInfo.selectedChat]);

  useEffect(() => {
    const initializeChat = async () => {
      if (
        chatInfo.selectedChat &&
        chatState.chatId === chatInfo.selectedChat.id
      ) {
        setInitialLoadComplete(true);
        await fetchMessages(chatState.chatId);
        await markChatMessagesAsRead(chatState.chatId);
      }
    };

    initializeChat();
  }, [chatState.chatId]);

  useEffect(() => {
    if (connection) {
      connection.off("ReceiveMessage");
      connection.on("ReceiveMessage", specificReceiveMessageHandler);
      connection.on("SetMessageRead", handleSetMessageRead);

      return () => {
        connection.off("ReceiveMessage", specificReceiveMessageHandler);
        connection.off("SetMessageRead", handleSetMessageRead);
        connection.on("ReceiveMessage", previousReceiveMessageHandler);
      };
    }
  }, [
    connection,
    specificReceiveMessageHandler,
    handleSetMessageRead,
    previousReceiveMessageHandler,
  ]);

  const handleScroll = () => {
    const container = containerRef.current;
    if (container) {
      if (
        container.scrollTop === 0 &&
        chatState.hasMoreMessages &&
        !chatState.loadingMoreMessages &&
        initialLoadComplete
      ) {
        fetchMessages(chatState.chatId);
      }
    }
  };

  useEffect(() => {
    const container = containerRef.current;
    if (container) {
      container.addEventListener("scroll", handleScroll);
      return () => container.removeEventListener("scroll", handleScroll);
    }
  }, [
    chatState.hasMoreMessages,
    chatState.loadingMoreMessages,
    chatState.lastMessageId,
  ]);

  const onSendMessage = async () => {
    try {
      if (!chatState.message.trim()) {
        toast({
          title: "Error",
          description: "Message cannot be empty",
          status: "error",
          duration: 3000,
          isClosable: true,
        });
        return;
      }
      if (chatInfo.selectedChat) {
        const newMessage = await chatService.sendMessage(
          chatState.chatId,
          chatState.message,
          handleError
        );
        if (newMessage) {
          newMessage.readBy = [];
          setChatState((prevState) => ({
            ...prevState,
            messages: [...prevState.messages, newMessage],
            message: "",
          }));
          if (setChatInfo) {
            setChatInfo((prevChatInfo) => ({
              ...prevChatInfo,
              chats: prevChatInfo.chats.map((c) =>
                c.id === chatState.chatId
                  ? { ...c, lastMessage: newMessage }
                  : c
              ),
            }));
          }
          scrollToBottom();
        }
      }
    } catch (error) {
      console.error("Error sending message", error);
    }
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  const formatTimestamp = (timestamp) => {
    const date = new Date(timestamp);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
  };

  const handleShowReadBy = (readBy) => {
    setChatState((prevState) => ({
      ...prevState,
      readByList: readBy,
    }));
    onOpen();
  };

  return (
    <Box w="full" bg={bg} p={8} rounded="md" shadow="lg">
      <Flex justify="space-between" align="center" mb={5}>
        <Heading size="lg" color="teal.500">
          {chatInfo.selectedChat && chatInfo.selectedChat.users
            ? chatInfo.selectedChat.users
                .map((user) => user.userName)
                .join(", ")
            : "Loading..."}
        </Heading>
        <CloseButton onClick={closeChat} />
      </Flex>
      <Box
        className="flex flex-col overflow-auto"
        h="60vh"
        p={3}
        mb={4}
        ref={containerRef}
      >
        {chatState.messages.length > 0 ? (
          chatState.messages.map((messageInfo, index) => (
            <Message
              key={index}
              messageInfo={messageInfo}
              userId={userInfo.userId}
              handleShowReadBy={handleShowReadBy}
              formatTimestamp={formatTimestamp}
            />
          ))
        ) : (
          <Text>No messages yet.</Text>
        )}
        <div ref={messagesEndRef} />
      </Box>
      <Flex>
        <Input
          type="text"
          value={chatState.message}
          onChange={(e) =>
            setChatState((prevState) => ({
              ...prevState,
              message: e.target.value,
            }))
          }
          placeholder="Enter message"
          flex="1"
          mr={2}
        />
        <Button onClick={onSendMessage} colorScheme="teal">
          Send
        </Button>
      </Flex>
      <ReadByModal
        isOpen={isOpen}
        onClose={onClose}
        readBy={chatState.readByList}
      />
    </Box>
  );
};

export default Chat;
