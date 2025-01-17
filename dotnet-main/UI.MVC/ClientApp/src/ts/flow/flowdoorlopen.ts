import * as signalR from "@microsoft/signalr";
import {
    AnswerRequest,
    AnswerRequestClosedQuestion, AnswerRequestMultipleChoiceQuestion,
    AnswerRequestOpenQuestion, RangeAnswerDto
} from "../Configs/FlowStepConfig/AddAnswersInterfaces";
import { generateInfoHtml, generateMultipleChoiceQuestionHtml, generateOpenQuestionHtml, generateRangeQuestionHtml, generateClosedQuestionHtml } from "./generatehtml";
import { skipSubtheme } from "../flowstep/skiptheme";
import { pauseTimer, resumeTimer } from "./timer";

let flowid: number;
let flowSessionId: number;
export let currentStepType: string = "";

document.addEventListener('DOMContentLoaded', async function () {
    const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement | null;
    const flowidDom = document.getElementById("flowid") as HTMLSpanElement | null;

    if (flowsessionIdDom && flowidDom) {
        flowSessionId = parseInt(flowsessionIdDom.innerHTML);
        flowid = parseInt(flowidDom.innerHTML);
        await fetchCurrentStep(flowSessionId);
    }
});

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

connection.onreconnected(async (connectionId) => {
    console.log("Reconnected, attempting to rejoin group...");
    await joinGroup();
});

async function joinGroup() {
    try {
       connection.invoke("JoinGroup", flowSessionId).catch(err => console.error("Error joining group:", err.toString()));
        console.log("Joined group with session ID:", flowSessionId);
        console.error("Error joining group:");
    }
    catch (err) {
        console.error("Error joining group:", err);
    }
}


connection.on("PauseTimer", () => {
    console.log("Timer paused by host.");
    const nextButton = document.querySelector("#nextButton") as HTMLButtonElement | null;
    const skipButton = document.querySelector("#SkipButton") as HTMLButtonElement | null;
    if (nextButton) nextButton.disabled = true;
    if (skipButton) skipButton.disabled = true;
    document.body.style.backgroundColor = "rgba(0,0,0,0.5)";
    pauseTimer();
});

connection.on("ResumeTimer", () => {
    console.log("Timer resumed by host.");
    const nextButton = document.querySelector("#nextButton") as HTMLButtonElement | null;
    const skipButton = document.querySelector("#SkipButton") as HTMLButtonElement | null;
    if (nextButton) nextButton.disabled = false;
    if (skipButton) skipButton.disabled = false;
    document.body.style.backgroundColor = "";
    resumeTimer();
});

connection.on("RemoveQrCode", () => {
    console.log("Received remove QR code");
    removeQrCode();
});

connection.on("ShowQrCode", () => {
    console.log("Received show QR code");
    showQrCode();
});

function removeQrCode() {
    const begeleiderDiv = document.getElementById("BegeleiderDiv");
    if (begeleiderDiv) {
        begeleiderDiv.style.display = "none";
    }
}

function showQrCode() {
    const begeleiderDiv = document.getElementById("BegeleiderDiv");
    if (begeleiderDiv) {
        begeleiderDiv.style.display = "block";
    }
}

const stapje = document.getElementById('stapje') as HTMLDivElement | null;

document.addEventListener('DOMContentLoaded', async function () {
    const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement | null;
    const flowidDom = document.getElementById("flowid") as HTMLSpanElement | null;

    if (flowsessionIdDom && flowidDom) {
        flowSessionId = parseInt(flowsessionIdDom.innerHTML);
        flowid = parseInt(flowidDom.innerHTML);
        await fetchCurrentStep(flowSessionId);
    }
});

async function fetchCurrentStep(flowSessionId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/current/${flowSessionId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            const stepData = await response.json();
            identifyStepType(stepData);
        } else if (response.status === 404) {
            window.location.href = `/Flow/ThankYou/${flowid}`;
        } else {
            console.error('Failed to fetch the current step');
        }
    } catch (error) {
        console.error('Error fetching current step:', (error as Error).message);
    }
}

async function handleNextButtonClick() {
    const definedAnswer = await defineAnswerRequest(currentStepType);
    if (definedAnswer && currentStepType !== "Info") {
        await postAnswerRequest(definedAnswer);
    } else {
        console.log("No option selected or open question answer is empty.");
    }
}

const nextButton = document.querySelector("#nextButton") as HTMLButtonElement | null;
if (nextButton) {
    nextButton.addEventListener('click', handleNextButtonClick);
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
            identifyStepType(stepData);
        } else if (response.status === 404) {
            window.location.href = `/Flow/ThankYou/${flowid}`;
        } else {
            console.error('Failed to fetch the next step');
        }
    } catch (error) {
        console.error('Error fetching next step:', (error as Error).message);
    }
}

async function defineAnswerRequest(currentStepType: string): Promise<AnswerRequest | AnswerRequestOpenQuestion | AnswerRequestClosedQuestion | AnswerRequestMultipleChoiceQuestion | RangeAnswerDto | undefined> {
    let answerRequest: AnswerRequest | AnswerRequestOpenQuestion | AnswerRequestClosedQuestion | AnswerRequestMultipleChoiceQuestion | RangeAnswerDto | undefined;

    if (currentStepType === "Info") {
        console.log("Info step, no answer required.");
        await getNextStep(flowSessionId);
    } else {
        const closedQuestionRadio = document.querySelector('#closedQuestionForm input[name="closedQuestion"]:checked') as HTMLInputElement | null;
        const openQuestionInput = document.querySelector('#openQuestion') as HTMLInputElement | null;
        const multipleChoiceCheckboxes = document.querySelectorAll('#multipleChoiceQuestionForm input[name="multipleChoiceQuestion"]:checked') as NodeListOf<HTMLInputElement>;
        const rangeQuestionInput = document.querySelector('#rangeQuestion') as HTMLInputElement | null;

        if (closedQuestionRadio) {
            answerRequest = {
                flowSessionId: flowSessionId,
                SelectedAnswer: parseInt(closedQuestionRadio.value, 10),
            };
        } else if (openQuestionInput && openQuestionInput.value.trim() !== '') {
            answerRequest = {
                flowSessionId: flowSessionId,
                answer: openQuestionInput.value.trim(),
            };
        } else if (multipleChoiceCheckboxes.length > 0) {
            const selectedOptions = Array.from(multipleChoiceCheckboxes).map(checkbox => parseInt(checkbox.value, 10));
            answerRequest = {
                flowSessionId: flowSessionId,
                SelectedAnswers: selectedOptions,
            };
        } else if (rangeQuestionInput) {
            answerRequest = {
                flowSessionId: flowSessionId,
                SelectedValue: parseInt(rangeQuestionInput.value, 10),
            };
        }
    }
    return answerRequest;
}

async function postAnswerRequest(answerRequest: AnswerRequest) {
    try {
        const response = await fetch(`/api/Responses/addanswer`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(answerRequest)
        });

        if (!response.ok) {
            throw new Error('Failed to add answer');
        }

        await getNextStep(flowSessionId);
        return response.json();
    } catch (error) {
        console.error('Error:', (error as Error).message);
        throw error;
    }
}

function identifyStepType(step: any): string {
    const skipButton = document.querySelector("#SkipButton") as HTMLButtonElement | null;
    const stapje = document.getElementById('stapje') as HTMLDivElement | null;
    if (!stapje || !skipButton) return "";

    switch (step.flowStepType) {
        case "Info":
            currentStepType = "Info";
            stapje.innerHTML = generateInfoHtml(step);
            break;
        case "ClosedQuestion":
            currentStepType = "ClosedQuestion";
            stapje.innerHTML = generateClosedQuestionHtml(step);
            break;
        case "OpenQuestion":
            currentStepType = "OpenQuestion";
            stapje.innerHTML = generateOpenQuestionHtml(step);
            break;
        case "MultipleChoiceQuestion":
            currentStepType = "MultipleChoiceQuestion";
            stapje.innerHTML = generateMultipleChoiceQuestionHtml(step);
            break;
        case "RangeQuestion":
            currentStepType = "RangeQuestion";
            stapje.innerHTML = generateRangeQuestionHtml(step);
            break;
        default:
            console.log("No step type found.");
            break;
    }
    return currentStepType;
}

const skipButton = document.getElementById('SkipButton') as HTMLButtonElement | null;
if (skipButton) {
    skipButton.addEventListener('click', () => {
        skipSubtheme(flowSessionId).then(() => console.log("Subtheme skipped successfully"));
    });
}
