import React from "react";
import { Avatar, Box } from "@chakra-ui/react";
import { convertBytesToBase64Image } from "../../utils/imageUtils";

const AvatarSection = ({ user }) => {
  return (
    <Box flex="1" display="flex" justifyContent="flex-end">
      <Avatar
        style={{ width: "350px", height: "350px", marginRight: "50px" }}
        name={user.userName}
        src={convertBytesToBase64Image(user.avatar)}
        alignSelf="center"
      />
    </Box>
  );
};

export default AvatarSection;
