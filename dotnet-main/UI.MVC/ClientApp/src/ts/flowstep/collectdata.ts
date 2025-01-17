import {Option} from "../Configs/AddFlowConfig/flowstepDto";
import {flowIdNumber} from "./flowsteplist";
import {postOnlyFile} from "./addflowstep";
import {max} from "@popperjs/core/lib/utils/math";


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

    return {
        orderNr,
        subthemeId,
        query,
        options,
        flowStepType: 'ClosedQuestion',
        flow: flowIdNumber
    };
}

export async function collectInfoData(orderNr: number, subthemeId: string | undefined) {
    const body = (document.getElementById('infoBody') as HTMLTextAreaElement).value;
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

    return {
        orderNr,
        subthemeId,
        body,
        uploadedImage,
        uploadedVideo,
        uploadedAudio,
        flowStepType: 'Info',
        flow: flowIdNumber
    };
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

    return {
        orderNr,
        subthemeId,
        query,
        options,
        flowStepType: 'MultipleChoiceQuestion',
        flow: flowIdNumber
    };
}

export async function collectRangeQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('query') as HTMLInputElement).value;
    let minValue = parseInt((document.getElementById('minValue') as HTMLInputElement).value);
    let maxValue = parseInt((document.getElementById('maxValue') as HTMLInputElement).value);

    if (isNaN(minValue))
    {
        minValue = 0;
        
    }

    if (isNaN(maxValue))
    {
        maxValue = 0;

    }
    return {
        orderNr,
        subthemeId,
        query,
        minValue,
        maxValue,
        flowStepType: 'RangeQuestion',
        flow: flowIdNumber
    };
}

export async function collectOpenQuestionData(orderNr: number, subthemeId: string | undefined) {
    const query = (document.getElementById('openQuestion') as HTMLInputElement).value;
console.log(flowIdNumber);
    return {
        orderNr,
        subthemeId,
        query,
        flowStepType: 'OpenQuestion',
        flow: flowIdNumber
    };
}