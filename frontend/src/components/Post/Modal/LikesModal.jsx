import React from "react";
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  Button,
  HStack,
  Avatar,
  Text,
  Link,
} from "@chakra-ui/react";
import { convertBytesToBase64Image } from "../../../utils/imageUtils";

const LikesModal = ({ isOpen, onClose, likes }) => (
  <Modal isOpen={isOpen} onClose={onClose}>
    <ModalOverlay />
    <ModalContent>
      <ModalHeader>Likes</ModalHeader>
      <ModalCloseButton />
      <ModalBody>
        {likes.length > 0 ? (
          likes.map((like) => (
            <HStack key={like.user.id} spacing="6" align="center" mt="4">
              <Avatar
                name={like.user.email}
                src={convertBytesToBase64Image(
                  like.user.avatar,
                  like.user.email
                )}
              />
              <Link
                to={`/profile/${like.user.id}`}
                style={{ textDecoration: "none" }}
              >
                <Text cursor="pointer">
                  {like.user.firstName} {like.user.lastName}
                </Text>
                <Text cursor="pointer">{like.user.userName}</Text>
              </Link>
            </HStack>
          ))
        ) : (
          <Text>No likes yet</Text>
        )}
      </ModalBody>
      <ModalFooter>
        <Button colorScheme="blue" onClick={onClose}>
          Close
        </Button>
      </ModalFooter>
    </ModalContent>
  </Modal>
);

export default LikesModal;
