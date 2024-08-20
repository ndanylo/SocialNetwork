import { axiosInstance } from "./axiosInterceptor";
import config from "../config.json";

const API_URL = config.API_URL;

const notificationService = {
  getAllNotifications: async (handleError) => {
    try {
      const response = await axiosInstance.get(
        `${API_URL}/online-chat/notification/get-all-notifications`
      );
      return response.data;
    } catch (error) {
      handleError(error);
    }
  },

  deleteNotification: async (notificationId, handleError) => {
    try {
      await axiosInstance.delete(
        `${API_URL}/online-chat/notification/delete/${notificationId}`
      );
    } catch (error) {
      handleError(error);
    }
  },

  deleteAllNotifications: async (handleError) => {
    try {
      await axiosInstance.delete(
        `${API_URL}/online-chat/notification/delete-all`
      );
    } catch (error) {
      handleError(error);
    }
  },
};

export default notificationService;
