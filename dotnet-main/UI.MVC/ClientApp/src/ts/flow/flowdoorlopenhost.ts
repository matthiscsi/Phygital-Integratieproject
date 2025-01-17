import * as signalR from "@microsoft/signalr";
import {
    generateInfoHtmlHost,
    generateMultipleChoiceQuestionHtmlHost,
    generateOpenQuestionHtmlHost,
    generateRangeQuestionHtmlHost,
    generateClosedQuestionHtmlHost
} from "./generatehosthtml";


document.addEventListener('DOMContentLoaded', async function () {

    flowSessionId = parseInt(flowsessionIdDom.innerHTML);
    flowid = parseInt(flowidDom.innerHTML);

});


const stapje = document.getElementById('stapje') as HTMLDivElement;
const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement;
const flowidDom = document.getElementById("flowid") as HTMLSpanElement;
const skipButton = document.getElementById('skipButton') as HTMLButtonElement;
const sendButton = document.getElementById('sendStep') as HTMLButtonElement;
const nextButton = document.getElementById('nextButton') as HTMLButtonElement;
export let flowid: number;
export let flowSessionId: number;
let isHandlingNextStep = false;


let connection = new signalR.HubConnectionBuilder()
    .withUrl("/flowStepHub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect([0, 2000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.keepAliveIntervalInMilliseconds = 10000; // 30 seconds
connection.serverTimeoutInMilliseconds = 15000; // 60 seconds

connection.start().then(() => {
    joinGroup().then(r => console.log("Group joined successfully"));
}).catch(err => console.error("Error establishing connection:", err));

window.addEventListener('beforeunload', async function () {
    console.log("Closing connection");
    connection.invoke("SendThankYou", flowSessionId).catch(err => console.error("Sending thankyou:", err.toString()));
});
connection.onreconnected(async (connectionId) => {
    console.log("Reconnected, attempting to rejoin group...");
    await joinGroup();
});

async function joinGroup() {
    try {
        connection.invoke("SetHost", flowSessionId).catch(err => console.error("Error setting host:", err));
        connection.invoke("JoinGroup", flowSessionId).catch(err => console.error("Error joining group:", err.toString()));
        console.log("Joined group with session ID:", flowSessionId);
    } catch (err) {
        console.error("Error joining group:", err);
    }
}



// @ts-ignore
nextButton.addEventListener('click', async () => {
    if (!isHandlingNextStep) {
        isHandlingNextStep = true;
        await getNextStep(flowSessionId);
        isHandlingNextStep = false;
    }
});

// @ts-ignore
sendButton.addEventListener('click', async () => {
    console.log("Sending current step to clients");
    if (currentStepType) { 
        await sendCurrentStepToClients(flowSessionId);
    }
});


connection.on("MoveToNextStep", async (sessionId) => {
    console.log("Received MoveToNextStep for session:", sessionId);
    if (sessionId === flowSessionId.toString()) {
        await handleNextButtonClick();
    }
});

connection.on("EnableNextStep", async () => {
    console.log("All clients are ready. Enabling move to next step.");
    const info = document.getElementById('info');
    const buttonsHost = document.getElementById('buttonsHost') as HTMLDivElement;

    if (info) info.style.display = 'none';
    if (buttonsHost) buttonsHost.style.display = 'block'; // Maak de buttonsHost div zichtbaar

    fetchCurrentStep(flowSessionId).then(r => console.log("Current step fetched"));


    await sendCurrentStepToClients(flowSessionId);
});
connection.on("AllClientsReady", () => {
    console.log("All clients are ready. Host can now move to the next step.");
    nextButton.disabled = false;
    sendButton.disabled = false;
    skipButton.disabled = false;
});

// @ts-ignore
skipButton.addEventListener('click', async () => {
    await skipSubtheme(flowSessionId);
});

async function sendCurrentStepToClients(flowSessionId: number) {
    await connection.invoke("SendCurrentStep", flowSessionId).catch(err => console.error("Error sending current step to clients:", err));
    skipButton.disabled = true;
    sendButton.disabled = true;
    nextButton.disabled = true;
}
export async function fetchCurrentStep(flowSessionId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/current/${flowSessionId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            const stepData = await response.json();
            identifyStepType(stepData); // Implement this function based on how you want to display the step.
        } else if (response.status === 404) {
            window.location.href = `/Flow/ThankYou/${flowid}`;
        } else {
            console.error('Failed to fetch the current step');
        }
    } catch (error) {
        console.error('Error fetching current step:', error);
    }
}

async function handleNextButtonClick() {
    await getNextStep(flowSessionId);
}

export async function getNextStep(flowSessionId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/next/${flowSessionId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            const stepData = await response.json();
            identifyStepType(stepData); // Refresh or update the UI with the new step data
        } else if (response.status === 404) {
            window.location.href = `/Flow/ThankYou/${flowid}`;
        } else {
            console.error('Failed to fetch the next step');
        }
    } catch (error) {
        console.error('Error fetching next step:', error);
    }
}

let currentStepType = "";

function identifyStepType(step: any): string {
    switch (step.flowStepType) {
        case "Info":
            // @ts-ignore
            skipButton.style.display = '';
            currentStepType = "Info";
            stapje.innerHTML = generateInfoHtmlHost(step);
            break;
        case "ClosedQuestion":
            // @ts-ignore
            skipButton.style.display = '';
            currentStepType = "ClosedQuestion";
            stapje.innerHTML = generateClosedQuestionHtmlHost(step);
            break;
        case "OpenQuestion":
            // @ts-ignore
            skipButton.style.display = '';
            currentStepType = "OpenQuestion";
            stapje.innerHTML = generateOpenQuestionHtmlHost(step);
            break;
        case "MultipleChoiceQuestion":
            // @ts-ignore
            skipButton.style.display = '';
            currentStepType = "MultipleChoiceQuestion";
            stapje.innerHTML = generateMultipleChoiceQuestionHtmlHost(step);
            break;
        case "RangeQuestion":
            // @ts-ignore
            skipButton.style.display = '';
            currentStepType = "RangeQuestion";
            stapje.innerHTML = generateRangeQuestionHtmlHost(step);
            break;
        default:
            console.log("No step type found.");
            break;
    }
    return currentStepType;
}


if (skipButton) {
    skipButton.addEventListener('click', () => {
        skipSubtheme(flowSessionId).then(r => console.log("Subtheme skipped successfully"));
    });
}
export async function skipSubtheme(flowSessionId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/skipsubtheme/${flowSessionId}`, {
            method: 'GET',  // Consider changing this to POST or PUT if you are modifying data
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            console.log("Subtheme skipped successfully");
            await getNextStep(flowSessionId);  // Refresh the current step if needed
        }
        else {
            console.error("Failed to skip the subtheme");
        }
    } catch (error) {
        console.error("Could not get the next step", error);
    }
}