import {Note} from "../Configs/AddFlowConfig/flowstepDto";

const notitiebutton = document.getElementById('Notitie');

const projectiddom = document.getElementById('projectId')

const textFieldContainer = document.createElement('div');
textFieldContainer.className = 'text-field-container d-none'; // 

const titlefield = document.createElement('input');
titlefield.type = 'text';
titlefield.placeholder = 'Enter Title here...';
titlefield.className = 'form-control fancy-textfield';
const textfield = document.createElement('input');
textfield.type = 'text';
textfield.placeholder = 'Enter your note here...';
textfield.className = 'form-control fancy-textfield';


titlefield.style.fontSize = '20px'; 
titlefield.style.fontWeight = 'bold';

const saveButton = document.createElement('button');
saveButton.className = 'btn btn-primary mt-2';
saveButton.id = 'saveButton';
saveButton.innerText = 'Save';

textFieldContainer.appendChild(titlefield);
textFieldContainer.appendChild(textfield);
textFieldContainer.appendChild(saveButton);

document.body.appendChild(textFieldContainer);

function toggleTextField() {
    if (textFieldContainer.classList.contains('d-none')) {
        textFieldContainer.classList.remove('d-none');
        textFieldContainer.classList.add('animate-slide-up');
    } else {
        textFieldContainer.classList.add('d-none');
        textFieldContainer.classList.remove('animate-slide-up');
    }
}

// @ts-ignore
notitiebutton.addEventListener('click', () => {
    toggleTextField();
});
const flowsessionid = document.getElementById('flowsessionid')
let flowsessionidval: number = 0;
if(flowsessionid){
    flowsessionidval = parseInt(flowsessionid.innerHTML);
}


saveButton.addEventListener('click', async () => {
    try {

        const note :Note = {
            FlowsessionId : flowsessionidval,
            Title: titlefield.value,
            Description: textfield.value
        };

        await fetch('/api/FlowSessions/notitie', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(note)

        });
        console.log(note);
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-success';
        alertDiv.style.position = 'fixed';
        alertDiv.style.bottom = '20px';
        alertDiv.style.right = '20px';
        alertDiv.textContent = 'Note Saved';
        document.body.appendChild(alertDiv);
        setTimeout(() => {
            alertDiv.remove();
        }, 3000);
        toggleTextField();
    } catch (error) {
        console.error('Error saving note:', error);
        alert('Failed to save note.');
    }
});
