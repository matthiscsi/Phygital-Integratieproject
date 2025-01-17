import Chart from 'chart.js/auto';
import { MultipleChoiceAnswerData, MultipleChoiceAnswerDto } from "./answeranalysisdtos";

document.addEventListener('DOMContentLoaded', async function () {
    const flowStepIdSpan = document.getElementById('flowStepIdSpan') as HTMLElement;

    if (!flowStepIdSpan) {
        console.error('Element with id flowStepIdSpan not found');
        return;
    }

    const flowStepIdString = flowStepIdSpan.dataset.flowstepId;
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

async function GetAnswersDataForMultipleChoiceQuestion(flowStepId: number): Promise<MultipleChoiceAnswerData[]> {
    try {
        const response = await fetch(`/api/Responses/MultipleChoiceQuestionAnswersData/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching flowStep analysis:', response.statusText);
            throw new Error('Failed to fetch flowStep analysis: ' + response.statusText);
        }

        const data = await response.json() as MultipleChoiceAnswerData[];
        return data.map((elem: MultipleChoiceAnswerData) => ({
            multipleChoiceAnswerDto: elem.multipleChoiceAnswerDto as MultipleChoiceAnswerDto,
            quantity: elem.quantity
        }));

    } catch (error) {
        console.error('Error fetching next step:', error);
        throw new Error('Failed to fetch flow analysis: ' + error);
    }
}

async function createChart(flowStepId: number) {
    const canvas = document.getElementById('myChart') as HTMLCanvasElement;
    if (!canvas) {
        console.error('Element with id myChart not found');
        return;
    }

    const ctx = canvas.getContext('2d');
    if (!ctx) {
        console.error('Failed to get canvas context');
        return;
    }

    const data = await GetAnswersDataForMultipleChoiceQuestion(flowStepId);

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.map((row: MultipleChoiceAnswerData) => row.multipleChoiceAnswerDto.selectedAnswerText),
            datasets: [{
                label: 'Quantity',
                data: data.map((row: MultipleChoiceAnswerData) => row.quantity),
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