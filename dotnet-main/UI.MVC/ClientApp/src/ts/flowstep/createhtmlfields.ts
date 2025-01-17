import {addFlowStep} from './flowsteplist';

let addFlowstepBtn = document.getElementById('addFlowstep') as HTMLButtonElement;
const questionTypeSelect = document.getElementById('questionType') as HTMLSelectElement;
const rangeQuestionFields = document.getElementById('rangeQuestionFields') as HTMLElement;
const openQuestionFields = document.getElementById('openQuestionFields') as HTMLElement;
const infoFields = document.getElementById('infoFields') as HTMLElement;
const closedQuestionFields = document.getElementById('closedQuestionFields') as HTMLElement;
const closedOptionsContainer = document.getElementById('closedOptionsContainer') as HTMLElement;
const multipleChoiceQuestionFields = document.getElementById('multipleChoiceQuestionFields') as HTMLElement;
const multipleChoiceOptionsContainer = document.getElementById('multipleChoiceOptionsContainer') as HTMLElement;
const selectSubtheme = document.getElementById('subthemeSelect') as HTMLSelectElement;

console.log('addFlowstepBtn:', addFlowstepBtn);
if (addFlowstepBtn) {
    console.log('addFlowstepBtn:', addFlowstepBtn);
    addFlowstepBtn.addEventListener('click', () => {
        const selectedType = questionTypeSelect.value;
        console.log('Flowstep toevoegen voor type:', selectedType);
        addFlowStep();
     
    });
}

export function CreateFlowstepFields() {
    questionTypeSelect.addEventListener('change', () => {
        if (questionTypeSelect.value === 'range') {
            rangeQuestionFields.style.display = 'block';
            selectSubtheme.style.display = 'block';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            addFlowstepBtn.style.display = 'block';
        } else if (questionTypeSelect.value === 'open') {
            openQuestionFields.style.display = 'block';
            selectSubtheme.style.display = 'block';
            rangeQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            addFlowstepBtn.style.display = 'block';
        } else if (questionTypeSelect.value === 'info') {
            infoFields.style.display = 'block';
            selectSubtheme.style.display = 'block';
            openQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            addFlowstepBtn.style.display = 'block';
        } else if (questionTypeSelect.value === 'closed') {
            closedQuestionFields.style.display = 'block';
            selectSubtheme.style.display = 'block';
            multipleChoiceQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            addFlowstepBtn.style.display = 'block';
        } else if (questionTypeSelect.value === 'multiple') {
            multipleChoiceQuestionFields.style.display = 'block';
            selectSubtheme.style.display = 'block';
            closedQuestionFields.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            addFlowstepBtn.style.display = 'block';
            generateMultipleChoiceOptions(0); // Ensure at least 2 options
        } else {
            selectSubtheme.style.display = 'none';
            rangeQuestionFields.style.display = 'none';
            openQuestionFields.style.display = 'none';
            infoFields.style.display = 'none';
            closedQuestionFields.style.display = 'none';
            multipleChoiceQuestionFields.style.display = 'none';
            addFlowstepBtn.style.display = 'none';
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
        generateClosedOptions(numOptions);
    });

    numMCOptionsSelect.addEventListener('change', () => {
        const numOptions = parseInt(numMCOptionsSelect.value);
        generateMultipleChoiceOptions(numOptions);
    });
}

export function generateClosedOptions(numOptions: number) {
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
}


export function generateMultipleChoiceOptions(numOptions: number) {
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
}
