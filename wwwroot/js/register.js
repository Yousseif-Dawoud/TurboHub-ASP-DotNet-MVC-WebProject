document.addEventListener('DOMContentLoaded', function () {

    // Toggle password visibility
    const passwordInput = document.getElementById('Password');
    const confirmInput = document.getElementById('ConfirmPassword');
    const togglePass = document.getElementById('togglePassword');
    const toggleConfirm = document.getElementById('toggleConfirm');

    if (togglePass && passwordInput) {
        togglePass.addEventListener('click', function () {
            const type = passwordInput.type === 'password' ? 'text' : 'password';
            passwordInput.type = type;
            togglePass.textContent = type === 'password' ? '👁' : '🙈';
        });
    }

    if (toggleConfirm && confirmInput) {
        toggleConfirm.addEventListener('click', function () {
            const type = confirmInput.type === 'password' ? 'text' : 'password';
            confirmInput.type = type;
            toggleConfirm.textContent = type === 'password' ? '👁' : '🙈';
        });
    }

    // Auto dismiss alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.remove('show');
        }, 5000);
    });

});