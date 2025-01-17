import {
    ClosedQuestionDto,
    InfoDto,
    MultipleChoiceQuestionDto,
    OpenQuestionDto,
    RangeQuestionDto
} from "../Configs/FlowStepConfig/flowstepDto";


export function generateInfoHtmlHost(step: InfoDto): string {
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

    if (step.uploadedVideo) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-2">
                <video controls class="w-100 mb-2">
                    <source src="${step.uploadedVideo}" type="video/mp4">
                    Your browser does not support the video tag.
                </video>
            </div>`;
    }

    if (step.uploadedImage) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-3">
                <img src="${step.uploadedImage}" class="img-fluid rounded mb-2" alt="Subthema image">
            </div>`;
    }

    if (step.uploadedAudio) {
        html += `
            <div class="col-md-6 col-lg-6 order-md-4">
                <audio controls class="w-100 mb-2">
                    <source src="${step.uploadedAudio}" type="audio/mp3">
                    Your browser does not support the audio element.
                </audio>
            </div>`;
    }

    html += `
        </div>
    </div>`;

    return html;
}


export function generateMultipleChoiceQuestionHtmlHost(step: MultipleChoiceQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h5 class="text-primary mb-3">het huidige subthema: ${step.subtheme}</h5>
                        <h4 class="text-info">${step.query}</h4>
                    </div>`;

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateOpenQuestionHtmlHost(step: OpenQuestionDto): string {
    return `<div class="text-center my-3">
                <h5 class="text-primary mb-3">het huidige subthema: ${step.subtheme}</h5>
                <h4 class="text-info">${step.query}</h4>
            </div>`;
}

export function generateClosedQuestionHtmlHost(step: ClosedQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h5 class="text-primary mb-3">het huidige subthema: ${step.subtheme}</h5>
                        <h4 class="text-info">${step.query}</h4>
                    </div>`;

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateRangeQuestionHtmlHost(step: RangeQuestionDto): string {
    return `<div class="text-center my-3">
                <h5 class="text-primary mb-3">het huidige subthema: ${step.subtheme}</h5>
                <h4 class="text-info">${step.query}</h4>
            </div>`;
}


