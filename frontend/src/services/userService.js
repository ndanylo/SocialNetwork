import { axiosInstance } from "./axiosInterceptor";
import config from "../config.json";

const API_URL = config.API_URL;

const userService = {
  getAllUsers: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/user/get-all-users`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  getFriends: async (profileUserId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/user/${profileUserId}/friends`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  getSentFriendRequests: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/friend-request/sent`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  acceptFriendRequest: async (userId, handleError) => {
    try {
      await axiosInstance.post(
        `${API_URL}/online-chat/friend-request/accept/${userId}`
      );
    } catch (error) {
      handleError(error);
    }
  },

  deleteFriend: async (friendId, handleError) => {
    try {
      await axiosInstance.post(
        `${API_URL}/online-chat/user/removeFriend/${friendId}`
      );
    } catch (error) {
      handleError(error);
    }
  },

  declineFriendRequest: async (senderId, handleError) => {
    try {
      await axiosInstance.delete(
        `${API_URL}/online-chat/friend-request/decline/${senderId}`
      );
    } catch (error) {
      handleError(error);
    }
  },

  cancelFriendRequest: async (receiverId, handleError) => {
    try {
      await axiosInstance.delete(
        `${API_URL}/online-chat/friend-request/cancel/${receiverId}`,
        {
          data: {
            receiverId: receiverId,
          },
        }
      );
    } catch (error) {
      handleError(error);
    }
  },

  fetchReceivedFriendRequests: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        "/online-chat/friend-request/received"
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  handleAddFriend: async (userId, handleError) => {
    try {
      await axiosInstance.post(
        `${API_URL}/online-chat/friend-request/send/${userId}`
      );
    } catch (error) {
      if (
        error.response &&
        (error.response.data === "Users already have friendship." ||
          error.response.data === "Friend request already sent.")
      ) {
        throw error;
      }
      handleError(error);
    }
  },

  fetchUserProfile: async (userId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/user/profile/${userId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
};

export default userService;
