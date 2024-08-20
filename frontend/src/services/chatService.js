import { axiosInstance } from "./axiosInterceptor";
import config from "../config.json";

const API_URL = config.API_URL;

const chatService = {
  getUserChats: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/chat/user-chats`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
  leaveChat: async (chatId, handleError) => {
    try {
      await axiosInstance.post(
        `${API_URL}/online-chat/chat/leave/${chatId}`,
        {}
      );
    } catch (error) {
      handleError(error);
    }
  },
  markMessagesAsRead: async (chatRoomId, handleError) => {
    try {
      await axiosInstance.post(
        `${API_URL}/online-chat/chat/read-messages/${chatRoomId}`
      );
    } catch (error) {
      handleError(error);
    }
  },
  createGroupChat: async (chatName, userIds, handleError) => {
    try {
      const response = await axiosInstance.post(
        `${API_URL}/online-chat/chat/create-group-chat`,
        {
          name: chatName,
          userIds: userIds,
        }
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
  sendMessage: async (chatId, messageContent, handleError) => {
    try {
      const response = await axiosInstance.post(
        `${API_URL}/online-chat/chat/send-message`,
        {
          ChatId: chatId,
          Content: messageContent,
        }
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
  getChatByUser: async (userId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/chat/by-user/${userId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
  getMessagesByChatId: async (chatId, amountOfMessage, count, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/chat/messages`,
        {
          params: {
            chatId: chatId,
            amountOfMessage: amountOfMessage,
            count: count,
          },
        }
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
};

export default chatService;
