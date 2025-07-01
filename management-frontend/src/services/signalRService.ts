import * as signalR from "@microsoft/signalr";
import { API_BASE_URL } from "../utils/constants";

export const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE_URL}/hub/notifications`, { withCredentials: false, accessTokenFactory: () => sessionStorage.getItem("token") || localStorage.getItem("token") || "" }) // Không truyền { withCredentials: true }
    .withAutomaticReconnect()
    .build();

export const startHubConnection = async () => {
    if (hubConnection.state === "Disconnected") {
        try {
            await hubConnection.start();
            console.log("SignalR connected");
        } catch (err) {
            console.error("SignalR connection error:", err);
        }
    }
};