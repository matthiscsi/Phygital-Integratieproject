import {
    ClosedQuestionDto,
    InfoDto,
    MultipleChoiceQuestionDto,
    OpenQuestionDto,
    RangeQuestionDto
} from "../Configs/FlowStepConfig/flowstepDto";


export function generateInfoHtml(step: InfoDto): string {
    let html = `
    <div class="container mt-3">
        <div class="row">
        <h5 class="text-primary text-center mb-3">Het huidige subthema: ${step.subtheme}</h5>
            <div class="col-12">      
                    <div class="alert alert-info" role="alert">
                    <h4 class="alert-heading">${step.body}</h4>
                </div>
            </div>
        </div>
        <div class="row">`;

    if (step.urlVideo) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-2">
                <video controls class="w-100 mb-2">
                    <source src="${step.urlVideo}" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
            </div>`;
    }

    if (step.urlImage) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-3">
                <img src="${step.urlImage}" class="img-fluid rounded mb-2" alt="Subthema image">
            </div>`;
    }

    if (step.urlAudio) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-4">
                <audio controls class="w-100 mb-2">
                    <source src="${step.urlAudio}" type="audio/mp3">
                    Your browser does not support the audio element.
                </audio>
            </div>`;
    }

    html += `
        </div>
    </div>`;

    return html;
}


export function generateMultipleChoiceQuestionHtml(step: MultipleChoiceQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h5 class="text-primary mb-3">Het huidige subthema: ${step.subtheme}</h5>
                        <h4 class="text-body">${step.query}</h4>
                    </div>
                    <form id="multipleChoiceQuestionForm" class="multiple-choice-options mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check ">
                <input class="form-check-input multiple-choice-option" type="checkbox" name="multipleChoiceQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-check-label text-body " for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateOpenQuestionHtml(step: OpenQuestionDto): string {
    return `<div class="text-center my-3">
                <h5 class="text-primary mb-3">het huidige subthema: ${step.subtheme}</h5>
                <h4 class="text-body">${step.query}</h4>
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="text" class="form-control" id="openQuestion" placeholder="Enter your answer">
                </div>
            </div>`;
}

export function generateClosedQuestionHtml(step: ClosedQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h5 class="text-primary mb-3">Het huidige subthema: ${step.subtheme}</h5>
                        <h4 class="text-body">${step.query}</h4>
                    </div>
                    <form id="closedQuestionForm" class="closed-question-options mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check ">
                <input class="form-check-input closed-question-option" type="radio" name="closedQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-check-label text-body " for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateRangeQuestionHtml(step: RangeQuestionDto): string {
    return `<div class="text-center my-3">
                <h5 class="text-primary mb-3">Het huidige subthema: ${step.subtheme}</h5>
                <h4 class="text-body">${step.query}</h4>
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="range" class="form-control-range range-input" id="rangeQuestion" min="${step.minValue}" max="${step.maxValue}" value="${step.minValue}" oninput="document.getElementById('rangeValue').innerText = this.value">
                    <p id="rangeValue">${step.minValue}</p>
                </div>
            </div>`;
}


