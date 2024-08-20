import React from "react";
import { Box, Heading } from "@chakra-ui/react";
import PostList from "../Post/PostList";

const PostListSection = ({ user, handleError, setUser, userId }) => {
  return (
    <Box mt="8">
      <Heading size="md" color="teal.500">
        Posts
      </Heading>
      <Box mt="4">
        <PostList
          userId={userId}
          posts={user.posts}
          handleError={handleError}
          setPosts={(updatedPosts) => setUser({ ...user, posts: updatedPosts })}
        />
      </Box>
    </Box>
  );
};

export default PostListSection;
