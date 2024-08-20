import React, { useState } from "react";
import {
  Box,
  Heading,
  Textarea,
  Input,
  Button,
  FormControl,
  FormLabel,
} from "@chakra-ui/react";
import { AttachmentIcon } from "@chakra-ui/icons";
import postService from "../../services/postService";

const CreatePostForm = ({ handleError, setUser }) => {
  const [newPostContent, setNewPostContent] = useState("");
  const [newPostImage, setNewPostImage] = useState(null);

  const handleCreatePost = async () => {
    try {
      const formData = new FormData();
      formData.append("content", newPostContent);
      formData.append("image", newPostImage);

      const newPost = await postService.createPost(formData, handleError);
      if (newPost) {
        setUser((prevUser) => ({
          ...prevUser,
          posts: [newPost, ...prevUser.posts],
        }));
        setNewPostContent("");
        setNewPostImage(null);
      }
    } catch (error) {
      console.error("Error creating post", error);
    }
  };

  return (
    <Box mt="4" p="6" bg="white" shadow="md" borderRadius="md">
      <Heading size="sm" mb="4" color="gray.700">
        Create a new post
      </Heading>
      <Textarea
        placeholder="What's on your mind?"
        value={newPostContent}
        onChange={(e) => setNewPostContent(e.target.value)}
        mb="4"
      />
      <FormControl id="post-image">
        <FormLabel color="gray.700">Upload Image</FormLabel>
        <Input
          type="file"
          accept="image/*"
          onChange={(e) => setNewPostImage(e.target.files[0])}
        />
      </FormControl>
      <Button
        colorScheme="teal"
        onClick={handleCreatePost}
        mt="4"
        leftIcon={<AttachmentIcon />}
      >
        Post
      </Button>
    </Box>
  );
};

export default CreatePostForm;
