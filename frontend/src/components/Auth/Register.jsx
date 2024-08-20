import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  Box,
  Container,
  Heading,
  Input,
  Button,
  FormControl,
  FormLabel,
  VStack,
  useToast,
} from "@chakra-ui/react";
import authService from "../../services/authService";

const Register = () => {
  const [userData, setUserData] = useState({
    email: "",
    firstName: "",
    lastName: "",
    city: "",
    password: "",
    confirmPassword: "",
  });
  const [fileData, setFileData] = useState({
    avatar: null,
  });
  const toast = useToast();
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setUserData((prevUserData) => ({
      ...prevUserData,
      [name]: value,
    }));
  };

  const handleFileChange = (e) => {
    const { name, files } = e.target;
    setFileData((prevFileData) => ({
      ...prevFileData,
      [name]: files[0],
    }));
  };

  const handleRegister = async () => {
    const { password, confirmPassword } = userData;
    if (password !== confirmPassword) {
      toast({
        title: "Passwords do not match.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
      return;
    }

    const formData = new FormData();
    for (const key in userData) {
      formData.append(key, userData[key]);
    }
    if (fileData.avatar) {
      formData.append("Avatar", fileData.avatar);
    }

    try {
      await authService.register(formData);
      toast({
        title: "Registration successful.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
      navigate("/auth");
    } catch (error) {
      toast({
        title: "Registration failed.",
        description: error.response?.data || "An error occurred.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Container maxW="container.md" py={8}>
      <Box p="8" bg="white" shadow="md" borderRadius="md">
        <Heading mb="6" color="teal.500">
          Register
        </Heading>
        <VStack spacing="4">
          <FormControl id="email">
            <FormLabel>Email</FormLabel>
            <Input
              type="email"
              name="email"
              value={userData.email}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="firstName">
            <FormLabel>First Name</FormLabel>
            <Input
              type="text"
              name="firstName"
              value={userData.firstName}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="lastName">
            <FormLabel>Last Name</FormLabel>
            <Input
              type="text"
              name="lastName"
              value={userData.lastName}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="city">
            <FormLabel>City</FormLabel>
            <Input
              type="text"
              name="city"
              value={userData.city}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="password">
            <FormLabel>Password</FormLabel>
            <Input
              type="password"
              name="password"
              value={userData.password}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="confirmPassword">
            <FormLabel>Confirm Password</FormLabel>
            <Input
              type="password"
              name="confirmPassword"
              value={userData.confirmPassword}
              onChange={handleChange}
            />
          </FormControl>
          <FormControl id="avatar">
            <FormLabel>Avatar</FormLabel>
            <Input
              type="file"
              name="avatar"
              accept="image/*"
              onChange={handleFileChange}
            />
          </FormControl>
          <FormLabel mt={4} textAlign="center">
            Уже есть аккаунт?{" "}
            <Link
              to="/auth"
              style={{ color: "teal", textDecoration: "underline" }}
            >
              Логин
            </Link>
          </FormLabel>
          <Button colorScheme="teal" onClick={handleRegister}>
            Register
          </Button>
        </VStack>
      </Box>
    </Container>
  );
};

export default Register;
