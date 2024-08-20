import React, { useEffect, useState } from "react";
import { Container, Box, Heading, Grid, GridItem } from "@chakra-ui/react";
import { useNavigate } from "react-router-dom";
import Chat from "../components/Chat/Chat";
import UserSearch from "../components/Users/UserSearch";
import UserCard from "../components/Users/UserCard";
import Pagination from "../components/Users/Pagination";
import userService from "../services/userService";
import useResetSelectedChatOnRouteChange from "../hooks/useResetSelectedChatOnRouteChange";

const Users = ({
  chatInfo,
  token,
  openChat,
  connection,
  closeChat,
  handleError,
  setChatInfo,
  userInfo,
}) => {
  const [users, setUsers] = useState([]);
  const [searchState, setSearchState] = useState({
    searchInput: "",
    searchCity: "",
    searchEmail: "",
  });
  const [pagination, setPagination] = useState({
    currentPage: 1,
    usersPerPage: 9,
  });
  const navigate = useNavigate();

  useResetSelectedChatOnRouteChange(setChatInfo);

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const usersData = await userService.getAllUsers(handleError);
        if (usersData) {
          setUsers(usersData);
        }
      } catch (error) {
        console.error("Error fetching users", error);
      }
    };
    fetchUsers();
  }, [token]);

  const handleProfileClick = (userId) => {
    navigate(`/profile/${userId}`);
  };

  const handleSearchChange = (field, value) => {
    setSearchState((prevState) => ({
      ...prevState,
      [field]: value,
    }));
  };

  const handlePageChange = (newPage) => {
    setPagination((prevState) => ({
      ...prevState,
      currentPage: newPage,
    }));
  };

  const filteredUsers = users.filter((user) => {
    const fullName = `${user.firstName} ${user.lastName}`.toLowerCase();
    const searchName = searchState.searchInput.toLowerCase();
    const city = user.city.toLowerCase();
    const searchCityLower = searchState.searchCity.toLowerCase();
    const email = user.email.toLowerCase();
    const searchEmailLower = searchState.searchEmail.toLowerCase();

    return (
      fullName.includes(searchName) &&
      city.includes(searchCityLower) &&
      email.includes(searchEmailLower)
    );
  });

  const indexOfLastUser = pagination.currentPage * pagination.usersPerPage;
  const indexOfFirstUser = indexOfLastUser - pagination.usersPerPage;
  const currentUsers = filteredUsers.slice(indexOfFirstUser, indexOfLastUser);

  const totalPages = Math.ceil(filteredUsers.length / pagination.usersPerPage);

  if (chatInfo.selectedChat) {
    return (
      <Chat
        setChatInfo={setChatInfo}
        chatInfo={chatInfo}
        userInfo={userInfo}
        token={token}
        connection={connection}
        closeChat={closeChat}
        handleError={handleError}
      />
    );
  }

  return (
    <Container maxW="container.xl" py={8}>
      <Box className="min-h-screen p-8 bg-gray-100 rounded-lg shadow-lg">
        <Heading mb={8} textAlign="center" color="teal.500">
          Users
        </Heading>
        <UserSearch
          searchInput={searchState.searchInput}
          setSearchInput={(value) => handleSearchChange("searchInput", value)}
          searchCity={searchState.searchCity}
          setSearchCity={(value) => handleSearchChange("searchCity", value)}
          searchEmail={searchState.searchEmail}
          setSearchEmail={(value) => handleSearchChange("searchEmail", value)}
        />
        <Grid templateColumns="repeat(auto-fill, minmax(250px, 1fr))" gap={6}>
          {currentUsers.map((user) => (
            <GridItem key={user.id}>
              <UserCard
                user={user}
                onProfileClick={handleProfileClick}
                openChat={openChat}
              />
            </GridItem>
          ))}
        </Grid>
        <Pagination
          currentPage={pagination.currentPage}
          totalPages={totalPages}
          paginate={handlePageChange}
        />
      </Box>
    </Container>
  );
};

export default Users;
