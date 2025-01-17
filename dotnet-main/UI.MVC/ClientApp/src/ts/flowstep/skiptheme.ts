import {getNextStep} from "../flow/flowdoorlopen";


export async function skipSubtheme(flowSessionId: number) {
    try {
        const response = await fetch(`/api/Flowsteps/skipsubtheme/${flowSessionId}`, {
            method: 'GET', 
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (response.ok) {
            console.log("Subtheme skipped successfully");
            await getNextStep(flowSessionId);  
        }
        else {
            console.error("Failed to skip the subtheme");
        }
    } catch (error) {
        console.error("Could not get the next step", error);
    }
}
