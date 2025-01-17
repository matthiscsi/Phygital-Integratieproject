import * as signalR from "@microsoft/signalr";
import {
    AnswerRequest,
    AnswerRequestClosedQuestion, AnswerRequestMultipleChoiceQuestion,
    AnswerRequestOpenQuestion, RangeAnswerDto
} from "../Configs/FlowStepConfig/AddAnswersInterfaces";
import {
    generateInfoHtmlClient,
    generateMultipleChoiceQuestionHtmlClient,
    generateOpenQuestionHtmlClient,
    generateRangeQuestionHtmlClient,
    generateClosedQuestionHtmlClient
} from "./generateclienthtml";

const stapje = document.getElementById('stapje') as HTMLDivElement;
const flowsessionIdDom = document.getElementById("flowsessionid") as HTMLSpanElement;
const flowidDom = document.getElementById("flowid") as HTMLSpanElement;
const readyButton = document.getElementById('readybutton') as HTMLButtonElement;
const readyText = document.getElementById('waitingText') as HTMLParagraphElement;
const spinner = document.querySelector('.spinner-border') as HTMLDivElement;
export let flowid: number;
export let flowSessionId: number;
let isHandlingNextStep = false;
document.addEventListener('DOMContentLoaded', async function () {

    flowSessionId = parseInt(flowsessionIdDom.innerHTML);
    flowid = parseInt(flowidDom.innerHTML);

})

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
        await connection.invoke("JoinGroup", flowSessionId);
        console.log("Joined group with session ID:", flowSessionId);
    } catch (err) {
        console.error("Error joining group:", err);
    }
}
connection.on("SendThankYou", async (message) => {
    if (message === flowSessionId.toString()) {
        identifyStepType({flowStepType: "ThankYou"});
    }
});


// @ts-ignore
// @ts-ignore
readyButton.addEventListener('click', () => {
    connection.invoke("ReadyToBegin", flowSessionId)
        .then(() => {
            fetchCurrentStep(flowSessionId); // Fetch the current step right after signaling readiness
            readyText.innerText = "Wacht tot de begeleider de eerste vraag toont"; // Initial waiting message
            readyText.style.display = 'block';
        })
        .catch(err => console.error("Error on ready to begin:", err));
});

connection.on("ShowCurrentStep", async (message) => {
    console.log("Received ShowCurrentStep:", message);
    if (message === flowSessionId.toString()) {
        await fetchCurrentStep(flowSessionId);
        readyText.style.display = 'none';
        readyButton.style.display = 'none';
        console.log(nextButton)
        // @ts-ignore
        nextButton.style.display = 'block';
        const titleElement = document.querySelector('h2.text-center.my-4');
        // @ts-ignore
        titleElement.style.display = 'block';

        const buttonDiv = document.getElementById('buttonDiv') as HTMLDivElement;
        const stapje = document.getElementById('stapje') as HTMLDivElement;

        buttonDiv.style.display = 'flex';
        stapje.style.display = 'block';
        spinner.style.display = 'none';

    }
});
let nextButton = document.querySelector("#nextButton");
// @ts-ignore
nextButton.addEventListener('click', async () => {
    readyText.style.display = 'block';
    handleNextButtonClick().then(() => {
        console.log("Next button clicked.")
    });
    // @ts-ignore
    nextButton.style.display = 'none';
    readyText.innerText = "Wacht tot de begeleider de volgende vraag toont";

    spinner.style.display = 'block';
    connection.invoke("ClientReady", flowSessionId).catch(err => console.error("Error signaling readiness:", err));
});

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
    if (!isHandlingNextStep) {
        isHandlingNextStep = true;
        let definedanswer = await defineAnswerRequest(currentStepType);

        if (definedanswer && currentStepType !== "Info") {
            const submissionSuccess = await postAnswerRequest(definedanswer);
            if (submissionSuccess) {
                // Notify the server that this client is ready to move to the next step
                readyText.innerText = "Wachten tot iedereen klaar is"; // Update the text to indicate waiting status
                readyText.style.display = 'block';
                // @ts-ignore
                nextButton.style.display = 'none';
                stapje.style.display = 'none';
            }
        } else {
            console.log("No option selected or open question answer is empty.");
        }

        isHandlingNextStep = false;
    }
}

let currentStepType = "";

async function defineAnswerRequest(currentStepType: string): Promise<AnswerRequest | AnswerRequestOpenQuestion | AnswerRequestClosedQuestion | AnswerRequestMultipleChoiceQuestion | RangeAnswerDto | undefined> {
    let answerRequest: AnswerRequest | AnswerRequestOpenQuestion | AnswerRequestClosedQuestion | AnswerRequestMultipleChoiceQuestion | RangeAnswerDto | undefined;

    if (currentStepType === "Info") {
        console.log("Info step, no answer required.");
    } else {

        const closedQuestionRadio = document.querySelector('#closedQuestionForm input[name="closedQuestion"]:checked') as HTMLInputElement;
        const openQuestionInput = document.querySelector('#openQuestion') as HTMLInputElement;
        const multipleChoiceCheckboxes = document.querySelectorAll('#multipleChoiceQuestionForm input[name="multipleChoiceQuestion"]:checked');
        const rangeQuestionInput = document.querySelector('#rangeQuestion') as HTMLInputElement;

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
            const selectedOptions = Array.from(multipleChoiceCheckboxes).map(checkbox => parseInt((checkbox as HTMLInputElement).value, 10));
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
        return answerRequest

    }
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
        return response.json();
    } catch (error) {
        // @ts-ignore
        console.error('Error:', error.message);
        throw error;
    }
}


function identifyStepType(step: any): string {
    switch (step.flowStepType) {
        case "Info":
            currentStepType = "Info";
            stapje.innerHTML = generateInfoHtmlClient(step);

            break;
        case "ClosedQuestion":
            currentStepType = "ClosedQuestion";
            stapje.innerHTML = generateClosedQuestionHtmlClient(step);
            break;
        case "OpenQuestion":
            currentStepType = "OpenQuestion";
            stapje.innerHTML = generateOpenQuestionHtmlClient(step);
            break;
        case "MultipleChoiceQuestion":
            currentStepType = "MultipleChoiceQuestion";
            stapje.innerHTML = generateMultipleChoiceQuestionHtmlClient(step);
            break;
        case "RangeQuestion":
            currentStepType = "RangeQuestion";
            stapje.innerHTML = generateRangeQuestionHtmlClient(step);
            break;
        case "ThankYou":
            window.location.href = `/Flow/ThankYou/${flowid}`;
            break;
        default:
            console.log("No step type found.");
            break;
    }
    return currentStepType;
}

