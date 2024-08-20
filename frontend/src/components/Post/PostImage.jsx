import React from "react";
import { Image } from "@chakra-ui/react";
import { convertBytesToBase64Image } from "../../utils/imageUtils";

const PostImage = ({ image }) => (
  <Image
    src={convertBytesToBase64Image(image, "Post Image")}
    mt="2"
    borderRadius="md"
    minW="300px"
    maxW="800px"
    mx="auto"
  />
);

export default PostImage;
