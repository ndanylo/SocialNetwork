import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  Box,
  Container,
  Heading,
  Input,
  Button,
  FormControl,
  FormLabel,
  VStack,
  Text,
  useToast,
} from "@chakra-ui/react";
import authService from "../../services/authService";

const Login = ({ token, setToken }) => {
  const [credentials, setCredentials] = useState({ email: "", password: "" });
  const navigate = useNavigate();
  const toast = useToast();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setCredentials((prevCredentials) => ({
      ...prevCredentials,
      [name]: value,
    }));
  };

  const login = async () => {
    try {
      const token = await authService.login(
        credentials.email,
        credentials.password
      );
      await setToken(token);
      toast({
        title: "Login successful.",
        status: "success",
        duration: 5000,
        isClosable: true,
      });
    } catch (error) {
      toast({
        title: "Login failed.",
        description: error.response?.data || "An error occurred.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    }
  };

  useEffect(() => {
    if (token != null) {
      navigate("/home");
    }
  }, [token, navigate]);

  return (
    <Container maxW="container.md" py={8}>
      <Box p="8" bg="white" shadow="md" borderRadius="md">
        <Heading mb="6" color="teal.500">
          Login
        </Heading>
        <VStack spacing="4">
          <FormControl id="email">
            <FormLabel>Email</FormLabel>
            <Input
              type="email"
              name="email"
              value={credentials.email}
              onChange={handleChange}
              focusBorderColor="teal.500"
            />
          </FormControl>
          <FormControl id="password">
            <FormLabel>Password</FormLabel>
            <Input
              type="password"
              name="password"
              value={credentials.password}
              onChange={handleChange}
              focusBorderColor="teal.500"
            />
          </FormControl>
          <Button colorScheme="teal" onClick={login}>
            Login
          </Button>
        </VStack>
        <Text mt="4" textAlign="center">
          Нет аккаунта?{" "}
          <Link color="teal.500" to={"/register"}>
            Зарегистрируйтесь
          </Link>
        </Text>
      </Box>
    </Container>
  );
};

export default Login;
