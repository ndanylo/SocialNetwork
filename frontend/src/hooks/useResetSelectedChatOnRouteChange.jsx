import { useEffect } from "react";
import { useLocation } from "react-router-dom";

const useResetSelectedChatOnRouteChange = (setChatInfo) => {
  const location = useLocation();

  useEffect(() => {
    setChatInfo((prevState) => ({ ...prevState, selectedChat: null }));
  }, [location]);
};

export default useResetSelectedChatOnRouteChange;
