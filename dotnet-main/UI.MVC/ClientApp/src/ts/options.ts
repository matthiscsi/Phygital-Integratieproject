import {OptionDto} from "./Configs/FlowStepConfig/flowstepDto";
const currentUrl = window.location.href;
const url = new URL(currentUrl);
let flowId = url.searchParams.get('flowid') as string;
if (flowId === null) {
    flowId = url.searchParams.get('FlowId') as string;
}

let flowIdUrl = parseInt(flowId, 10);

document.addEventListener('DOMContentLoaded', async (event) => {
    const maxOptions = 4;
    const optionsContainer = document.getElementById('optionsContainer') as HTMLDivElement | null;
    const addOptionButton = document.getElementById('addOption') as HTMLButtonElement | null;
    const form = document.getElementById("form")
    console.log(form)

    const flowStepIdElement = document.getElementById('flowStepId') as HTMLInputElement | null;
    const flowStepId = flowStepIdElement ? parseInt(flowStepIdElement.value) : null;



    if (!optionsContainer || !addOptionButton || !flowStepId) {
        console.error('Required elements are missing.');
        return;
    }

    let options: OptionDto[] = [];

    try {
        const response = await fetch(`/api/Options/getoptions?questionId=${flowStepId}`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        options = data.map((option: any) => ({
            id: option.id,
            text: option.text
        })) as OptionDto[];
    } catch (error) {
        console.log('There was an error loading the options', error);
    }

    function addInputEventListeners() {
        const optionInputs = document.querySelectorAll<HTMLInputElement>('.option input[type="text"]');
        optionInputs.forEach(optionInput => {
            optionInput.addEventListener('input', function () {
                const optionIndex = options.findIndex(option => option.id.toString() === optionInput.dataset.id);
                if (optionIndex !== -1) {
                    options[optionIndex].text = optionInput.value;
                }
            });
        });
    }

    addInputEventListeners();

    addOptionButton.addEventListener('click', function () {
        if (optionsContainer.children.length < maxOptions) {
            const optionDiv = document.createElement('div');
            optionDiv.className = 'option';

            const optionLabel = document.createElement('label');
            optionLabel.className = 'form-label';
            optionLabel.textContent = 'Optie:';

            const optionInput = document.createElement('input');
            optionInput.type = 'text';
            optionInput.className = 'form-control';
            optionInput.required = true;



            optionDiv.appendChild(optionLabel);
            optionDiv.appendChild(optionInput);

            optionsContainer.appendChild(optionDiv);

            let option: OptionDto = {
                id: -1,
                text: ''
            };
            options.push(option);

            optionInput.addEventListener('input', function () {
                option.text = optionInput.value;
            });

            addInputEventListeners();
        }
    });

    optionsContainer.addEventListener('click', function (event) {
        const target = event.target as HTMLElement;
        if (target.classList.contains('removeOption')) {
            const optionDiv = target.closest('.option') as HTMLDivElement;
            if (optionDiv) {
                const optionInput = optionDiv.querySelector('input[type="text"]') as HTMLInputElement;
                if (optionInput) {
                    const optionValue = optionInput.value;
                    const index = options.findIndex(option => option.text === optionValue);
                    if (index > -1) {
                        options.splice(index, 1);
                    }
                }
                optionDiv.remove();
            }
        }
    });

    if (form) {
        form.addEventListener('submit', async function (event) {
            event.preventDefault();

            const flowStepTypeElement = document.getElementById('flowStepType') as HTMLInputElement | null;
            const flowStepType = flowStepTypeElement ? flowStepTypeElement.value : '';
            const subthemeSelect = document.getElementById('subthemeSelect') as HTMLSelectElement | null;
            const selectedSubthemeId = subthemeSelect ? subthemeSelect.value : '';
            const isActive = (document.getElementById('activeRadio') as HTMLInputElement).checked;

            let isactivebool :boolean;

            if (isActive) {
                isactivebool = true;
            } else {
                isactivebool = false;
            }

            let updatedData: any = {
                isActive: isactivebool,
                SubthemeId: selectedSubthemeId
            };

            if (flowStepType === 'Info') {
                updatedData = await handleInfoFlowStep(updatedData);
            } else if (flowStepType === 'RangeQuestion') {
                updatedData.isActive = isactivebool;
                updatedData.MinValue = (document.getElementById('rangeQuestionMinValue') as HTMLInputElement).valueAsNumber;
                updatedData.MaxValue = (document.getElementById('rangeQuestionMaxValue') as HTMLInputElement).valueAsNumber;
                updatedData.Query = (document.getElementById('Query') as HTMLInputElement).value;
            } else if (['OpenQuestion', 'ClosedQuestion', 'MultipleChoiceQuestion'].includes(flowStepType)) {
                updatedData.isActive = isactivebool;
                updatedData.Query = (document.getElementById('Query') as HTMLInputElement).value;
            }

            await updateFlowStep(flowStepId, updatedData);

            try {
                const response = await fetch(`/api/Options/updateoptions/${flowStepId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(options)
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const data = await response.json();
                console.log('Options successfully updated', data);
                location.reload();
            } catch (error) {
                console.log('There was an error updating the options', error);
            }
        });
    }
});

async function handleInfoFlowStep(updatedData: any) {
    const isActive = (document.getElementById('activeRadio') as HTMLInputElement).checked;
    updatedData.Body = (document.getElementById('infoBody') as HTMLInputElement).value;
    const imageInput = document.getElementById('infoImage') as HTMLInputElement;
    const videoInput = document.getElementById('infoVideo') as HTMLInputElement;
    const audioInput = document.getElementById('infoAudio') as HTMLInputElement;

    let uploadedImage, uploadedVideo, uploadedAudio;

    if (imageInput && imageInput.files && imageInput.files.length > 0) {
        uploadedImage = await postOnlyFile(imageInput.files[0]);
        console.log('Image URL:', uploadedImage);
    }

    if (videoInput && videoInput.files && videoInput.files.length > 0) {
        uploadedVideo = await postOnlyFile(videoInput.files[0]);
        console.log('Video URL:', uploadedVideo);
    }

    if (audioInput && audioInput.files && audioInput.files.length > 0) {
        uploadedAudio = await postOnlyFile(audioInput.files[0]);
        console.log('Audio URL:', uploadedAudio);
    }

    updatedData.isActive = isActive;
    updatedData.UploadedAudio = uploadedAudio;
    updatedData.UploadedImage = uploadedImage;
    updatedData.UploadedVideo = uploadedVideo;

    return updatedData;
}

async function updateFlowStep(flowStepId: number, updatedData: any) {
    try {
        const response = await fetch(`/api/FlowSteps/updateflowstep/${flowStepId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updatedData)
        });
        if (response.ok) {
            console.log('Project updated successfully');
            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-success';
            alertDiv.style.position = 'fixed';
            alertDiv.style.bottom = '20px';
            alertDiv.style.right = '20px';
            alertDiv.textContent = 'Changes Saved';
            document.body.appendChild(alertDiv);

            setTimeout(() => {
                alertDiv.remove();
            }, 20000); 


            setTimeout(() => {
                window.location.href = `Flow/Details?flowid=${flowIdUrl}`;
            }, 3000);
        } else {
            throw new Error('Failed to update project');
        }

        const data = await response.json();
        console.log('FlowStep successfully updated', data);
    } catch (error) {
        console.log('There was an error updating the FlowStep', error);
    }
}

async function postOnlyFile(file: File) {
    try {
        const formData = new FormData();
        formData.append('file', file);
        const response = await fetch('/api/Upload/upload', {
            method: 'POST',
            body: formData
        });
        const data = await response.json();
        console.log('File uploaded successfully, URL:', data.url);
        return data.url;
    } catch (error) {
        console.error('Error uploading file:', error);
    }
}