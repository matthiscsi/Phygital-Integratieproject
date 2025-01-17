import { counter } from "./flow";

export let flowid: number;
export let flowSessionId: number;
export let ordernr = 0;
export let subthemeid: number;
let subthemetitle: string;
let query: string;
let currentStepType: string = "";

window.onload = function () {
    const currentUrl = window.location.href;
    const urlSegments = currentUrl.split('/');
};

const startButtons = document.getElementsByClassName('btn btn-primary mt-auto startButton') as HTMLCollectionOf<HTMLButtonElement>;
console.log(startButtons);
for (let i = 0; i < startButtons.length; i++) {
    startButtons[i].addEventListener('click', function (event) {
        const button = event.target as HTMLButtonElement;
        const flowid = Number(button.getAttribute('data-flowid'));
        console.log("Start button clicked.");
        startSession(flowid, counter);
    });
}

export async function startSession(flowId: number, expectedUser: number): Promise<void> {
    try {
        const response = await fetch(`/api/FlowSessions/startFlowsession/${flowId}?expectedUser=${expectedUser}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error(`Failed to start the flow session with status: ${response.status}`);
        }

        const flowSession = await response.json();
        if (expectedUser === 1) {
            window.location.href = `/FlowStep/Index/${flowSession.flowSessionId}`;
        } else {
            window.location.href = `/FlowStep/WaitingRoom/${flowSession.flowSessionId}`;
        }
    } catch (error) {
        console.error('Could not start the flow session:', error);
    }
}
