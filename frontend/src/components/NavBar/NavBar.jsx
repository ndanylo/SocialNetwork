import React from "react";
import { Link } from "react-router-dom";
import {
  Box,
  Flex,
  Button,
  Heading,
  Badge,
  IconButton,
} from "@chakra-ui/react";
import Notifications from "../Notification/Notifications";

const NavBar = ({
  chatInfo,
  token,
  connection,
  getToken,
  onLogout,
  removeToken,
  userInfo,
  isTokenExpired,
  handleError,
}) => {
  return (
    <Flex
      as="nav"
      bg="teal.500"
      color="white"
      padding="1.5rem"
      align="center"
      boxShadow="md"
    >
      <Heading size="lg" color="white">
        Social Network
      </Heading>
      <Box ml="auto" display="flex" alignItems="center">
        <Button
          as={Link}
          to="/home"
          variant="ghost"
          color="white"
          _hover={{ bg: "teal.600" }}
        >
          Home
        </Button>
        <Button
          as={Link}
          to={`/profile/${userInfo.userId}`}
          variant="ghost"
          color="white"
          _hover={{ bg: "teal.600" }}
        >
          Profile
        </Button>
        <Button
          as={Link}
          to="/users"
          variant="ghost"
          color="white"
          _hover={{ bg: "teal.600" }}
        >
          Users
        </Button>
        {token && (
          <Button
            as={Link}
            to="/chats"
            variant="ghost"
            color="white"
            _hover={{ bg: "teal.600" }}
          >
            Chats{" "}
            {chatInfo.unreadMessagesCount > 0 && (
              <Badge colorScheme="red" ml="1">
                {chatInfo.unreadMessagesCount}
              </Badge>
            )}
          </Button>
        )}
        {token && (
          <Button
            as={Link}
            to={`/friends/${userInfo.userId}`}
            variant="ghost"
            color="white"
            _hover={{ bg: "teal.600" }}
          >
            Friends{" "}
            {userInfo.friendRequestsCount > 0 && (
              <Badge colorScheme="red" ml="1">
                {userInfo.friendRequestsCount}
              </Badge>
            )}
          </Button>
        )}
        {token && (
          <Notifications
            token={token}
            connection={connection}
            isTokenExpired={isTokenExpired}
            removeToken={removeToken}
            handleError={handleError}
            getToken={getToken}
          />
        )}
        <Button
          variant="ghost"
          color="white"
          _hover={{ bg: "teal.600" }}
          onClick={onLogout}
        >
          Logout
        </Button>
      </Box>
    </Flex>
  );
};

export default NavBar;
