export function StartDeleteFlowStep(id: number) {
    deleteFlowStep(id);
}

export async function deleteFlowStep(id: number): Promise<void> {
    const url = `/api/Flowsteps/delete/${id}`;

    try {
        const response = await fetch(url, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('Er is een probleem opgetreden bij het verwijderen van de flowstep.');
        }

        console.log('Flowstep succesvol verwijderd.');
    } catch (error) {
        console.error('Er is een fout opgetreden');
    }
}