import {SubthemeDto} from "../Configs/AddFlowConfig/subthemeDto";
import {flowIdNumber} from "../flowstep/flowsteplist";
import {loadSubthemesIntoSelectbox, loadSubthemesOfFlow} from "./loadsubthemes";


export function addSubtheme(subtheme: SubthemeDto | any) {
    fetch('/api/Subthemes/post', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(subtheme)
    })
        .then(async response => {
            if (response.status === 400) {
                console.log("test")
                const errorData = await response.json();
                displayValidationErrorsSubtheme(errorData.errors);

            }
            if (!response.ok) {
                throw new Error('Error');
            }

            displaySuccessMessage('Subthema toegevoegd');
            console.log(response)
            return response.json();
        })
        .then(data => {
            console.log('Subthema toegevoegd:', data);
        })
        .catch(error => {
            console.error('Error', error);
        });
}

export function initializeAddSubthemeButton() {
    console.log("initializeAddSubthemeButton")
    const addSubthemeButton = document.getElementById('addSubthemeButton');
    if (addSubthemeButton) {
        addSubthemeButton.addEventListener('click', function (event) {
            const title = document.getElementById('subthemeTitle') as HTMLInputElement;
            const body = document.getElementById('subthemeBody') as HTMLTextAreaElement;
            const photo = document.getElementById('subthemePhoto') as HTMLInputElement;

            if (title && body) {
                const subthemeData: SubthemeDto = {
                    subthemeId: 0,
                    title: title.value,
                    body: body.value,
                    urlPhoto: photo ? photo.value : undefined,
                    flow: flowIdNumber
                };

                addSubtheme(subthemeData);
                loadSubthemesOfFlow(flowIdNumber);
                if (title) title.value = '';
                if (body) body.value = '';
                if (photo) photo.value = '';
            } else {
                console.error("Een of meer elementen zijn niet gevonden.");
            }

        });
    } else {
        console.error("De knop met de id 'addSubthemeButton' is niet gevonden.");
    }
}

//todo, Eris al een functie in form.ts met van deze functie maar ik krijg een error als ik die wil importen -- marwane
function displaySuccessMessage(message: string) {
    const alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-success';
    alertDiv.style.position = 'fixed';
    alertDiv.style.bottom = '20px';
    alertDiv.style.right = '20px';
    alertDiv.textContent = message;
    document.body.appendChild(alertDiv);

    setTimeout(() => {
        alertDiv.remove();
        loadSubthemesOfFlow(flowIdNumber)
        loadSubthemesIntoSelectbox(flowIdNumber)
    }, 2000);
}

function displayValidationErrorsSubtheme(errors: { [key: string]: string[] }) {
    const errorSummary = document.getElementById('validationSummarySubthema');
    if (!errorSummary) return;
    console.log(errors);

    errorSummary.innerHTML = '<h4>Fouten:</h4>';
    for (const field in errors) {
        if (errors.hasOwnProperty(field)) {
            errors[field].forEach(errorMessage => {
                const errorItem = document.createElement('div');
                console.log(errorMessage);
                errorItem.textContent = errorMessage;
                errorSummary.appendChild(errorItem);
            });
        }
    }
    errorSummary.style.display = 'block';
}