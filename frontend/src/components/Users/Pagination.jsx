import React from "react";
import { Button, Stack } from "@chakra-ui/react";

const Pagination = ({ currentPage, totalPages, paginate }) => (
  <Stack direction="row" spacing={4} justify="center" mt={8}>
    {Array.from({ length: totalPages }, (_, index) => (
      <Button
        key={index + 1}
        onClick={() => paginate(index + 1)}
        colorScheme={currentPage === index + 1 ? "teal" : "gray"}
        variant={currentPage === index + 1 ? "solid" : "outline"}
      >
        {index + 1}
      </Button>
    ))}
  </Stack>
);

export default Pagination;
