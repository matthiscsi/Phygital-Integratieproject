import Chart from 'chart.js/auto';
import { ClosedAnswerData } from "./answeranalysisdtos";

document.addEventListener('DOMContentLoaded', async function () {
    const mySpan = document.getElementById('flowStepIdSpan') as HTMLElement;
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

    createChart(flowStepId);
});

async function GetAnswersDataForClosedQuestion(id: number): Promise<ClosedAnswerData[]> {
    try {
        const response = await fetch(`/api/Responses/ClosedQuestionAnswersData/${id}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching closed question answers data:', error);
        return [];
    }
}

async function createChart(flowStepId: number) {
    const canvas = document.getElementById('myChart') as HTMLCanvasElement;
    if (!canvas) {
        console.error('Canvas element with id myChart not found');
        return;
    }

    const ctx = canvas.getContext('2d');
    if (!ctx) {
        console.error('Failed to get canvas context');
        return;
    }

    const data = await GetAnswersDataForClosedQuestion(flowStepId);
    if (data.length === 0) {
        console.error('No data available to create the chart');
        return;
    }

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map((row: ClosedAnswerData) => row.closedAnswerDto.selectedAnswerText),
            datasets: [{
                label: 'Quantity',
                data: data.map((row: ClosedAnswerData) => row.quantity),
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                    },
                }
            }
        }
    });
}