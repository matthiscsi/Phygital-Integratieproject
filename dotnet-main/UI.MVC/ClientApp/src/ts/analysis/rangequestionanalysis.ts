import Chart from 'chart.js/auto';
import { RangeAnswerData, RangeAnswerDto } from "./answeranalysisdtos";

const flowStepIdSpan = document.getElementById('flowStepIdSpan') as HTMLElement | null;
let flowStepId: number;

if (flowStepIdSpan && flowStepIdSpan.dataset.flowstepId) {
    flowStepId = parseInt(flowStepIdSpan.dataset.flowstepId);
} else {
    console.error('FlowStepIdSpan element or dataset.flowstepId not found');
    throw new Error('Required element or data attribute not found');
}

document.addEventListener('DOMContentLoaded', async function () {
    createChart();
});

async function GetAnswersDataForRangeQuestion(flowStepId: number): Promise<RangeAnswerData[]> {
    try {
        const response = await fetch(`/api/Responses/RangeQuestionAnswersData/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            console.error('Error fetching flowStep analysis:', response.statusText);
            throw new Error('Failed to fetch flowStep analysis: ' + response.statusText);
        }

        const data = await response.json() as RangeAnswerData[];
        return data.map((elem: RangeAnswerData) => ({
            rangeAnswerDto: elem.rangeAnswerDto as RangeAnswerDto,
            quantity: elem.quantity
        }));
    } catch (error) {
        console.error('Error fetching next step:', error);
        throw new Error('Failed to fetch flow analysis: ' + error);
    }
}

async function createChart() {

    const ctx = (document.getElementById('myChart') as HTMLCanvasElement | null)?.getContext('2d');
    if (!ctx) {
        console.error('Chart context not found');
        return;
    }

    const data = await GetAnswersDataForRangeQuestion(flowStepId);

    let minValue = 0;
    let maxValue = 0;

    if (flowStepIdSpan) {
        const minValueString = flowStepIdSpan.dataset.minValue;
        const maxValueString = flowStepIdSpan.dataset.maxValue;

        if (minValueString && maxValueString) {
            minValue = parseInt(minValueString);
            maxValue = parseInt(maxValueString);
        } else {
            console.error('MinValue or MaxValue not found in dataset');
        }
    }

    const quantities = Array(maxValue - minValue + 1).fill(0);

    data.forEach(row => {
        quantities[row.rangeAnswerDto.selectedAnswer - minValue] += row.quantity;
    });

    let sum = 0;
    let totalQuantity = 0;
    data.forEach(item => {
        sum += item.rangeAnswerDto.selectedAnswer * item.quantity;
        totalQuantity += item.quantity;
    });
    const average = sum / totalQuantity;

    const labels = [];
    for (let i = minValue; i <= maxValue; i++) {
        labels.push(i);
    }

    const averageLinePlugin = {
        id: 'averageLinePlugin',
        afterDraw: (chart: Chart) => {
            const ctx = chart.ctx;
            const yAxis = chart.scales.y;
            const xAxis = chart.scales.x;

            const averageX = xAxis.getPixelForValue(average - minValue);

            ctx.save();
            ctx.beginPath();
            ctx.moveTo(averageX, yAxis.top);
            ctx.lineTo(averageX, yAxis.bottom);
            ctx.strokeStyle = 'rgba(255, 99, 132, 1)';
            ctx.lineWidth = 2;
            ctx.stroke();
            ctx.restore();

            ctx.font = '12px Arial';
            ctx.fillStyle = 'rgba(255, 99, 132, 1)';
            ctx.textAlign = 'center';
            ctx.fillText('Average: ' + average.toFixed(2), averageX, yAxis.top - 10);
        }
    };

    Chart.register(averageLinePlugin);

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Answers',
                data: quantities,
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                x: {
                    min: minValue,
                    max: maxValue,
                    ticks: {
                        stepSize: 1,
                    },
                },
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