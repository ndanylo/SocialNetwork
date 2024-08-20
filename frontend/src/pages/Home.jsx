import React, { useEffect, useState } from "react";
import {
  Container,
  Box,
  Heading,
  Stack,
  ButtonGroup,
  IconButton,
  HStack,
} from "@chakra-ui/react";
import {
  FaSortAmountDown,
  FaSortAmountUp,
  FaRegClock,
  FaThumbsUp,
} from "react-icons/fa";
import PostList from "../components/Post/PostList";
import postService from "../services/postService";

const Home = ({ token, handleError }) => {
  const [posts, setPosts] = useState([]);
  const [sortConfig, setSortConfig] = useState({
    key: "createdAt",
    direction: "desc",
  });

  useEffect(() => {
    const fetchPosts = async () => {
      try {
        const posts = await postService.getFriendsPosts(handleError);
        if (Array.isArray(posts)) {
          setPosts(posts);
        } else {
          console.error("Expected an array of posts");
        }
      } catch (error) {
        console.error("Error fetching posts", error);
      }
    };

    fetchPosts();
  }, [token, handleError]);

  const sortPosts = (key) => {
    const direction =
      sortConfig.key === key && sortConfig.direction === "asc" ? "desc" : "asc";
    setSortConfig({ key, direction });
  };

  const sortedPosts = [...posts].sort((a, b) => {
    let aValue = a[sortConfig.key];
    let bValue = b[sortConfig.key];

    if (sortConfig.key === "createdAt") {
      aValue = new Date(aValue);
      bValue = new Date(bValue);

      return sortConfig.direction === "asc" ? aValue - bValue : bValue - aValue;
    }

    if (aValue < bValue) return sortConfig.direction === "asc" ? -1 : 1;
    if (aValue > bValue) return sortConfig.direction === "asc" ? 1 : -1;
    return 0;
  });

  return (
    <Container maxW="container.xl" py={8}>
      <Box className="min-h-screen p-8 bg-gray-100">
        <Heading mb="8">News Feed</Heading>
        <HStack mb="4" spacing="4">
          <ButtonGroup isAttached variant="outline">
            <IconButton
              icon={<FaRegClock />}
              onClick={() => sortPosts("createdAt")}
              aria-label="Sort by date"
              title="Sort by date"
            />
            <IconButton
              icon={
                sortConfig.key === "createdAt" &&
                sortConfig.direction === "asc" ? (
                  <FaSortAmountUp />
                ) : (
                  <FaSortAmountDown />
                )
              }
              onClick={() => sortPosts("createdAt")}
              aria-label="Toggle sort direction for date"
              title="Toggle sort direction for date"
            />
          </ButtonGroup>
          <ButtonGroup isAttached variant="outline">
            <IconButton
              icon={<FaThumbsUp />}
              onClick={() => sortPosts("likesCount")}
              aria-label="Sort by likes"
              title="Sort by likes"
            />
            <IconButton
              icon={
                sortConfig.key === "likesCount" &&
                sortConfig.direction === "asc" ? (
                  <FaSortAmountUp />
                ) : (
                  <FaSortAmountDown />
                )
              }
              onClick={() => sortPosts("likesCount")}
              aria-label="Toggle sort direction for likes"
              title="Toggle sort direction for likes"
            />
          </ButtonGroup>
        </HStack>
        <Stack spacing="4">
          <PostList
            posts={sortedPosts}
            setPosts={setPosts}
            handleError={handleError}
          />
        </Stack>
      </Box>
    </Container>
  );
};

export default Home;
