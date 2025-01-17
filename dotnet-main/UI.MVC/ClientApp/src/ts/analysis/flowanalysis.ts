import Chart from 'chart.js/auto';
import { SubthemeDto } from '../Configs/AddFlowConfig/subthemeDto';
import { SkippedSubthemeDto } from '../skippedSubthemeDto';
import { FlowStepDto } from '../Configs/FlowStepConfig/flowstepDto';

const mySpan = document.getElementById('flowIdSpan');
const flowSessionQuantityText = document.getElementById('flowSessionQuantity') as HTMLElement;
const flowSessionAverageText = document.getElementById('averageTimeSpentForFlow') as HTMLElement;

document.addEventListener('DOMContentLoaded', async () => {
    const flowId = parseInt(mySpan?.dataset.flowId || '0', 10);
    if (!flowId) {
        console.error('Flow ID not found.');
        return;
    }
    updateChart(flowId);
    fetchFlowstepsOfFlow(flowId.toString());
    fetchFlowSessionCount(flowId.toString());
    fetchAverageTimeSpentForFlow(flowId.toString());
});

async function updateChart(flowId: number) {
    try {
        const ctx = (document.getElementById('myChart') as HTMLCanvasElement | null)?.getContext('2d');
        if (!ctx) {
            console.error('Chart context not found');
            return;
        }
        
        const skippedSubthemeDtos = await getSubthemeSkippedCount(flowId);
        if (!skippedSubthemeDtos || !ctx) {
            console.error('Skipped subtheme data or context not found.');
            return;
        }
        const subthemeNames = skippedSubthemeDtos.map(subtheme => subtheme.subthemeDto.title);
        const sessionCounts = skippedSubthemeDtos.map(subtheme => subtheme.quantity);

        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: subthemeNames,
                datasets: [{
                    label: 'Keer overgeslaan',
                    data: sessionCounts,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                        'rgba(255, 159, 64, 0.2)',
                        'rgba(94, 183, 183, 0.2)'
                    ],
                    borderColor: [
                        'rgba(255, 99, 132, 1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)',
                        'rgba(94, 183, 183, 1)'
                    ],
                    borderWidth: 1,
                }],
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Subthemas overgeslaan',
                    },
                },
            },
        });
    } catch (error) {
        console.error('Error updating chart:', error);
    }
}

async function getSubthemeSkippedCount(flowId: number): Promise<SkippedSubthemeDto[]> {
    try {
        const response = await fetch(`/api/FlowSessions/subthemeSkippedCount/${flowId}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch subtheme skipped count: ${response.statusText}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching subtheme skipped count:', error);
        throw error;
    }
}

async function fetchFlowstepsOfFlow(flowId: string) {
    try {
        const response = await fetch(`/api/Flowsteps/${flowId}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch flow steps: ${response.statusText}`);
        }
        const flowsteps: FlowStepDto[] = await response.json();
        showFlowStepsInList(flowsteps);
    } catch (error) {
        console.error('Error fetching flow steps:', error);
        alert(error);
    }
}

async function fetchFlowSessionCount(flowId: string) {
    try {
        const response = await fetch(`/api/FlowSessions/flowSessionCount/${flowId}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch flow session count: ${response.statusText}`);
        }
        const flowSessionCount = await response.json();
        flowSessionQuantityText.innerText = flowSessionCount;
    } catch (error) {
        console.error('Error fetching flow session count:', error);
        alert(error);
    }
}

async function fetchAverageTimeSpentForFlow(flowId: string) {
    try {
        const response = await fetch(`/api/FlowSessions/averageTimeSpent/${flowId}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch average time spent: ${response.statusText}`);
        }
        const averageTimeSpent = await response.json();
        flowSessionAverageText.innerText = averageTimeSpent;
    } catch (error) {
        console.error('Error fetching average time spent:', error);
        alert(error);
    }
}

function showFlowStepsInList(flowsteps: FlowStepDto[]) {
    const flowStepList = document.getElementById('flowStepList') as HTMLUListElement;
    flowsteps.sort((a, b) => a.orderNr - b.orderNr);
    flowStepList.innerHTML = '';
    flowsteps.forEach(flowstep => {
        const li = document.createElement('li');
        li.className = 'list-group-item d-flex justify-content-between align-items-center';
        li.textContent = `${flowstep.flowStepType} - ${flowstep.subtheme}`;

        if (flowstep.flowStepType !== 'Info') {
            const button = document.createElement('button');
            button.className = 'btn btn-primary';
            button.textContent = 'Analyse';
            const url = `/Statistics/FlowStepAnalysis?flowStepId=${flowstep.flowStepId}`;
            button.onclick = () => {
                window.location.href = url;
            };

            li.appendChild(button);
        }
        flowStepList.appendChild(li);
    });
}
