import { axiosInstance } from "./axiosInterceptor";
import config from "../config.json";

const API_URL = config.API_URL;

const postService = {
  fetchPost: async (postId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/post/get-post/${postId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  createPost: async (formData, handleError) => {
    try {
      const response = await axiosInstance.post(
        `${API_URL}/online-chat/post/create`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  likePost: async (postId, handleError) => {
    try {
      const response = await axiosInstance.post(
        `${API_URL}/online-chat/like/like-post/${postId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  unlikePost: async (postId, handleError) => {
    try {
      const response = await axiosInstance.delete(
        `${API_URL}/online-chat/like/unlike-post/${postId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  getFriendsPosts: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/post/friends-post`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  fetchPostLikes: async (postId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/post/likes/${postId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },
};

export default postService;
