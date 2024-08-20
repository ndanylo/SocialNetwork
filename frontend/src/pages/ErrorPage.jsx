import React from "react";
import { Box, Heading, Text, Button } from "@chakra-ui/react";

const ErrorPage = ({ errorMessage, removeError }) => {
  const handleGoBack = () => {
    removeError();
    window.location.href = "/home";
  };

  return (
    <Box textAlign="center" py={10} px={6}>
      <Heading as="h2" size="xl" mt={6} mb={2}>
        Oops! Something went wrong.
      </Heading>
      <Text color={"gray.500"}>An unexpected error has occurred.</Text>
      <Text color={"gray.500"}>{errorMessage}</Text>
      <Button colorScheme="teal" variant="solid" mt={4} onClick={handleGoBack}>
        Go to Home
      </Button>
    </Box>
  );
};

export default ErrorPage;
