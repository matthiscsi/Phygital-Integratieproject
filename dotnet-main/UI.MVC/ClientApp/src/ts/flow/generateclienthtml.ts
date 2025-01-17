import {
    ClosedQuestionDto,
    InfoDto,
    MultipleChoiceQuestionDto,
    OpenQuestionDto, RangeQuestionDto
} from "../Configs/FlowStepConfig/flowstepDto";

export function generateInfoHtmlClient(step: InfoDto): string {
    return `
    <div class="container mt-3">
        <div class="row">
            <h5 class="text-center mb-3 text-info">Kijk op de kiosk voor de info</h5>
        </div>
    </div>`;
}


export function generateMultipleChoiceQuestionHtmlClient(step: MultipleChoiceQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<form id="multipleChoiceQuestionForm" class="mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check">
                <input class="form-check-input" type="checkbox" name="multipleChoiceQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-check-label text-info" for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateOpenQuestionHtmlClient(step: OpenQuestionDto): string {
    return `<div class="text-center my-3">
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="text" class="form-control text-info" id="openQuestion" placeholder="Enter your answer">
                </div>
            </div>`;
}

export function generateClosedQuestionHtmlClient(step: ClosedQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<form id="closedQuestionForm" class="mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check">
                <input class="form-check-input" type="radio" name="closedQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-label text-info" for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateRangeQuestionHtmlClient(step: RangeQuestionDto): string {
    return `<div class="text-center my-3">
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="range" class="form-control-range" id="rangeQuestion" min="${step.minValue}" max="${step.maxValue}" value="${step.minValue}" oninput="rangeValue.innerText = this.value">
                    <p class="text-info" id="rangeValue">0</p>
                </div>
            </div>`;
}



