function updateCharacterCount(inputId: string, maxLength: number): void {
    const inputElement = document.getElementById(inputId) as HTMLInputElement | HTMLTextAreaElement;
    const countElement = document.getElementById(`${inputId}Count`) as HTMLElement;
    if (inputElement && countElement) {
        const currentLength = inputElement.value.length;
        countElement.textContent = `${currentLength}/${maxLength}`;
    }
}

function attachInputListeners(): void {
    const inputFields = [
        {id: 'Name', maxLength: 25},
        {id: 'Email', maxLength: 50},
        {id: 'Password', maxLength: 30},
        {id: 'MainText', maxLength: 100},
        {id: 'Link', maxLength: 50},
        {id: 'flowTitle', maxLength: 50},
        {id: 'flowDesc', maxLength: 200},
        {id: 'themeTitle', maxLength: 50},
        {id: 'themeBody', maxLength: 200},
        {id: 'name', maxLength: 50},
        {id: 'password', maxLength: 30},
        {id: 'projectName', maxLength: 50},
        {id: 'projectDescription', maxLength: 200},
        {id: 'Font', maxLength: 30},
        {id: 'Description', maxLength: 200},
        {id: 'subject', maxLength: 100},
        {id: 'messageToUsers', maxLength: 500}
    ];

    inputFields.forEach(field => {
        const inputElement = document.getElementById(field.id) as HTMLInputElement | HTMLTextAreaElement;
        if (inputElement) {
            inputElement.addEventListener('input', () => updateCharacterCount(field.id, field.maxLength));

            updateCharacterCount(field.id, field.maxLength);
        }
    });
}

document.addEventListener('DOMContentLoaded', function() {
    attachInputListeners();

    const togglePasswordButton = document.getElementById('togglePassword');
    console.log(togglePasswordButton)
    if (togglePasswordButton) {
        togglePasswordButton.addEventListener('click', function () {
            const passwordInput = document.getElementById('Password') as HTMLInputElement;
            const passwordIcon = this.querySelector('i');
            if (passwordInput && passwordIcon) {
                const type = passwordInput.type === 'password' ? 'text' : 'password';
                passwordInput.type = type;
                passwordIcon.classList.toggle('fa-eye');
                passwordIcon.classList.toggle('fa-eye-slash');
            }
        });
    }
});