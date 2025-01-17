import {
    ProjectDto
} from "../Configs/AddFlowConfig/projectDto";

const selectProjectBox = document.getElementById('projectId') as HTMLSelectElement

fetchAllProjectsOfSubplatform()
function fetchAllProjectsOfSubplatform(){
    fetch(`/api/Projects`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    })
        .then(response => {
            console.log(response);
            return response.json();
        })
        .then(projects => showProjectInComboBox(projects))
        .catch(error => {
            alert(error)
        });
}

function showProjectInComboBox(projects:ProjectDto[]){
    console.log(projects)
    selectProjectBox.innerHTML = '';

    const defaultOption = document.createElement('option');
    defaultOption.text = 'Kies een project';
    defaultOption.value = '0';
    selectProjectBox.appendChild(defaultOption);

    projects.forEach(project => {
        const option = document.createElement('option');
        option.text = project.name;
        option.value = ''+project.projectId;
        selectProjectBox.appendChild(option);
    });
}


