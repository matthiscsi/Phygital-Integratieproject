import Chart from "chart.js/auto";
import { WordCloudController, WordElement, WordCloudChart } from 'chartjs-chart-wordcloud';
import 'chartjs-chart-wordcloud';

Chart.register(WordCloudController, WordElement, WordCloudChart);

document.addEventListener('DOMContentLoaded', async function () {
    const flowStepIdSpan = document.getElementById('flowStepIdSpan') as HTMLElement | null;
    const answersDiv = document.getElementById('answersList') as HTMLElement | null;

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

    const data = await FetchOpenAnswersForOpenQuestion(flowStepId);
    if (answersDiv) {
        data.forEach((answer: string) => {
            const listItem = document.createElement('p');
            listItem.textContent = answer;
            answersDiv.appendChild(listItem);
        });
    }
});

async function FetchAnswersDataForOpenQuestion(flowStepId: number): Promise<{ word: string; frequency: number }[]> {
    try {
        const response = await fetch(`/api/Responses/OpenQuestionAnswersData/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching flowStep analysis:', response.statusText);
            throw new Error('Failed to fetch flowStep analysis: ' + response.statusText);
        }

        const data = await response.json();
        return data;

    } catch (error) {
        console.error('Error fetching next step:', error);
        throw new Error('Failed to fetch flow analysis: ' + error);
    }
}

async function FetchOpenAnswersForOpenQuestion(flowStepId: number): Promise<string[]> {
    try {
        const response = await fetch(`/api/Responses/OpenAnswers/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching flowStep analysis:', response.statusText);
            throw new Error('Failed to fetch flowStep analysis: ' + response.statusText);
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching next step:', error);
        throw new Error('Failed to fetch flow analysis: ' + error);
    }
}

async function createChart(flowStepId: number) {
    try {
        const canvas = document.getElementById('myChart') as HTMLCanvasElement | null;
        if (!canvas) {
            console.error('Element with id myChart not found');
            return;
        }

        canvas.width = 1200;
        canvas.height = 800;
        const ctx = canvas.getContext('2d');
        if (!ctx) {
            console.error('Failed to get canvas context');
            return;
        }

        const data = await FetchAnswersDataForOpenQuestion(flowStepId);
        if (!data || data.length === 0) {
            console.error('No data returned from the API.');
            return;
        }

        const labels = data.map(item => item.word);
        const frequencies = data.map(item => 10 + item.frequency * 10);

        new Chart(ctx, {
            type: WordCloudController.id,
            data: {
                labels: labels,
                datasets: [{
                    label: 'Woordfrequenties',
                    data: frequencies,
                }]
            },
            options: {
                plugins: {
                    tooltip: {
                        enabled: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error: ' + error);
    }
}