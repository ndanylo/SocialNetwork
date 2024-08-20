import React from "react";
import { Box, Text } from "@chakra-ui/react";
import Post from "./Post";

const PostList = ({ posts, setPosts, handleError, userId }) => {
  return (
    <Box>
      {posts.length > 0 ? (
        posts.map((post) => (
          <Post
            key={post.id}
            post={post}
            userId={userId}
            setPosts={setPosts}
            posts={posts}
            handleError={handleError}
          />
        ))
      ) : (
        <Text>No posts available</Text>
      )}
    </Box>
  );
};

export default PostList;
