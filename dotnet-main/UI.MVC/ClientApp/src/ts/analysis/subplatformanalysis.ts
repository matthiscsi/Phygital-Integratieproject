const flowSessionQuantityForSubplatformText = document.getElementById('flowSessionCountTextSubplatform') as HTMLElement | null;
const averageTimeSpentForSubplatformText = document.getElementById('averageTimeSpentTextSubplatform') as HTMLElement | null;
const subplatformIdSpan = document.getElementById('subplatformIdSpan');

if (flowSessionQuantityForSubplatformText && averageTimeSpentForSubplatformText && subplatformIdSpan && subplatformIdSpan.dataset.subplatformId) {
    const subplatformId = parseInt(subplatformIdSpan.dataset.subplatformId);
    document.addEventListener('DOMContentLoaded', async function () {
        fetchFlowSessionCountOfSubplatform(subplatformId);
        fetchAverageTimeForFlowsOfSubplatform(subplatformId);
    });
} else {
    console.error('Required elements or dataset attributes not found');
}

async function fetchFlowSessionCountOfSubplatform(subplatformId: number) {
    try {
        const response = await fetch(`/api/Subplatforms/flowSessionCount/${subplatformId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        const flowSessionCount = await response.json();
        if (flowSessionQuantityForSubplatformText) {
            flowSessionQuantityForSubplatformText.innerText = flowSessionCount;
        }
    } catch (error) {
        console.error('Error fetching flow session count:', error);
        alert(error);
    }
}

async function fetchAverageTimeForFlowsOfSubplatform(subplatformId: number) {
    try {
        const response = await fetch(`/api/Subplatforms/averageTime/${subplatformId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        const averageTime = await response.json();
        if (averageTimeSpentForSubplatformText) {
            averageTimeSpentForSubplatformText.innerText = averageTime;
        }
    } catch (error) {
        console.error('Error fetching average time:', error);
        alert(error);
    }
}