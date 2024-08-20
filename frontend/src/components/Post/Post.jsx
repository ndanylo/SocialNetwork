import React, { useState } from "react";
import { Box, Text, useDisclosure, useToast } from "@chakra-ui/react";
import CommentSection from "./CommentSection";
import postService from "../../services/postService";
import PostHeader from "./PostHeader";
import PostImage from "./PostImage";
import PostActions from "./PostActions";
import LikesModal from "./Modal/LikesModal";

const Post = ({ post, setPosts, posts, handleError, userId }) => {
  const [postState, setPostState] = useState({
    likes: [],
    commentSectionOpen: false,
    isLikesModalOpen: false,
    likedByUser: post.likedByUser,
    likesCount: post.likesCount,
  });

  const toast = useToast();
  const { isOpen, onOpen, onClose } = useDisclosure();

  const handleLikePost = async () => {
    try {
      if (post.user.id === userId) {
        toast({
          title: "Ошибка!",
          description: "Вы не можете лайкнуть свой собственный пост",
          status: "error",
          duration: 3000,
          isClosable: true,
        });
        return;
      }

      await postService.likePost(post.id, handleError);
      setPostState((prevState) => ({
        ...prevState,
        likesCount: prevState.likesCount + 1,
        likedByUser: true,
      }));
      setPosts(
        posts.map((p) =>
          p.id === post.id
            ? { ...p, likesCount: postState.likesCount + 1, likedByUser: true }
            : p
        )
      );
    } catch (error) {
      console.error("Error liking post", error);
    }
  };

  const handleUnlikePost = async () => {
    try {
      await postService.unlikePost(post.id, handleError);
      setPostState((prevState) => ({
        ...prevState,
        likesCount: prevState.likesCount - 1,
        likedByUser: false,
      }));
      setPosts(
        posts.map((p) =>
          p.id === post.id
            ? { ...p, likesCount: postState.likesCount - 1, likedByUser: false }
            : p
        )
      );
    } catch (error) {
      console.error("Error unliking post", error);
    }
  };

  const fetchPostLikes = async () => {
    try {
      const likes = await postService.fetchPostLikes(post.id, handleError);
      if (likes) {
        setPostState((prevState) => ({
          ...prevState,
          likes: likes,
        }));
        onOpen();
      }
    } catch (error) {
      console.error("Error fetching post likes", error);
    }
  };

  const toggleCommentSection = () => {
    setPostState((prevState) => ({
      ...prevState,
      commentSectionOpen: !prevState.commentSectionOpen,
    }));
  };

  return (
    <Box display="flex" justifyContent="center" mt="4">
      <Box p="4" bg="white" shadow="md" borderRadius="md" w="100%" maxW="100%">
        <PostHeader user={post.user} createdAt={post.createdAt} />
        <Text mt="2">{post.content}</Text>
        {post.image && <PostImage image={post.image} />}
        <PostActions
          likedByUser={postState.likedByUser}
          likesCount={postState.likesCount}
          onLike={handleLikePost}
          onUnlike={handleUnlikePost}
          onViewLikes={fetchPostLikes}
          onToggleComments={toggleCommentSection}
          commentSectionOpen={postState.commentSectionOpen}
        />
        {postState.commentSectionOpen && (
          <CommentSection postId={post.id} handleError={handleError} />
        )}
        <LikesModal isOpen={isOpen} onClose={onClose} likes={postState.likes} />
      </Box>
    </Box>
  );
};

export default Post;
