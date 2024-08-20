import React from "react";
import {
  Box,
  FormControl,
  FormLabel,
  InputGroup,
  InputLeftElement,
  Input,
  SimpleGrid,
  Icon,
} from "@chakra-ui/react";
import { SearchIcon, EmailIcon, AtSignIcon } from "@chakra-ui/icons";

const UserSearch = ({
  searchInput,
  setSearchInput,
  searchCity,
  setSearchCity,
  searchEmail,
  setSearchEmail,
}) => (
  <Box bg="white" p={6} rounded="lg" shadow="lg" mb={8}>
    <SimpleGrid columns={{ base: 1, md: 3 }} spacing={4}>
      <FormControl id="search-name">
        <FormLabel>Search by Name</FormLabel>
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <Icon as={AtSignIcon} color="gray.400" />
          </InputLeftElement>
          <Input
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            placeholder="Enter name"
            focusBorderColor="teal.400"
            variant="filled"
          />
        </InputGroup>
      </FormControl>
      <FormControl id="search-city">
        <FormLabel>Search by City</FormLabel>
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <Icon as={SearchIcon} color="gray.400" />
          </InputLeftElement>
          <Input
            value={searchCity}
            onChange={(e) => setSearchCity(e.target.value)}
            placeholder="Enter city"
            focusBorderColor="teal.400"
            variant="filled"
          />
        </InputGroup>
      </FormControl>
      <FormControl id="search-email">
        <FormLabel>Search by Email</FormLabel>
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <Icon as={EmailIcon} color="gray.400" />
          </InputLeftElement>
          <Input
            value={searchEmail}
            onChange={(e) => setSearchEmail(e.target.value)}
            placeholder="Enter email"
            focusBorderColor="teal.400"
            variant="filled"
          />
        </InputGroup>
      </FormControl>
    </SimpleGrid>
  </Box>
);

export default UserSearch;
