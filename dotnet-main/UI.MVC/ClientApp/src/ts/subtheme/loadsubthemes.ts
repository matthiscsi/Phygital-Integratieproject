import {SubthemeDto} from "../Configs/AddFlowConfig/subthemeDto";
import {startDeleteSubtheme} from "./deletesubtheme";

window.onload = function () {
    const currentUrl = window.location.href;
    const url = new URL(currentUrl);


};


export function loadSubthemesOfFlow(flowId: number) {
    fetch(`/api/Subthemes/flow/${flowId}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch subthemes');
            }
            return response.json();
        })
        .then(data => {
            const subthemeList = document.getElementById('subthemeList');
            if (!subthemeList) return;
            subthemeList.innerHTML = '';

            if (data.length === 0) {
                let html = `
                        <div class="alert alert-info" style="max-width: 80%;" role="alert">
                            <h4 class="alert-heading">Er zijn geen subthema's beschikbaar voor deze flow.</h4>
                        </div>`;
                const noSubthemesMessage = document.createElement('div');
                noSubthemesMessage.className = 'text-center my-3';
                noSubthemesMessage.innerHTML = html;
                subthemeList.appendChild(noSubthemesMessage);
                return;
            }

            data.forEach((subtheme: any) => {
                const listItem = document.createElement('a');
                listItem.classList.add('list-group-item', 'list-group-item-action', 'd-flex', 'justify-content-between', 'align-items-center');

                const textSpan = document.createElement('span');
                textSpan.textContent = subtheme.title;
                textSpan.ariaValueText = subtheme.subthemeId.toString();
                listItem.appendChild(textSpan);

                const buttonContainer = document.createElement('div');
                buttonContainer.className = 'btn-group';

                const deleteButton = document.createElement('button');
                deleteButton.className = 'btn btn-danger ms-2';
                deleteButton.textContent = 'Verwijderen';
                deleteButton.addEventListener('click', () => {
                    const listItem = deleteButton.closest('.list-group-item');
                    if (!listItem) {
                        console.error('Parent list item not found');
                        return;
                    }

                    startDeleteSubtheme(subtheme.subthemeId);
                    listItem.remove();
                });

                const editButton = document.createElement('button');
                editButton.className = 'btn btn-primary';
                editButton.textContent = 'Bewerken';
                editButton.addEventListener('click', () => {
                    window.location.href = `/Subtheme/Edit/${subtheme.subthemeId}`;
                });
                buttonContainer.appendChild(editButton);
                buttonContainer.appendChild(deleteButton);

                listItem.appendChild(buttonContainer); 
                subthemeList.appendChild(listItem);
            });
        })
        .catch(error => {
            console.error('Fetch error:', error);
        });
}


export async function loadSubthemesIntoSelectbox(flowId: number) {
    const selectbox = document.getElementById('subthemeSelectbox') as HTMLSelectElement;
    if (!selectbox) return;

    // Verwijder elke optie uit de selectbox
    for (let i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }

    const addedOptions = new Set<string>(); // Houd unieke opties bij

    try {
        const response = await fetch(`/api/Subthemes/flow/${flowId}`);
        const subthemes: SubthemeDto[] = await response.json();

        if (subthemes.length === 0) {
            const option = document.createElement('option');
            option.value = '';
            option.textContent = 'Maak eerst een subthema';
            selectbox.appendChild(option);
        } else {
            subthemes.forEach(subtheme => {
                // Controleer of de optie al is toegevoegd
                if (!addedOptions.has(subtheme.title)) {
                    const option = document.createElement('option');
                    option.value = subtheme.title;
                    option.textContent = subtheme.title;
                    option.setAttribute('data-subthemeId', subtheme.subthemeId.toString());
                    selectbox.appendChild(option);
                    addedOptions.add(subtheme.title); // Voeg de optie toe aan de set van toegevoegde opties
                }
            });
        }
    } catch (error) {
        console.error('Error loading subthemes:', error);
    }
}











