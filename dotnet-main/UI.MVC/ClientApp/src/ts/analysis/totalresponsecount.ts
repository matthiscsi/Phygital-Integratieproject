document.addEventListener('DOMContentLoaded', async function () {
    const mySpan = document.getElementById('flowStepIdSpan') as HTMLElement;
    const totalResponseCountText = document.getElementById('totalResponse') as HTMLElement;

    if (!mySpan) {
        console.error('Element with id flowStepIdSpan not found');
        return;
    }

    const flowStepIdString = mySpan.dataset.flowstepId;
    if (!flowStepIdString) {
        console.error('flowstepId not found in dataset');
        return;
    }

    const flowStepId = parseInt(flowStepIdString);
    if (isNaN(flowStepId)) {
        console.error('Invalid flowStepId');
        return;
    }

    fetchTotalResponseCount(flowStepId, totalResponseCountText);
});

async function fetchTotalResponseCount(flowStepId: number, totalResponseCountText: HTMLElement) {
    try {
        const response = await fetch(`/api/Responses/TotalResponseCount/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const averageTime = await response.json();
        totalResponseCountText.innerText = averageTime.toString();
    } catch (error) {
        console.error('Error fetching total response count:', error);
        alert(error);
    }
}