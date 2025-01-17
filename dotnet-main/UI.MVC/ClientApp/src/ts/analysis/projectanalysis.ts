let flowSessionQuantityText = document.getElementById('flowSessionQuantityOfProject') as HTMLElement | null;
const projectIdSpan = document.getElementById('projectIdSpan') as HTMLElement | null;
let averageTimeSpentForProjectText = document.getElementById('averageTimeSpentForProject') as HTMLElement | null;

document.addEventListener('DOMContentLoaded', async function () {
    if (!projectIdSpan) {
        console.error('Element with id projectIdSpan not found');
        return;
    }

    const projectIdString = projectIdSpan.dataset.projectId;
    if (!projectIdString) {
        console.error('projectId not found in dataset');
        return;
    }

    const projectIdNr = parseInt(projectIdString);
    if (isNaN(projectIdNr)) {
        console.error('Invalid projectId');
        return;
    }

    fetchFlowSessionCount(projectIdNr);
    fetchAverageTimeForFlowsOfProject(projectIdNr);
});

async function fetchFlowSessionCount(projectId: number) {
    console.log("fetchFlowSessionCount called");
    try {
        const response = await fetch(`/api/Projects/flowSessionCount/${projectId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching flow session count:', response.statusText);
            throw new Error('Failed to fetch flow session count: ' + response.statusText);
        }

        const flowSessionCount = await response.json();
        console.log(flowSessionCount);

        if (flowSessionQuantityText) {
            flowSessionQuantityText.innerText = flowSessionCount.toString();
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error fetching flow session count: ' + error);
    }
}

async function fetchAverageTimeForFlowsOfProject(projectId: number) {
    console.log("fetchAverageTime called");
    try {
        const response = await fetch(`/api/Projects/averageTime/${projectId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching average time:', response.statusText);
            throw new Error('Failed to fetch average time: ' + response.statusText);
        }

        const averageTime = await response.json();
        console.log(averageTime);

        if (averageTimeSpentForProjectText) {
            averageTimeSpentForProjectText.innerText = averageTime.toString();
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error fetching average time: ' + error);
    }
}