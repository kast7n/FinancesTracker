// layout.js - handles sidebar toggle and responsive sidebar behavior
$(function () {
    const sidebar = document.getElementById('sidebar');
    const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            sidebar.classList.toggle('show');
        });
    }
    // Hide sidebar on nav click (mobile)
    document.querySelectorAll('.sidebar .nav-link').forEach(link => {
        link.addEventListener('click', () => {
            if (window.innerWidth < 992) sidebar.classList.remove('show');
        });
    });
});
