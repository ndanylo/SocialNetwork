import { HubConnectionBuilder } from "@microsoft/signalr";
import { jwtDecode } from "jwt-decode";
import config from "../config.json";

const API_URL = config.API_URL;

export const createHubConnection = (token, setConnection, setUserId) => {
  try {
    const connection = new HubConnectionBuilder()
      .withUrl(`${API_URL}/chat`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        setConnection(connection);
        const decodedToken = jwtDecode(token);
        setUserId(decodedToken.nameid);
      })
      .catch((error) => console.error("Error connecting to SignalR", error));

    return connection;
  } catch (error) {
    console.error("Error creating Hub connection:", error);
  }
};
