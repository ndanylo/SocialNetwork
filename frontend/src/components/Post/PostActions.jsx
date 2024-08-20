import React from "react";
import { HStack, Button } from "@chakra-ui/react";
import { FaHeart } from "react-icons/fa";
import { ViewIcon, ChatIcon } from "@chakra-ui/icons";

const PostActions = ({
  likedByUser,
  likesCount,
  onLike,
  onUnlike,
  onViewLikes,
  onToggleComments,
  commentSectionOpen,
}) => (
  <HStack mt="4" spacing="2">
    <Button
      size="sm"
      colorScheme={likedByUser ? "red" : "gray"}
      onClick={likedByUser ? onUnlike : onLike}
      leftIcon={<FaHeart />}
    >
      {likesCount}
    </Button>
    <Button
      size="sm"
      colorScheme="blue"
      onClick={onViewLikes}
      leftIcon={<ViewIcon />}
    >
      Likes
    </Button>
    <Button
      size="sm"
      colorScheme="green"
      onClick={onToggleComments}
      leftIcon={<ChatIcon />}
    >
      {commentSectionOpen ? "Hide Comments" : "Show Comments"}
    </Button>
  </HStack>
);

export default PostActions;
