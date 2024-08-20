import React, { useState, useEffect } from "react";
import {
  Box,
  Button,
  Input,
  VStack,
  HStack,
  Avatar,
  Text,
} from "@chakra-ui/react";
import commentService from "../../services/commentService";

const CommentSection = ({ postId, handleError }) => {
  const [commentState, setCommentState] = useState({
    comments: [],
    commentContent: "",
  });

  useEffect(() => {
    fetchComments();
  }, []);

  const fetchComments = async () => {
    try {
      const comments = await commentService.fetchComments(postId, handleError);
      if (comments) {
        setCommentState((prevState) => ({
          ...prevState,
          comments: comments,
        }));
      }
    } catch (error) {
      console.error("Error fetching comments", error);
    }
  };

  const handleCreateComment = async () => {
    try {
      const newComment = await commentService.createComment(
        postId,
        commentState.commentContent,
        handleError
      );
      if (newComment) {
        setCommentState((prevState) => ({
          ...prevState,
          comments: [...prevState.comments, newComment],
          commentContent: "",
        }));
      }
    } catch (error) {
      console.error("Error creating comment", error);
    }
  };

  const handleDeleteComment = async (commentId) => {
    try {
      await commentService.deleteComment(commentId, handleError);
      setCommentState((prevState) => ({
        ...prevState,
        comments: prevState.comments.filter(
          (comment) => comment.id !== commentId
        ),
      }));
    } catch (error) {
      console.error("Error deleting comment", error);
    }
  };

  return (
    <Box mt="4">
      <VStack spacing="4" align="start">
        {commentState.comments.map((comment) => (
          <Box key={comment.id} p="2" bg="gray.100" borderRadius="md" w="full">
            <HStack align="start">
              <Avatar name={comment.userName} />
              <VStack align="start" spacing="1" flex="1">
                <HStack justifyContent="space-between" w="full">
                  <Text fontWeight="bold">{comment.userName}</Text>
                  <Button
                    size="xs"
                    colorScheme="red"
                    onClick={() => handleDeleteComment(comment.id)}
                  >
                    Delete
                  </Button>
                </HStack>
                <Text>{comment.content}</Text>
                <Text fontSize="xs" color="gray.500">
                  {new Date(comment.createdAt).toLocaleString()}
                </Text>
              </VStack>
            </HStack>
          </Box>
        ))}
        <HStack w="full">
          <Input
            placeholder="Write a comment..."
            value={commentState.commentContent}
            onChange={(e) =>
              setCommentState((prevState) => ({
                ...prevState,
                commentContent: e.target.value,
              }))
            }
          />
          <Button onClick={handleCreateComment} colorScheme="blue">
            Comment
          </Button>
        </HStack>
      </VStack>
    </Box>
  );
};

export default CommentSection;
