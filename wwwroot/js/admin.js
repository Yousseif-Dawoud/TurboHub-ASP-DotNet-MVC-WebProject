document.addEventListener('DOMContentLoaded', function () {

    // Highlight active menu item
    const currentPath = window.location.pathname.toLowerCase();
    const menuItems = document.querySelectorAll('.menu-item');

    menuItems.forEach(function (item) {
        const href = item.getAttribute('href');
        if (href && currentPath.includes(href.toLowerCase())) {
            item.classList.add('active');
        }
    });

    // Mobile sidebar toggle
    const toggleBtn = document.getElementById('sidebarToggle');
    const sidebar = document.querySelector('.sidebar');

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.toggle('open');
        });
    }

    // Close sidebar when clicking outside on mobile
    document.addEventListener('click', function (e) {
        if (sidebar && window.innerWidth <= 768) {
            if (!sidebar.contains(e.target) && e.target !== toggleBtn) {
                sidebar.classList.remove('open');
            }
        }
    });

    // Auto dismiss alerts
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.classList.remove('show');
        }, 4000);
    });

});