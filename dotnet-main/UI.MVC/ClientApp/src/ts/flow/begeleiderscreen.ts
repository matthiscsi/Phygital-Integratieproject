import * as signalR from "@microsoft/signalr";
import {pauseTimer, resumeTimer} from "./timer";

document.addEventListener('DOMContentLoaded', async function () {
    const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement | null;
    const flowidDom = document.getElementById("flowid") as HTMLSpanElement | null;
    const isTimerScreen = document.getElementById("timerScreen") !== null; // Check if timer screen ID object exists

    flowSessionId = parseInt(flowsessionIdDom?.innerHTML || "0");
    flowid = parseInt(flowidDom?.innerHTML || "0");

    if (isTimerScreen) {
        // Start timer only if it's the timer screen
        startTimer();
    }
});

window.addEventListener('beforeunload', async function () {
    console.log("Closing connection");
    connection.invoke("ShowQrCode", flowSessionId).catch(err => console.error("Error removing QR code:", err.toString()));
});

const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement | null;
const flowidDom = document.getElementById("flowid") as HTMLSpanElement | null;
const pauseButton = document.getElementById('playPauseButton');
export let flowid: number;
export let flowSessionId: number;
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/flowStepHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect([0, 2000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.keepAliveIntervalInMilliseconds = 10000; // 30 seconds
connection.serverTimeoutInMilliseconds = 150000; // 60 seconds

connection.start().then(() => {
    joinGroup().then(r => console.log("Group joined successfully"));
}).catch(err => console.error("Error establishing connection:", err));

connection.onreconnected(async (connectionId) => {
    console.log("Reconnected, attempting to rejoin group...");
    await joinGroup();
});

async function joinGroup() {
    try {
        connection.invoke("SetHost", flowSessionId).catch(err => console.error("Error setting host:", err));
        connection.invoke("JoinGroup", flowSessionId).catch(err => console.error("Error joining group:", err.toString()));
        console.log("Joined group with session ID:", flowSessionId);
        console.log("RemoveQrCode");
        connection.invoke("RemoveQrCode", flowSessionId).catch(err => console.error("Error removing QR code:", err.toString()));
    } catch (err) {
        console.error("Error joining group:", err);
    }
}

connection.on("PauseTimer", async (sessionId) => {
    console.log("Received pause timer for session:", sessionId);
    if (sessionId === flowSessionId) {
        pauseTimer();
    }
});

connection.on("ResumeTimer", async (sessionId) => {
    console.log("Received resume timer for session:", sessionId);
    if (sessionId === flowSessionId) {
        resumeTimer();
    }
});

pauseButton?.addEventListener('click', async () => {
    if (pauseButton.classList.contains('bi-pause')) {
        console.log("Sending pause timer to clients");
        await sendPauseTimerToClients(flowSessionId);
        pauseTimer();
        pauseButton.classList.remove('bi-pause');
        pauseButton.classList.add('bi-play');
    } else {
        console.log("Sending resume timer to clients");
        await sendResumeTimerToClients(flowSessionId);
        resumeTimer();
        pauseButton.classList.remove('bi-play');
        pauseButton.classList.add('bi-pause');
    }
});

export async function sendPauseTimerToClients(sessionId: number) {
    await connection.invoke("PauseTimer", sessionId)
        .catch(err => console.error("Error on pause timer:", err));
}

export async function sendResumeTimerToClients(sessionId: number) {
    await connection.invoke("ResumeTimer", sessionId)
        .catch(err => console.error("Error on resume timer:", err));
}

function startTimer() {
resumeTimer();
}
