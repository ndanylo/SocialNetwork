import {
  Box,
  Heading,
  Text,
  List,
  ListItem,
  Button,
  Avatar,
  Flex,
  IconButton,
  Collapse,
  useDisclosure,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";
import { convertBytesToBase64Image } from "../../utils/imageUtils";
import { ChevronDownIcon, ChevronUpIcon, EmailIcon } from "@chakra-ui/icons";

const ReceivedFriendRequests = ({
  receivedFriendRequests,
  handleAcceptFriendRequest,
  handleDeclineFriendRequest,
}) => {
  const { isOpen, onToggle } = useDisclosure();

  const requests = Array.isArray(receivedFriendRequests)
    ? receivedFriendRequests
    : [];

  return (
    <Box
      borderWidth="1px"
      borderRadius="lg"
      overflow="hidden"
      p={4}
      mb={4}
      onClick={onToggle}
    >
      <Flex justify="space-between" align="center" mb={3}>
        <Flex align="center">
          <EmailIcon mr={2} />
          <Heading size="md" color="teal.500">
            Friend Requests
          </Heading>
          {requests.length > 0 && (
            <Text color="red" fontSize="lg" fontWeight="bold">
              ({requests.length})
            </Text>
          )}
        </Flex>
        <IconButton
          icon={isOpen ? <ChevronUpIcon /> : <ChevronDownIcon />}
          onClick={onToggle}
          aria-label={
            isOpen ? "Collapse Friend Requests" : "Expand Friend Requests"
          }
          variant="outline"
          size="sm"
        />
      </Flex>
      <Collapse in={isOpen} animateOpacity>
        {requests.length > 0 ? (
          <List spacing={3}>
            {requests.map((user) => (
              <ListItem key={user.id}>
                <Flex alignItems="center">
                  <Avatar
                    name={user.userName}
                    mr={3}
                    src={convertBytesToBase64Image(user.avatar)}
                  />
                  <Link
                    to={`/profile/${user.id}`}
                    style={{ textDecoration: "none" }}
                  >
                    <Text cursor="pointer">
                      {user.firstName} {user.lastName}
                    </Text>
                    <Text cursor="pointer">{user.userName}</Text>
                  </Link>
                  <Button
                    colorScheme="green"
                    size="xs"
                    ml={3}
                    onClick={() => handleAcceptFriendRequest(user.id)}
                  >
                    Accept
                  </Button>
                  <Button
                    colorScheme="red"
                    size="xs"
                    ml={3}
                    onClick={() => handleDeclineFriendRequest(user.id)}
                  >
                    Decline
                  </Button>
                </Flex>
              </ListItem>
            ))}
          </List>
        ) : (
          <Text>No friend requests found.</Text>
        )}
      </Collapse>
    </Box>
  );
};

export default ReceivedFriendRequests;
