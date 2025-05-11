// accounts-index.js
$(function () {
    // Inject neon CSS if not present (for consistency with transactions)
    if (!document.getElementById('neon-acc-style')) {
        const style = document.createElement('style');
        style.id = 'neon-acc-style';
        style.textContent = `
            .table-bordered th, .table-bordered td {
                border: 1px solid #444 !important;
            }
            .table-striped tbody tr:nth-of-type(odd) {
                background-color: #222 !important;
                color: #fff !important;
            }
            .table-hover tbody tr:hover {
                background: #39ff14 !important;
                color: #111 !important;
                box-shadow: 0 0 10px #39ff14, 0 0 20px #39ff14;
            }
            .table thead th {
                background: #23272b !important;
                color: #fff !important;
                border-bottom: 2px solid #444 !important;
            }
            .table {
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            }
        `;
        document.head.appendChild(style);
    }

    // Toggle advanced filters
    $('#toggleAdvancedFilters').on('click', function () {
        $('#advancedFilters').toggleClass('show');
    });

    // Pagination and filtering state
    let accountsData = window.accountsData || [];
    let currentPage = 1;
    const pageSize = 10;

    // Filter accounts on the client side
    function filterAccounts() {
        const search = $('#searchInput').val()?.toLowerCase() || '';
        const type = $('#typeInput').val();
        const minBalance = parseFloat($('#minBalanceInput').val()) || null;
        const maxBalance = parseFloat($('#maxBalanceInput').val()) || null;
        // Add more filters as needed
        return accountsData.filter(account => {
            let match = true;
            if (search && !(account.Name?.toLowerCase().includes(search) || account.Currency?.toLowerCase().includes(search))) match = false;
            if (type && account.Type !== type) match = false;
            if (minBalance !== null && account.Balance < minBalance) match = false;
            if (maxBalance !== null && account.Balance > maxBalance) match = false;
            return match;
        });
    }

    // Render table rows
    function renderAccountsTable() {
        const tbody = document.querySelector('#accountsTable tbody');
        tbody.innerHTML = '';
        const filtered = filterAccounts();
        const start = (currentPage - 1) * pageSize;
        const end = start + pageSize;
        const pageData = filtered.slice(start, end);
        for (const account of pageData) {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td><input type="checkbox" data-account-id="${account.AccountID}" /></td>
                <td>${account.AccountID}</td>
                <td>${account.Name}</td>
                <td>${account.Balance}</td>
                <td>${account.Currency}</td>
            `;
            tbody.appendChild(tr);
        }
    }

    // Toolbar Edit button logic
    $('#editAccountBtn').on('click', function () {
        const checked = document.querySelectorAll('#accountsTable tbody input[type="checkbox"]:checked');
        if (checked.length !== 1) {
            const toast = new bootstrap.Toast(document.getElementById('editSelectionToast'));
            toast.show();
            return;
        }
        const accountId = checked[0].dataset.accountId;
        const account = accountsData.find(a => a.AccountID == accountId);
        if (!account) return;
        document.getElementById('editAccountId').value = account.AccountID;
        document.getElementById('editAccountName').value = account.Name;
        document.getElementById('editAccountBalance').value = account.Balance;
        // Populate currency dropdown
        const currencySelect = document.getElementById('editAccountCurrency');
        currencySelect.innerHTML = '<option value="LBP">LBP</option><option value="USD">USD</option><option value="EUR">EUR</option>';
        currencySelect.value = account.Currency;
        const editModal = new bootstrap.Modal(document.getElementById('editAccountModal'));
        editModal.show();
    });

    // Toolbar Delete button logic
    $('#deleteAccountsBtn').on('click', function () {
        const checked = document.querySelectorAll('#accountsTable tbody input[type="checkbox"]:checked');
        if (checked.length === 0) return;
        const list = document.getElementById('accountsToDeleteList');
        list.innerHTML = '';
        checked.forEach(cb => {
            const account = accountsData.find(a => a.AccountID == cb.dataset.accountId);
            if (account) {
                const li = document.createElement('li');
                li.className = 'list-group-item bg-dark text-light border-secondary';
                li.textContent = account.Name;
                list.appendChild(li);
            }
        });
        const deleteModal = new bootstrap.Modal(document.getElementById('deleteAccountsModal'));
        deleteModal.show();
    });

    document.getElementById('deleteAccountsForm').addEventListener('submit', function(e) {
        var checked = document.querySelectorAll('#accountsTable tbody input[type="checkbox"]:checked');
        var ids = Array.from(checked).map(cb => cb.dataset.accountId).join(',');
        document.getElementById('deleteAccountIds').value = ids;
        // Do NOT call e.preventDefault() here!
    });

    // Render pagination
    function renderPagination() {
        const totalPages = Math.ceil(filterAccounts().length / pageSize);
        const pagination = document.getElementById('accountsPagination');
        pagination.innerHTML = '';
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === currentPage ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = i;
                renderAccountsTable();
                renderPagination();
            });
            pagination.appendChild(li);
        }
    }

    // Filter form events
    $('#searchInput, #typeInput, #minBalanceInput, #maxBalanceInput').on('input change', function () {
        currentPage = 1;
        renderAccountsTable();
        renderPagination();
    });

    // Initial load
    renderAccountsTable();
    renderPagination();

    // Print logic
    $('#printBtn').on('click', function () {
        const checkboxes = $('#accountsTable tbody input[type="checkbox"]:checked');
        let rowsToPrint = [];
        if (checkboxes.length > 0) {
            checkboxes.each(function () {
                rowsToPrint.push($(this).closest('tr')[0].outerHTML);
            });
        } else {
            $('#accountsTable tbody tr').each(function () {
                rowsToPrint.push(this.outerHTML);
            });
        }
        const printWindow = window.open('', '', 'width=900,height=700');
        printWindow.document.write('<html><head><title>Print Accounts</title>');
        printWindow.document.write('<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />');
        printWindow.document.write('<style>body{background:#fff!important;color:#222!important;padding:32px;} .print-title{font-size:2rem;font-weight:600;margin-bottom:1.5rem;} .table th,.table td{vertical-align:middle!important;} .table thead th{background:#f8f9fa!important;color:#222;} .table-striped tbody tr:nth-of-type(odd){background-color:#f2f2f2;} .table{border-radius:12px;overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,0.08);} </style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write('<div class="print-title">Accounts List</div>');
        printWindow.document.write('<table class="table table-bordered table-striped table-hover align-middle">');
        printWindow.document.write('<thead>' + document.querySelector('#accountsTable thead').innerHTML + '</thead>');
        printWindow.document.write('<tbody>' + rowsToPrint.join('') + '</tbody></table></body></html>');
        printWindow.document.close();
        printWindow.print();
    });

    // Edit button logic
    window.showEditModal = function () {
        const checked = document.querySelectorAll('#accountsTable tbody input[type="checkbox"]:checked');
        if (checked.length !== 1) {
            const toast = new bootstrap.Toast(document.getElementById('editSelectionToast'));
            toast.show();
            return;
        }
        // Load account data and show modal (implement as needed)
        const accountId = checked[0].dataset.accountId;
        // ...fetch and populate modal fields...
        const editModal = new bootstrap.Modal(document.getElementById('editAccountModal'));
        editModal.show();
    }
});
