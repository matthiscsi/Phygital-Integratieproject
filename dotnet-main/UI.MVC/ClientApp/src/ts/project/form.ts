import { ProjectDto } from "../Configs/ProjectConfig/ProjectDto";

const form = document.getElementById('form') as HTMLFormElement | null;
const projectSpan = document.getElementById('projectSpanId');
const projectiddom = projectSpan ? projectSpan.innerHTML : '';

document.addEventListener('DOMContentLoaded', function() {
    if (!form) return; 

    form.addEventListener('submit', async function(event) {
        event.preventDefault(); 

        const projectName = (document.getElementById('Name') as HTMLInputElement).value;
        const projectDescription = (document.getElementById('Description') as HTMLTextAreaElement).value;
        const isActive = (document.getElementById('activeRadio') as HTMLInputElement).checked;
        const circulair = (document.getElementById('circulairRadio') as HTMLInputElement).checked;
        const background = (document.getElementById('BackgroundColor') as HTMLInputElement).value;
        const font = (document.getElementById('Font') as HTMLInputElement).value;
        let projectId = parseInt(projectiddom);

        const projectDto: ProjectDto = {
            projectId: projectId,
            Circulair: circulair,
            name: projectName,
            description: projectDescription,
            isActive: isActive,
            backgroundColor: background,
            font: font
        };

        try {
            const response = await fetch('/api/Projects/FlowSoort', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(projectDto)
            });

            if (response.ok) {
                console.log('Project updated successfully');
                displaySuccessMessage('Project is succesvol aangepast');
                const errorSummary = document.getElementById('validationSummary') as HTMLDivElement | null;
                if (errorSummary) {
                    errorSummary.style.display = 'none';
                }
            } else if (response.status === 400) {
                const errorData = await response.json();
                displayValidationErrors(errorData.errors);
            } else {
                throw new Error('Failed to update project');
            }
        } catch (error) {
            console.error('Error updating project:', (error as Error).message);
        }
    });
});

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
    }, 2000);
}

export function displayValidationErrors(errors: { [key: string]: string[] }) {
    const errorSummary = document.getElementById('validationSummary') as HTMLDivElement | null;
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
