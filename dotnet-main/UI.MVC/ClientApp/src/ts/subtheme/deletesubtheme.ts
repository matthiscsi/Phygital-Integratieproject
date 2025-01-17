import {flowId, flowIdNumber,fetchFlowstepsOfFlow} from "../flowstep/flowsteplist";
import {loadSubthemesIntoSelectbox, loadSubthemesOfFlow} from "./loadsubthemes";

export async function startDeleteSubtheme(subthemeId: string) {

    console.log("startDeleteSubtheme");
    await deleteSubtheme(subthemeId);
    loadSubthemesOfFlow(flowIdNumber);
    loadSubthemesIntoSelectbox(flowIdNumber);
    fetchFlowstepsOfFlow(flowId);

}





async function deleteSubtheme(subthemeId: string) {
    try {
        const response = await fetch(`/api/Subthemes/${subthemeId}`, {
            method: 'DELETE'
        });
        if (!response.ok) {
            throw new Error('Error');
        }
   
    } catch (error) {
        console.error('Error deleting subtheme:', error);
    }
}