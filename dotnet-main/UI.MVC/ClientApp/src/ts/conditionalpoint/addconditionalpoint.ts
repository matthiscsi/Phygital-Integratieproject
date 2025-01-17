
import {ConditionalPointDto} from "../Configs/FlowStepConfig/conditionalpointdto";
import {
    ClosedQuestionDto, FlowStepDto,
    InfoDto,
    MultipleChoiceQuestionDto,
    OpenQuestionDto, OptionDto, QuestionDto, RangeQuestionDto
} from "../Configs/FlowStepConfig/flowstepDto";
import {MultipleChoiceQuestion, Option} from "../Configs/AddFlowConfig/flowstepDto";
import {
    AnswerDto, ClosedAnwser, MultipleChoiceAnswerDto, openAnswer, RangeAnswerDtoo
} from "../Configs/FlowStepConfig/AddAnswersInterfaces";



let newOrderNr = 0;
let subthemeId: string | undefined = '';
let flowStepData : FlowStepDto | QuestionDto | OpenQuestionDto | ClosedQuestionDto | RangeQuestionDto | MultipleChoiceQuestionDto;
let flowIdNumber = 0;
let stapje: HTMLDivElement;
let currentStepType= '';

let addFlowstepBtn: HTMLButtonElement | null = null;
let rangeQuestionFields: HTMLElement;

let openQuestionFields:  HTMLElement;
let infoFields:  HTMLElement;
let closedQuestionFields:  HTMLElement;
let closedOptionsContainer:  HTMLElement;
let multipleChoiceQuestionFields:  HTMLElement;
let multipleChoiceOptionsContainer:  HTMLElement;
let flowStepId = 0;

let questionTypeSelect: HTMLSelectElement;
document.addEventListener('DOMContentLoaded', async function () {
    const addButton = document.getElementById('addConditionalPointButton');
    if (addButton) {
        addButton.addEventListener('click', addConditionalPoint);
    }   
    openQuestionFields = document.getElementById('openQuestionFields') as HTMLElement;
    infoFields = document.getElementById('infoFields') as HTMLElement;
    closedQuestionFields = document.getElementById('closedQuestionFields') as HTMLElement;
    closedOptionsContainer = document.getElementById('closedOptionsContainer') as HTMLElement;
    multipleChoiceQuestionFields = document.getElementById('multipleChoiceQuestionFields') as HTMLElement;
    multipleChoiceOptionsContainer = document.getElementById('multipleChoiceOptionsContainer') as HTMLElement;
    const flowStepIdElement = document.getElementById('flowStepId') as HTMLSpanElement;
    flowStepId = parseInt(flowStepIdElement.innerText);
    rangeQuestionFields = document.getElementById('rangeQuestionFields') as HTMLElement;
    stapje = document.getElementById('stapje') as HTMLDivElement;
    questionTypeSelect = document.getElementById('questionType') as HTMLSelectElement;
    
    await fetchFlowStep(flowStepId);
    CreateFlowstepFields()

   
});



async function addConditionalPoint() {
    let question;
    try {
        await getSelectedOption();
      
        let criteria = await defineAnswerRequest();
        console.log(criteria)
        if (criteria == null) {
        } else {
            const conditionalPointDto: ConditionalPointDto = {
                Question: question = {
                    flowStepId: flowStepId
                } as QuestionDto,
                Criteria: criteria,
                FollowUpStep: flowStepData
            };
            flowStepData = flowStepData as OpenQuestionDto
            console.log(flowStepData.flowStepType)
            
            await postConditionalPoint(conditionalPointDto);
            window.location.href = `/Flowstep/Edit/${flowStepId}`;
        }
    } catch (error) {
        console.error('Error adding conditional point:', error);
        alert('Failed to add conditional point.');
    }
}
export async function defineAnswerRequest(): Promise<AnswerDto | null> {
    let answerRequest: AnswerDto | ClosedAnwser | MultipleChoiceAnswerDto | RangeAnswerDtoo | openAnswer | null = null;
   {
        const closedQuestionRadio = document.querySelector('#closedQuestionForm input[name="closedQuestion"]:checked') as HTMLInputElement;
        const openQuestionInput = document.querySelector('#openQuestion') as HTMLInputElement;
        const multipleChoiceCheckboxes = document.querySelectorAll('#multipleChoiceQuestionForm input[name="multipleChoiceQuestion"]:checked');
        const rangeQuestionInput = document.querySelector('#rangeQuestion') as HTMLInputElement;

        if (closedQuestionRadio) {
            answerRequest = {
                selectedOption: parseInt(closedQuestionRadio.value, 10),
                answerType: "Closed"
            } 
        } else if (multipleChoiceCheckboxes.length > 0) {
            const selectedOptions = Array.from(multipleChoiceCheckboxes).map(checkbox => parseInt((checkbox as HTMLInputElement).value, 10));
            const optionDtos: OptionDto[] = selectedOptions.map(optionId => ({
                id: optionId,
                text: ''
            }));
            answerRequest = {
                options: optionDtos,
                selectedAnswers: selectedOptions,
                answerType: "MultipleChoice"
            }
        } else if (rangeQuestionInput) {
            answerRequest = {
                selectedAnswer: parseInt(rangeQuestionInput.value, 10),
                answerType: "Range"
            }
        }else if (openQuestionInput && openQuestionInput.value.trim() !== '') {
            answerRequest = {
                Answer: openQuestionInput.value.trim(),
                answerType: "Open"
            } 
        } 
        return answerRequest

    }
}
export async function fetchFlowStep(flowStepId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/getStep/${flowStepId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            const stepData = await response.json();
            
            identifyStepType(stepData); 
        } else if (response.status === 404) {
            
        } else {
            console.error('Failed to fetch the step');
        }
    } catch (error) {
        console.error('Error fetching step:', error);
    }
}

function identifyStepType(step: any): string {
    newOrderNr = step.orderNr + 1;
    subthemeId = step.subthemeId;
    flowIdNumber = step.FlowId
    switch (step.flowStepType) {
        case "Info":
            // @ts-ignore
            currentStepType = "Info";
            stapje.innerHTML = generateInfoHtml(step);
            break;
        case "ClosedQuestion":
            // @ts-ignore
            currentStepType = "ClosedQuestion";
            stapje.innerHTML = generateClosedQuestionHtml(step);
            break;
        case "OpenQuestion":
            // @ts-ignore
            currentStepType = "OpenQuestion";
            stapje.innerHTML = generateOpenQuestionHtml(step);
            break;
        case "MultipleChoiceQuestion":
            // @ts-ignore
            currentStepType = "MultipleChoiceQuestion";
            stapje.innerHTML = generateMultipleChoiceQuestionHtml(step);
            break;
        case "RangeQuestion":
            // @ts-ignore
            currentStepType = "RangeQuestion";
            stapje.innerHTML = generateRangeQuestionHtml(step);
            break;
        default:
            break;
    }
    return currentStepType;
}

export async function getSelectedOption() {
    
       
        switch (questionTypeSelect.value) {
            case 'closed':
                flowStepData = await collectClosedQuestionData(newOrderNr, subthemeId);
                break;
            case 'info':
                flowStepData = await collectInfoData(newOrderNr, subthemeId);
                break;
            case 'multiple':
                flowStepData = await collectMultipleChoiceQuestionData(newOrderNr, subthemeId);
                break;
            case 'range':
                flowStepData = await collectRangeQuestionData(newOrderNr, subthemeId);
                break;
            case 'open':
                flowStepData = await collectOpenQuestionData(newOrderNr, subthemeId);
                break;
            default:
                console.error('Unknown question type');
                return;
        }
        
        
}

export async function postConditionalPoint(conditionalpoint: ConditionalPointDto){
    try {
        console.log(conditionalpoint)
        const response = await fetch('/api/Flowsteps/postConditionalPoint', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(conditionalpoint)
        });

        if (!response.ok) {
            throw new Error('Failed to add conditional point');
        }

    } catch (error) {
        console.error('Error adding conditional point:', error);
    }
}


export function generateInfoHtml(step: InfoDto): string {
    let html = `
    <div class="container mt-3">
        <div class="row">
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


export function generateMultipleChoiceQuestionHtml(step: MultipleChoiceQuestionDto): string {
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h4 class="text-body">${step.query}</h4>
                    </div>
                    <form id="multipleChoiceQuestionForm" class="mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check">
                <input class="form-check-input" type="checkbox" name="multipleChoiceQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-check-label text-body" for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateOpenQuestionHtml(step: OpenQuestionDto): string {
    return `<div class="text-center my-3">
                <h4 class="text-body">${step.query}</h4>
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="text" class="form-control" id="openQuestion" placeholder="Enter your answer">
                </div>
            </div>`;
}

export function generateClosedQuestionHtml(step: ClosedQuestionDto): string {
 
    let optionsHtml = '';
    optionsHtml += `<div class="text-center my-3">
                        <h4 class="text-body">${step.query}</h4>
                    </div>
                    <form id="closedQuestionForm" class="mx-auto" style="max-width: 80%;">`;

    step.options.forEach(option => {
        optionsHtml += `
            <div class="form-check">
                <input class="form-check-input" type="radio" name="closedQuestion" id="option${option.id}" value="${option.id}">
                <label class="form-check-label text-body" for="option${option.id}">${option.text}</label>
            </div>`;
    });

    optionsHtml += '</form>';
    return optionsHtml;
}

export function generateRangeQuestionHtml(step: RangeQuestionDto): string {
    return `<div class="text-center my-3">
                <h4 class="text-body">${step.query}</h4>
                <div class="form-group mx-auto" style="max-width: 80%;">
                    <input type="range" class="form-control-range" id="rangeQuestion" min="${step.minValue}" max="${step.maxValue}" value="${step.minValue}" oninput="rangeValue.innerText = this.value">
                    <p id="rangeValue">0</p>
                </div>
            </div>`;
}



export async function collectClosedQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('closedQuestionText') as HTMLInputElement).value;
    const numOptionsSelect = document.getElementById('numOptions') as HTMLSelectElement;
    const numOptions = parseInt(numOptionsSelect.value);

    const options: Option[] = [];
    for (let i = 1; i <= numOptions; i++) {
        const optionInput = document.getElementById(`closedOption${i}`) as HTMLInputElement;
        const text = optionInput.value;
        options.push({text});
    }

    flowStepData = {
        options : options ,
        flowStepType : "ClosedQuestion",
        query : query,
        
    } as ClosedQuestionDto
    return flowStepData;

}



export async function collectInfoData(orderNr: number, subthemeId: string | undefined) {
    const infoBody = (document.getElementById('infoBody') as HTMLTextAreaElement).value;
    const imageInput = document.getElementById('infoImage') as HTMLInputElement;
    const videoInput = document.getElementById('infoVideo') as HTMLInputElement;
    const audioInput = document.getElementById('infoAudio') as HTMLInputElement;

    let uploadedImage, uploadedVideo, uploadedAudio;

    if (imageInput.files && imageInput.files.length > 0) {
        uploadedImage = await postOnlyFile(imageInput.files[0]);
        console.log('Image URL:', uploadedImage);
    }

    if (videoInput.files && videoInput.files.length > 0) {
        uploadedVideo = await postOnlyFile(videoInput.files[0]);
        console.log('Video URL:', uploadedVideo);
    }

    if (audioInput.files && audioInput.files.length > 0) {
        uploadedAudio = await postOnlyFile(audioInput.files[0]);
        console.log('Audio URL:', uploadedAudio);
    }

    flowStepData = {
        flowStepType: "Info",
        body: infoBody,
        uploadedAudio: uploadedAudio,
        uploadedVideo: uploadedVideo,
        uploadedImage: uploadedImage
    } as InfoDto
    return flowStepData;
}


export async function collectMultipleChoiceQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('multipleChoiceQuestionText') as HTMLInputElement).value;
    const numOptionsSelect = document.getElementById('numMCOptions') as HTMLSelectElement;
    const numOptions = parseInt(numOptionsSelect.value);

    const options: Option[] = [];
    for (let i = 1; i <= numOptions; i++) {
        const optionInput = document.getElementById(`multipleChoiceOption${i}`) as HTMLInputElement;
        const text = optionInput.value;
        options.push({text});
    }

    flowStepData = {
        query: query,
        options: options,
        flowStepType: "MultipleChoiceQuestion",
        
    } as MultipleChoiceQuestionDto
    return flowStepData;
    
}

export async function collectRangeQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('query') as HTMLInputElement).value;
    const minValue = parseInt((document.getElementById('minValue') as HTMLInputElement).value);
    const maxValue = parseInt((document.getElementById('maxValue') as HTMLInputElement).value);

    flowStepData = {
        query: query,
        minValue: minValue,
        maxValue: maxValue,
        flowStepType: "RangeQuestion"
    } as RangeQuestionDto
    return flowStepData;
}

export async function collectOpenQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('openQuestion') as HTMLInputElement).value;
    flowStepData = {
        query: query,
        flowStepType: "OpenQuestion"
    } as OpenQuestionDto
    return flowStepData;
}


export function CreateFlowstepFields(){
    questionTypeSelect.addEventListener('change', () => {
        if (questionTypeSelect.value === 'range') {
            rangeQuestionFields.style.display = 'block';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            if (!document.getElementById('addFlowstep')) {
                const buttonDiv = document.createElement('div');
                const rowElement = document.querySelector('.row');
                if (rowElement) {
                    rowElement.appendChild(buttonDiv.firstChild as HTMLElement);
                    addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
                }
                if (addFlowstepBtn) {
                    addFlowstepBtn.addEventListener('click', () => {
                        const selectedType = questionTypeSelect.value;
                        console.log('Flowstep toevoegen voor type:', selectedType);
                        addFlowStep()
                    });
                }
            }
        } else if (questionTypeSelect.value === 'open') {
            openQuestionFields.style.display = 'block';

            rangeQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            if (!document.getElementById('addFlowstep')) {
                const buttonDiv = document.createElement('div');
                const rowElement = document.querySelector('.row');
                if (rowElement) {
                    rowElement.appendChild(buttonDiv.firstChild as HTMLElement);
                    addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
                }
                if (addFlowstepBtn) {
                    addFlowstepBtn.addEventListener('click', () => {
                        const selectedType = questionTypeSelect.value;
                        console.log('Flowstep toevoegen voor type:', selectedType);
                        addFlowStep();
                    });
                }
            }
        } else if (questionTypeSelect.value === 'info') {
            infoFields.style.display = 'block';

            openQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            if (!document.getElementById('addFlowstep')) {
                const buttonDiv = document.createElement('div');
                const rowElement = document.querySelector('.row');
                if (rowElement) {
                    rowElement.appendChild(buttonDiv.firstChild as HTMLElement);
                    addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
                }
                if (addFlowstepBtn) {
                    addFlowstepBtn.addEventListener('click', () => {
                        const selectedType = questionTypeSelect.value;
                        console.log('Flowstep toevoegen voor type:', selectedType);
                        addFlowStep();
                    });
                }
            }
        } else if (questionTypeSelect.value === 'closed') {
            closedQuestionFields.style.display = 'block';

            multipleChoiceQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            if (!document.getElementById('addFlowstep')) {
                const buttonDiv = document.createElement('div');
                const rowElement = document.querySelector('.row');
                if (rowElement) {
                    rowElement.appendChild(buttonDiv.firstChild as HTMLElement);
                    addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
                }
                if (addFlowstepBtn) {
                    addFlowstepBtn.addEventListener('click', () => {
                        const selectedType = questionTypeSelect.value;
                        console.log('Flowstep toevoegen voor type:', selectedType);
                        addFlowStep().then(r => console.log('Flowstep toegevoegd'));
                    });
                }
            }
        } else if (questionTypeSelect.value === 'multiple') {
            multipleChoiceQuestionFields.style.display = 'block';

            closedQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            if (!document.getElementById('addFlowstep')) {
                const buttonDiv = document.createElement('div');
                const rowElement = document.querySelector('.row');
                if (rowElement) {
                    rowElement.appendChild(buttonDiv.firstChild as HTMLElement);
                    addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
                }
                if (addFlowstepBtn) {
                    addFlowstepBtn.addEventListener('click', () => {
                        const selectedType = questionTypeSelect.value;
                        console.log('Flowstep toevoegen voor type:', selectedType);
                        addFlowStep();
                    });
                }
            }
        } else {

            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            const addFlowstep = document.getElementById('addFlowstep');
            if (addFlowstep) {
                addFlowstep.remove();
            }
        }
    });

    const numOptionsSelect = document.getElementById('numOptions') as HTMLSelectElement;
    const numMCOptionsSelect = document.getElementById('numMCOptions') as HTMLSelectElement;

    numOptionsSelect.addEventListener('change', () => {
        const numOptions = parseInt(numOptionsSelect.value);
        const container = closedOptionsContainer;
        container.innerHTML = '';

        for (let i = 1; i <= numOptions; i++) {
            const optionInput = document.createElement('input');
            optionInput.type = 'text';
            optionInput.id = `closedOption${i}`;
            optionInput.className = 'form-control';
            optionInput.placeholder = `Optie ${i}`;
            container.appendChild(optionInput);
        }
    });

    numMCOptionsSelect.addEventListener('change', () => {
        const numOptions = parseInt(numMCOptionsSelect.value);
        const container = multipleChoiceOptionsContainer;
        container.innerHTML = '';

        for (let i = 1; i <= numOptions; i++) {
            const optionInput = document.createElement('input');
            optionInput.type = 'text';
            optionInput.id = `multipleChoiceOption${i}`;
            optionInput.className = 'form-control';
            optionInput.placeholder = `Optie ${i}`;
            container.appendChild(optionInput);
        }
    });
}


export async function postOnlyFile(file: File) {
    try {
        const formData = new FormData();
        formData.append('file', file);
        const response = await fetch('/api/Upload/upload', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            console.log(response);
            throw new Error('Failed to upload file');
        }

        const data = await response.json();
        console.log('File uploaded successfully, URL:', data.url);
        return data.url;
    } catch (error) {
        console.error('Error uploading file:', error);
    }
}


export async function addFlowStep() {
    await getSelectedOption();
}
function hideAllFields() {
    rangeQuestionFields.style.display = 'none';
    openQuestionFields.style.display = 'none';
    infoFields.style.display = 'none';
    closedQuestionFields.style.display = 'none';
    multipleChoiceQuestionFields.style.display = 'none';
}