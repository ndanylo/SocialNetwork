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
  Text,
  Avatar,
  Box,
} from "@chakra-ui/react";
import { convertBytesToBase64Image } from "../../../utils/imageUtils";

const ReadByModal = ({ isOpen, onClose, readBy }) => {
  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <ModalOverlay />
      <ModalContent>
        <ModalHeader>Read by</ModalHeader>
        <ModalCloseButton />
        <ModalBody>
          {readBy.length > 0 ? (
            readBy.map((user, index) => (
              <Box key={index} display="flex" alignItems="center" mb={2}>
                <Avatar
                  name={user.userName}
                  src={convertBytesToBase64Image(user.avatar)}
                  size="md"
                  mr={3}
                />
                <Text>
                  {user.userName} ({user.email})
                </Text>
              </Box>
            ))
          ) : (
            <Text>No one has read this message yet.</Text>
          )}
        </ModalBody>
        <ModalFooter>
          <Button colorScheme="teal" onClick={onClose}>
            Close
          </Button>
        </ModalFooter>
      </ModalContent>
    </Modal>
  );
};

export default ReadByModal;
