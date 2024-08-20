import { axiosInstance } from "./axiosInterceptor";
import config from "../config.json";

const API_URL = config.API_URL;

const commentService = {
  fetchComments: async (postId, handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/comment/get-post-comments/${postId}`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  createComment: async (postId, commentContent, handleError) => {
    try {
      const response = await axiosInstance.post(
        `${API_URL}/online-chat/comment/create`,
        {
          postId,
          content: commentContent,
        }
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  deleteComment: async (commentId, handleError) => {
    try {
      await axiosInstance.delete(
        `${API_URL}/online-chat/comment/delete/${commentId}`
      );
      return true;
    } catch (error) {
      handleError(error);
    }
  },
};

export default commentService;
