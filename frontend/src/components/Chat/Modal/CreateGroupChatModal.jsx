import React, { useState, useEffect } from "react";
import {
  Modal,
  ModalOverlay,
  ModalContent,
  ModalHeader,
  ModalFooter,
  ModalBody,
  ModalCloseButton,
  Button,
  Checkbox,
  CheckboxGroup,
  VStack,
  Text,
  Input,
} from "@chakra-ui/react";
import userService from "../../../services/userService";
import chatService from "../../../services/chatService";

const CreateGroupChatModal = ({ userId, onCreate }) => {
  const [modalState, setModalState] = useState({
    isOpen: false,
    chatName: "",
    errorMessage: "",
  });
  const [friends, setFriends] = useState([]);
  const [selectedFriends, setSelectedFriends] = useState([]);

  useEffect(() => {
    const fetchFriends = async () => {
      try {
        const friends = await userService.getFriends(userId);
        if (friends) {
          setFriends(friends);
        }
      } catch (error) {
        console.error("Error fetching friends", error);
      }
    };

    if (modalState.isOpen) {
      fetchFriends();
    }
  }, [modalState.isOpen, userId]);

  const openModal = () => {
    setModalState((prevState) => ({ ...prevState, isOpen: true }));
  };

  const closeModal = () => {
    setModalState({
      isOpen: false,
      chatName: "",
      errorMessage: "",
    });
    setSelectedFriends([]);
  };

  const handleCreateGroupChat = async () => {
    if (selectedFriends.length < 2) {
      setModalState((prevState) => ({
        ...prevState,
        errorMessage: "Group chat must have at least 2 friends",
      }));
      return;
    }

    try {
      const chat = await chatService.createGroupChat(
        modalState.chatName,
        selectedFriends
      );
      onCreate(chat);
      closeModal();
    } catch (error) {
      console.error("Error creating group chat", error);
    }
  };

  return (
    <>
      <Button onClick={openModal} colorScheme="teal" variant="solid">
        Create Group Chat
      </Button>
      <Modal isOpen={modalState.isOpen} onClose={closeModal}>
        <ModalOverlay />
        <ModalContent>
          <ModalHeader>Create Group Chat</ModalHeader>
          <ModalCloseButton />
          <ModalBody>
            {modalState.errorMessage && (
              <Text color="red.500" mb={4}>
                {modalState.errorMessage}
              </Text>
            )}
            <VStack spacing={4}>
              <Input
                placeholder="Chat Name"
                value={modalState.chatName}
                onChange={(e) =>
                  setModalState((prevState) => ({
                    ...prevState,
                    chatName: e.target.value,
                  }))
                }
              />
              <CheckboxGroup
                value={selectedFriends}
                onChange={setSelectedFriends}
              >
                {friends.map((friend) => (
                  <Checkbox key={friend.id} value={friend.id}>
                    {friend.userName}
                  </Checkbox>
                ))}
              </CheckboxGroup>
            </VStack>
          </ModalBody>
          <ModalFooter>
            <Button colorScheme="teal" mr={3} onClick={handleCreateGroupChat}>
              Create
            </Button>
            <Button variant="ghost" onClick={closeModal}>
              Cancel
            </Button>
          </ModalFooter>
        </ModalContent>
      </Modal>
    </>
  );
};

export default CreateGroupChatModal;
