import React, { useState, useEffect } from "react";
import {
  Box,
  IconButton,
  Menu,
  MenuButton,
  MenuList,
  MenuItem,
  Badge,
  Button,
  Flex,
  Text,
} from "@chakra-ui/react";
import { BellIcon, ArrowLeftIcon, ArrowRightIcon } from "@chakra-ui/icons";
import { useNavigate } from "react-router-dom";
import notificationService from "../../services/notificationService";

const ITEMS_PER_PAGE = 10;

const Notifications = ({ getToken, token, connection, handleError }) => {
  const [notificationState, setNotificationState] = useState({
    notifications: [],
    unreadCount: 0,
    currentPage: 1,
    totalPages: 1,
  });

  const navigate = useNavigate();

  useEffect(() => {
    const fetchNotifications = async () => {
      try {
        if (getToken()) {
          const notes = await notificationService.getAllNotifications(
            handleError
          );
          if (notes) {
            setNotificationState((prevState) => ({
              ...prevState,
              notifications: notes,
              unreadCount: notes.length,
              totalPages: Math.ceil(notes.length / ITEMS_PER_PAGE),
            }));
          }
        }
      } catch (error) {
        console.error("Error fetching notifications", error);
      }
    };

    fetchNotifications();

    if (connection) {
      const handleReceiveNotification = (notification) => {
        setNotificationState((prevState) => ({
          ...prevState,
          notifications: [notification, ...prevState.notifications],
          unreadCount: prevState.unreadCount + 1,
          totalPages: Math.ceil(
            (prevState.notifications.length + 1) / ITEMS_PER_PAGE
          ),
        }));
      };

      connection.on("ReceiveNotification", handleReceiveNotification);

      return () => {
        if (connection) {
          connection.off("ReceiveNotification", handleReceiveNotification);
        }
      };
    }
  }, [token, connection]);

  const markAsRead = async (notificationId) => {
    try {
      await notificationService.deleteNotification(notificationId, handleError);
      const updatedNotifications = notificationState.notifications.filter(
        (n) => n.id !== notificationId
      );
      setNotificationState((prevState) => ({
        ...prevState,
        notifications: updatedNotifications,
        unreadCount: updatedNotifications.length,
        totalPages: Math.ceil(updatedNotifications.length / ITEMS_PER_PAGE),
      }));
    } catch (error) {
      console.error("Error marking notification as read", error);
    }
  };

  const markAllAsRead = async () => {
    try {
      await notificationService.deleteAllNotifications(handleError);
      setNotificationState({
        notifications: [],
        unreadCount: 0,
        currentPage: 1,
        totalPages: 1,
      });
    } catch (error) {
      console.error("Error marking all notifications as read", error);
    }
  };

  const handleNotificationClick = async (notification) => {
    await markAsRead(notification.id);
    navigate(`/post/${notification.post.id}`);
  };

  const currentNotifications = notificationState.notifications.slice(
    (notificationState.currentPage - 1) * ITEMS_PER_PAGE,
    notificationState.currentPage * ITEMS_PER_PAGE
  );

  const handleNextPage = () => {
    if (notificationState.currentPage < notificationState.totalPages) {
      setNotificationState((prevState) => ({
        ...prevState,
        currentPage: prevState.currentPage + 1,
      }));
    }
  };

  const handlePrevPage = () => {
    if (notificationState.currentPage > 1) {
      setNotificationState((prevState) => ({
        ...prevState,
        currentPage: prevState.currentPage - 1,
      }));
    }
  };

  return (
    <Menu>
      <MenuButton
        as={IconButton}
        icon={
          <Box className="relative w-14">
            <BellIcon boxSize={6} color="white" />
            <Badge className="absolute top-0 right-2" colorScheme="red">
              {notificationState.unreadCount}
            </Badge>
          </Box>
        }
        variant="outline"
      ></MenuButton>
      <MenuList className="bg-white shadow-md rounded-md">
        <Box className="p-2">
          <Box
            className="text-sm"
            color="gray"
            cursor="pointer"
            onClick={markAllAsRead}
            _hover={{ color: "green.500" }}
          >
            Mark all as read
          </Box>
        </Box>
        {currentNotifications.length === 0 ? (
          <MenuItem color="gray">No notifications</MenuItem>
        ) : (
          currentNotifications.map((notification) => (
            <MenuItem
              key={notification.id}
              className="flex justify-between items-center"
            >
              <Text
                className="cursor-pointer"
                color="gray"
                onClick={() => handleNotificationClick(notification)}
                _hover={{ color: "green.500" }}
              >
                {notification.content}
              </Text>
            </MenuItem>
          ))
        )}
        <Flex className="justify-between p-2">
          {notificationState.currentPage > 1 && (
            <IconButton
              aria-label="Previous page"
              icon={<ArrowLeftIcon />}
              size="sm"
              onClick={handlePrevPage}
              disabled={notificationState.currentPage === 1}
            />
          )}
          <Text color="gray">
            Page {notificationState.currentPage} of{" "}
            {notificationState.totalPages}
          </Text>
          {notificationState.currentPage < notificationState.totalPages && (
            <IconButton
              aria-label="Next page"
              icon={<ArrowRightIcon />}
              size="sm"
              onClick={handleNextPage}
              disabled={
                notificationState.currentPage === notificationState.totalPages
              }
            />
          )}
        </Flex>
      </MenuList>
    </Menu>
  );
};

export default Notifications;
