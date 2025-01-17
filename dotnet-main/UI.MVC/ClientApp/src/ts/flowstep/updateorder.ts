import {fetchFlowstepsOfFlow, flowId} from "./flowsteplist";

export async function updateOrder() {
    const flowStepList = document.getElementById('flowStepList') as HTMLUListElement;
    const flowSteps = Array.from(flowStepList.children);

    const order = flowSteps.map((flowStep, index) => {
        const spanElement = flowStep.querySelector('span');
        let id = null;
        if (spanElement) {
            id = spanElement.getAttribute('data-flowstepid');
        }
        return {
            id: id,
            orderNr: index + 1
        };
    });

    for (const flowStep of order) {
        try {
            const response = await fetch(`/api/Flowsteps/${flowStep.id}/order/${flowStep.orderNr}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
            });
            if (!response.ok) {
                throw new Error('Failed to update order');
            }
        } catch (error) {
            console.error('Error updating order:', error);
        }
    }

    await fetchFlowstepsOfFlow(flowId);
}