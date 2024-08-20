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

const FriendList = ({ friends, userId, profileUserId, handleDeleteFriend }) => {
  const { isOpen, onToggle } = useDisclosure();
  const friendsList = Array.isArray(friends) ? friends : [];

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
            Friends
          </Heading>
          {friendsList.length > 0 && (
            <Text color="red" fontSize="lg" fontWeight="bold">
              ({friendsList.length})
            </Text>
          )}
        </Flex>
        <IconButton
          icon={isOpen ? <ChevronUpIcon /> : <ChevronDownIcon />}
          onClick={onToggle}
          aria-label={isOpen ? "Collapse Friends" : "Expand Friends"}
          variant="outline"
          size="sm"
        />
      </Flex>
      <Collapse in={isOpen} animateOpacity>
        {friendsList.length > 0 ? (
          <List spacing={3}>
            {friendsList.map((user) => (
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
                  {userId === profileUserId && (
                    <Button
                      colorScheme="red"
                      size="xs"
                      ml={3}
                      onClick={() => handleDeleteFriend(user.id)}
                    >
                      Remove
                    </Button>
                  )}
                </Flex>
              </ListItem>
            ))}
          </List>
        ) : (
          <Text>No friends found.</Text>
        )}
      </Collapse>
    </Box>
  );
};

export default FriendList;
