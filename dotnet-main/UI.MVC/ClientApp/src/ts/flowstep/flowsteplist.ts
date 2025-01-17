import {FlowStepDto} from "../Configs/FlowStepConfig/flowstepDto";
import {Option} from "../Configs/AddFlowConfig/flowstepDto";
import {loadSubthemesIntoSelectbox, loadSubthemesOfFlow} from "../subtheme/loadsubthemes";
import {StartDeleteFlowStep} from "./deleteflowstep";
import {getSelectedOption} from "./addflowstep";
import {CreateFlowstepFields} from "./createhtmlfields";
import {initializeAddSubthemeButton} from "../subtheme/addsubtheme";
import {handleDragEnd,handleDragEnter,handleDragLeave,handleDragOver,handleDragStart,handleDrop} from "./draganddrop";

const saveBtn = document.getElementById('saveButton') as HTMLButtonElement;
export let flowId: any = null;
export let flowIdNumber: number = 0;
let orderNr = 0;
export let newOrderNr = 0;
window.onload = function () {
    const currentUrl = window.location.href;
    const url = new URL(currentUrl);

    flowId = url.searchParams.get('flowid') as string;
    if (flowId === null) {
        flowId = url.searchParams.get('FlowId') as string;
    }

    flowIdNumber = parseInt(flowId, 10);

    console.log(flowId);
    fetchFlowstepsOfFlow(flowId);
    loadSubthemesOfFlow(flowId);
    loadSubthemesIntoSelectbox(flowId)
    initializeAddSubthemeButton();

};

export async function fetchFlowstepsOfFlow(flowId: string) {
    try {
        const response = await fetch(`/api/Flowsteps/${flowId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        // Check if response is ok (status 200-299)
        if (!response.ok) {
            throw new Error(`Server error: ${response.statusText}`);
        }

        // Try to parse the response as JSON
        let flowsteps;
        try {
            flowsteps = await response.json();
        } catch (error) {
            throw new Error('Invalid JSON response from server');
        }

        console.log(flowsteps.length);
        showFlowStepsInList(flowsteps);
    } catch (error) {
        // @ts-ignore
        alert(`Error fetching flowsteps: ${error.message}`);
        console.error('Fetch flowsteps error:', error);
    }
}



function showFlowStepsInList(flowsteps: FlowStepDto[]) {
    const flowStepList = document.getElementById('flowStepList') as HTMLUListElement;
    flowStepList.innerHTML = '';
    if (flowsteps.length === 0) {
        let html = `
            <div class="alert alert-info" style="max-width: 80%;" role="alert">
                <h4 class="alert-heading">Er zijn geen flowsteps beschikbaar voor deze flow.</h4>
            </div>`;
        const noSubthemesMessage = document.createElement('div');
        noSubthemesMessage.className = 'text-center my-3';
        noSubthemesMessage.innerHTML = html;
        flowStepList.appendChild(noSubthemesMessage);
    }
    flowsteps.sort((a, b) => a.orderNr - b.orderNr);

    newOrderNr = flowsteps.length;

    flowsteps.forEach((flowstep) => {
        const li = document.createElement('li');
        li.draggable = true;

        // Voeg event listeners toe voor de drag events
        li.addEventListener('dragstart', handleDragStart, false);
        li.addEventListener('dragenter', handleDragEnter, false);
        li.addEventListener('dragover', handleDragOver, false);
        li.addEventListener('dragleave', handleDragLeave, false);
        li.addEventListener('drop', handleDrop, false);
        li.addEventListener('dragend', handleDragEnd, false);
        li.className = 'list-group-item d-flex justify-content-between align-items-center';

        // Bootstrap classes toegevoegd voor styling
        li.classList.add('bg-light', 'border', 'border-secondary', 'rounded-3', 'mb-2', 'p-2');

        const textContainer = document.createElement('div');
        textContainer.className = 'd-flex align-items-center';

        // Icoon toegevoegd
        const icon = document.createElement('i');
        icon.className = 'bi bi-arrows-move';
        textContainer.appendChild(icon);

        const textSpan = document.createElement('span');
        textSpan.textContent = `${flowstep.flowStepType} - ${flowstep.subtheme}`;
        textSpan.setAttribute("data-flowstepid", flowstep.flowStepId.toString());
        textContainer.appendChild(textSpan);

        // Indicator for active flow step
        const statusBadge = document.createElement('span');
        statusBadge.className = flowstep.isActive ? 'badge bg-success ms-2' : 'badge bg-danger ms-2';
        statusBadge.textContent = flowstep.isActive ? 'Active' : 'Inactive';
        textContainer.appendChild(statusBadge);

        const buttonContainer = document.createElement('div'); // Container for buttons
        buttonContainer.className = 'btn-group';

        const deleteButton = document.createElement('button');
        deleteButton.className = 'btn btn-danger ms-2';
        deleteButton.textContent = 'Verwijderen';
        deleteButton.addEventListener('click', (event) => {
            const spanElement = deleteButton.parentElement?.parentElement?.querySelector('span');
            if (spanElement) {
                const flowStepId = spanElement.getAttribute('data-flowstepid');
                if (flowStepId) {
                    StartDeleteFlowStep(parseInt(flowStepId, 10));
                    deleteButton.parentElement?.parentElement?.remove();
                }
            }
        });

        const editButton = document.createElement('button');
        editButton.className = 'btn btn-primary ms-2';
        editButton.textContent = 'Bewerken';
        editButton.addEventListener('click', () => {
            window.location.href = `/Flowstep/Edit/${flowstep.flowStepId}`;
        });

        buttonContainer.appendChild(editButton);
        buttonContainer.appendChild(deleteButton);

        li.appendChild(textContainer);
        li.appendChild(buttonContainer);
        flowStepList.appendChild(li);
    });
    CreateFlowstepFields();
}


const questionTypeSelect = document.getElementById('questionType') as HTMLSelectElement;
const subthemeSelectbox = document.getElementById('subthemeSelectbox') as HTMLSelectElement;
let subthemeId: string | undefined = '';

export async function addFlowStep() {

    await getSelectedOption(document.getElementById('subthemeSelectbox') as HTMLSelectElement);
    console.log('addFlowstep methode');
}



