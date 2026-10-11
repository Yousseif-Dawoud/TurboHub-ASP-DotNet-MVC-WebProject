document.addEventListener('DOMContentLoaded', function () {

    // Toggle password visibility
    const passwordInput = document.getElementById('Password');
    const toggleBtn = document.getElementById('togglePassword');

    if (toggleBtn && passwordInput) {
        toggleBtn.addEventListener('click', function () {
            const type = passwordInput.type === 'password' ? 'text' : 'password';
            passwordInput.type = type;
            toggleBtn.textContent = type === 'password' ? '👁' : '🙈';
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