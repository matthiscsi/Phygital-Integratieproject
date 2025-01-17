const toggleButton = document.getElementById('toggle-login-details') as HTMLButtonElement;
const loginDetails = document.getElementById('login-details') as HTMLDivElement;

if(toggleButton){
    toggleButton.addEventListener('click', () => {
        if (loginDetails.classList.contains('d-none')) {
            loginDetails.classList.remove('d-none');
            toggleButton.textContent = 'Hide Login Details';
        } else {
            loginDetails.classList.add('d-none');
            toggleButton.textContent = 'Show Login Details';
        }
    });
}
document.addEventListener('DOMContentLoaded', function() {
    const togglePasswordButton1 = document.getElementById('togglePassword1');
    if (togglePasswordButton1) {
        togglePasswordButton1.addEventListener('click', function () {
            const passwordInput = document.querySelector('[id$="Input_Password"]') as HTMLInputElement; // Specificeer het type als HTMLInputElement
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
