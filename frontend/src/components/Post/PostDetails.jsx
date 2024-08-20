import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Container, Spinner } from "@chakra-ui/react";
import postService from "../../services/postService";
import Post from "./Post";

const PostDetail = ({ token, handleError }) => {
  const { postId } = useParams();

  const [state, setState] = useState({
    posts: [],
    loading: true,
  });

  useEffect(() => {
    const fetchPost = async () => {
      try {
        const post = await postService.fetchPost(postId, handleError);
        if (post) {
          setState((prevState) => ({
            ...prevState,
            posts: [post],
          }));
        }
      } catch (error) {
        console.error("Error fetching post", error);
      } finally {
        setState((prevState) => ({
          ...prevState,
          loading: false,
        }));
      }
    };

    fetchPost();
  }, [postId, token]);

  const handleUpdatePost = (updatedPost) => {
    setState((prevState) => ({
      ...prevState,
      posts: [updatedPost],
    }));
  };

  return (
    <Container maxW="container.xl" py={8}>
      {state.loading ? (
        <Spinner />
      ) : (
        state.posts[0] && (
          <Post
            post={state.posts[0]}
            setPosts={(updatedPost) => handleUpdatePost(updatedPost)}
            posts={state.posts}
            avatarPhoto={state.posts[0].image}
            handleError={handleError}
          />
        )
      )}
    </Container>
  );
};

export default PostDetail;
