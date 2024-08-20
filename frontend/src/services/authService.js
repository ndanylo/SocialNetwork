import axios from "axios";
import config from "../config.json";

const API_URL = config.API_URL;

const authService = {
  login: async (email, password) => {
    try {
      const response = await axios.post(`${API_URL}/online-chat/login`, {
        email,
        password,
      });
      return response.data;
    } catch (error) {
      throw error;
    }
  },

  register: async (formData) => {
    try {
      const response = await axios.post(
        `${API_URL}/online-chat/register`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      return response.data;
    } catch (error) {
      throw error;
    }
  },
};

export default authService;
