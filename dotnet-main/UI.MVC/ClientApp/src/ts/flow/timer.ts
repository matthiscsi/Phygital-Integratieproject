import * as signalR from "@microsoft/signalr";

let timer: NodeJS.Timeout;
let domprojectId: number;
let flowsoort: boolean;
const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement;
const isTimerScreen = document.getElementById("timerScreen") !== null;
let flowSessionId = 0;
let elapsedSeconds = 0;
const timerDuration = 20000;
let shouldListenForActivity = true;

if (flowsessionIdDom != null) {
    flowSessionId = parseInt(flowsessionIdDom.innerHTML);
    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/flowStepHub", {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 10000, 30000])
        .build();

    connection.on("ResetTimer", function () {
        resetTimer();
    });

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
}

function redirectToScreen() {
    let url;

    if (flowsoort) {
        url = `/Home/WaitScreen?flowsessionid=${flowSessionId}`;
    } else {
        url = `/Home/Startup?id=${domprojectId}`;
    }

    // @ts-ignore
    window.location.href = url;
}

function startTimer() {
    timer = setInterval(() => {
        elapsedSeconds++;
        if (elapsedSeconds >= timerDuration / 1000) {
            redirectToScreen();
        }
    }, 1000);
}

function resetTimer() {
    clearInterval(timer);
    elapsedSeconds = 0;
    startTimer();
}

export function pauseTimer() {
    clearInterval(timer);
}

export function resumeTimer() {
    if (document.getElementById("timerScreen")) {
        resetTimer();
    }
}

document.addEventListener("DOMContentLoaded", function () {
    if (isTimerScreen) {
        startTimer();
        toggleEventListeners()
        resetTimer();
    }
});

const projectIdElement = document.getElementById('projectId');
const flowsoortid = document.getElementById('flowsoort');
// @ts-ignore
flowsoort = flowsoortid && flowsoortid.innerHTML === "True";

if (projectIdElement) {
    domprojectId = parseInt(projectIdElement.innerHTML);
} else {
    console.error("Project ID element not found!");
}
function toggleEventListeners() {
    if (shouldListenForActivity) {
        document.addEventListener("mousemove", resetTimer);
        document.addEventListener("keypress", resetTimer);
    } else {
        document.removeEventListener("mousemove", resetTimer);
        document.removeEventListener("keypress", resetTimer);
    }
}
