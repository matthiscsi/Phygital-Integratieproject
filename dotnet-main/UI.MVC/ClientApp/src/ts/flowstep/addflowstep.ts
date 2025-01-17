import {fetchFlowstepsOfFlow, flowId, flowIdNumber, newOrderNr} from "./flowsteplist";
import {
    collectClosedQuestionData,
    collectInfoData,
    collectMultipleChoiceQuestionData,
    collectOpenQuestionData,
    collectRangeQuestionData
} from "./collectdata";
import {generateClosedOptions, generateMultipleChoiceOptions} from "./createhtmlfields";

const questionTypeSelect = document.getElementById('questionType') as HTMLSelectElement;
const subthemeSelectbox = document.getElementById('subthemeSelectbox') as HTMLSelectElement;

let subthemeId: string | undefined = '';

export async function getSelectedOption(selectElement: HTMLSelectElement) {
    if (subthemeSelectbox) {
        const selectedOption = subthemeSelectbox.options[subthemeSelectbox.selectedIndex];

        if (selectedOption) {
            subthemeId = selectedOption.getAttribute('data-subthemeId') || '';
        }

        let flowStepData;

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
                console.log("adding openquestion");
                flowStepData = await collectOpenQuestionData(newOrderNr, subthemeId);
                break;
            default:
                console.error('Unknown question type');
                return;
        }

        console.log("getSelected method")

        await postFlowStep(flowStepData);
        await fetchFlowstepsOfFlow(flowIdNumber.toString());
    }
}

async function postFlowStep(flowStepData: any) {
    try {
        const response = await fetch('/api/Flowsteps/post', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(flowStepData)
        });

        if (response.status === 400) {
            const errorData = await response.json();
            console.log("hier")


            if (errorData.errors) {

                displayValidationErrorsFlowstep(errorData.errors);
            } else {
                displayValidationErrorsFlowstep(errorData);
            }
        }

        if (response.ok) {
            displaySuccessMessage('Flowstep toegevoegd');
        }

        return response.json();
    } catch (error) {
        console.error('Error adding flow step:', error);
    }
}


//todo, Eris al een functie in form.ts met van deze functie maar ik krijg een error als ik die wil importen -- marwane
function displaySuccessMessage(message: string) {
    const errorSummary = document.getElementById('validationSummaryFlowstep');
    if (errorSummary) {
        errorSummary.style.display = 'none';
    }
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-success';
    alertDiv.style.position = 'fixed';
    alertDiv.style.bottom = '20px';
    alertDiv.style.right = '20px';
    alertDiv.textContent = message;
    document.body.appendChild(alertDiv);
    resetDivFields('rangeQuestionFields');
    resetDivFields('openQuestionFields');
    resetDivFields('infoFields');
    resetDivFields('closedQuestionFields');
    resetDivFields('closedOptionsContainer');
    resetDivFields('multipleChoiceQuestionFields');
    resetDivFields('multipleChoiceOptionsContainer');
    setTimeout(() => {
        alertDiv.remove();
        console.log(flowId)
        fetchFlowstepsOfFlow(flowId)
    }, 2000);
}

function resetDivFields(divId: string) {
    const div = document.getElementById(divId);
    if (div) {
        const inputs = div.getElementsByTagName('input');
        for (let i = 0; i < inputs.length; i++) {
            generateMultipleChoiceOptions(0);
            generateClosedOptions(0);
            if (inputs[i].type === 'text' || inputs[i].type === 'number') {
                inputs[i].value = '';
            } else if (inputs[i].type === 'checkbox' || inputs[i].type === 'radio') {
                inputs[i].checked = false;
            } else if (inputs[i].type === 'file') {
                // @ts-ignore
                inputs[i].value = null;
            }
        }

        const selects = div.getElementsByTagName('select');
        for (let i = 0; i < selects.length; i++) {
            if (selects[i].id === 'numMCOptions') continue;
            selects[i].selectedIndex = 0;
        }

        const textareas = div.getElementsByTagName('textarea');
        for (let i = 0; i < textareas.length; i++) {
            if (textareas[i].id === 'numMCOptions') continue;
            textareas[i].value = '';
        }
    }
}


function displayValidationErrorsFlowstep(errors: { [key: string]: string[] | string }) {
    const errorSummary = document.getElementById('validationSummaryFlowstep');
    if (!errorSummary) return;
    console.log(errors);

    errorSummary.innerHTML = '<h4>Fouten:</h4>';
    for (const field in errors) {
        if (errors.hasOwnProperty(field)) {
            const errorMessages = errors[field];
            if (Array.isArray(errorMessages)) {
                errorMessages.forEach(errorMessage => {
                    const errorItem = document.createElement('div');
                    console.log(errorMessage);
                    errorItem.textContent = errorMessage;
                    errorSummary.appendChild(errorItem);
                });
            } else {
                const errorItem = document.createElement('div');
                console.log(errorMessages);
                errorItem.textContent = errorMessages;
                errorSummary.appendChild(errorItem);
            }
        }
    }
    errorSummary.style.display = 'block';
}

export async function postOnlyFile(file: File) {
    const loadingIndicator = document.getElementById('loadingFile') as HTMLDivElement;

    try {
        loadingIndicator.style.display = 'block'; // Show the loading indicator
        const formData = new FormData();
        formData.append('file', file);
        const response = await fetch('/api/Uploads/upload', {
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
    } finally {
        loadingIndicator.style.display = 'none'; // Hide the loading indicator
    }
}

