import React from "react";
import {
  Box,
  Flex,
  Text,
  Link,
  IconButton,
  Avatar,
  useColorModeValue,
} from "@chakra-ui/react";
import { CheckIcon } from "@chakra-ui/icons";
import { convertBytesToBase64Image } from "../../utils/imageUtils";

const Message = ({
  messageInfo,
  userId,
  handleShowReadBy,
  formatTimestamp,
}) => {
  const userBg = useColorModeValue("blue.100", "blue.900");
  const otherBg = useColorModeValue("gray.200", "gray.700");

  return (
    messageInfo && (
      <Flex
        bg={messageInfo.user.id === userId ? userBg : otherBg}
        alignSelf={messageInfo.user.id === userId ? "flex-end" : "flex-start"}
        id={`message-${messageInfo.id}`}
        maxWidth="75%"
        borderRadius="md"
        p={3}
        mb={4}
        shadow="md"
      >
        <Avatar
          name={messageInfo.user.userName}
          src={convertBytesToBase64Image(messageInfo.user.avatar)}
          size="md"
          mr={3}
        />
        <Box>
          <Text>{messageInfo.content}</Text>
          <Flex justify="space-between" mt={1} fontSize="sm" color="gray.500">
            <Link to={`/profile/${messageInfo.userId}`} color="teal.500">
              {messageInfo.user.userName}
            </Link>
            <Text>- {formatTimestamp(messageInfo.timestamp)}</Text>
            {messageInfo.user.id === userId && (
              <IconButton
                marginLeft="10px"
                size="xs"
                onClick={() => handleShowReadBy(messageInfo.readBy)}
                icon={
                  messageInfo.readBy.length > 0 ? (
                    <Box>
                      <CheckIcon color="green.500" />
                      <CheckIcon color="green.500" marginLeft="-5px" />
                    </Box>
                  ) : (
                    <CheckIcon />
                  )
                }
                aria-label="Read by status"
              />
            )}
          </Flex>
        </Box>
      </Flex>
    )
  );
};

export default Message;
