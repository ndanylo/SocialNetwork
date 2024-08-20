// useTokenState.js
import { useState, useEffect } from "react";
import { jwtDecode } from "jwt-decode";

const TOKEN_KEY = "token";

const useTokenState = () => {
  const [token, setToken] = useState(() => localStorage.getItem(TOKEN_KEY));
  const [userId, setUserId] = useState(null);

  useEffect(() => {
    if (token) {
      const decodedToken = jwtDecode(token);
      setUserId(decodedToken.nameid);
    } else {
      setUserId(null);
    }
  }, [token]);

  const handleSetToken = (newToken) => {
    localStorage.setItem(TOKEN_KEY, newToken);
    setToken(newToken);
  };

  const isTokenInStorage = () => {
    return localStorage.getItem(TOKEN_KEY) != null;
  };

  const getToken = () => {
    if (isTokenExpired()) {
      removeToken();
      return null;
    }
    return token;
  };

  const removeToken = () => {
    setToken(null);
    localStorage.removeItem(TOKEN_KEY);
  };

  const isTokenExpired = () => {
    if (!token) {
      return true;
    }
    try {
      const decodedToken = jwtDecode(token);
      const currentTime = Math.floor(Date.now() / 1000);
      const expired = decodedToken.exp < currentTime;
      return expired;
    } catch (error) {
      return true;
    }
  };

  return {
    token,
    userId,
    handleSetToken,
    getToken,
    removeToken,
    isTokenExpired,
    isTokenInStorage,
  };
};

export default useTokenState;
