// transactions-index.js
$(function () {
    // Toggle advanced filters (if any)
    $('#toggleAdvancedFilters').on('click', function () {
        $('#advancedFilters').toggleClass('show');
    });

    // Pagination and filtering state
    let transactionsData = window.transactionsData || [];
    let currentPage = 1;
    const pageSize = 10;

    // Add Type filter toggle buttons above the table
    const typeFilterHtml = `
        <div class="btn-group mb-3" role="group" aria-label="Type Filter">
            <button type="button" class="btn btn-outline-light type-filter-btn active" data-type="all" title="All"><i class="bi bi-list"></i> All</button>
            <button type="button" class="btn btn-outline-success type-filter-btn" data-type="Income" title="Income"><i class="bi bi-arrow-down-circle"></i> Income</button>
            <button type="button" class="btn btn-outline-danger type-filter-btn" data-type="Expense" title="Expense"><i class="bi bi-arrow-up-circle"></i> Expense</button>
        </div>
    `;
    if (!document.getElementById('typeFilterGroup')) {
        const filterGroup = document.createElement('div');
        filterGroup.id = 'typeFilterGroup';
        filterGroup.innerHTML = typeFilterHtml;
        const table = document.getElementById('transactionsTable');
        table.parentNode.insertBefore(filterGroup, table);
    }

    let typeFilter = 'all';
    $(document).on('click', '.type-filter-btn', function () {
        $('.type-filter-btn').removeClass('active');
        $(this).addClass('active');
        typeFilter = $(this).data('type');
        currentPage = 1;
        renderTransactionsTable();
        renderPagination();
    });

    // Filter transactions on the client side
    function filterTransactions() {
        const search = $('#searchInput').val()?.toLowerCase() || '';
        const category = $('#categoryInput').val();
        const account = $('#accountInput').val();
        const currency = $('#currencyInput').val();
        return transactionsData.filter(tx => {
            let match = true;
            if (search && !(tx.Description?.toLowerCase().includes(search) || (tx.Account?.Name?.toLowerCase().includes(search)))) match = false;
            if (category && tx.Category !== category) match = false;
            if (account && (!tx.Account || tx.Account.Name !== account)) match = false;
            if (currency && tx.Account.Currency !== currency) match = false;
            if (typeFilter !== 'all' && tx.Type !== typeFilter) match = false;
            return match;
        });
    }

    // Render table rows
    function renderTransactionsTable() {
        const tbody = document.querySelector('#transactionsTable tbody');
        tbody.innerHTML = '';
        const filtered = filterTransactions();
        const start = (currentPage - 1) * pageSize;
        const end = start + pageSize;
        const pageData = filtered.slice(start, end);
        for (const tx of pageData) {
            const tr = document.createElement('tr');
            // Add class for row type
            if (tx.Type === 'Income') {
                tr.classList.add('income-row');
            } else if (tx.Type === 'Expense') {
                tr.classList.add('expense-row');
            }
            tr.innerHTML = `
                <td><input type="checkbox" data-transaction-id="${tx.TransactionID}" /></td>
                <td>${tx.TransactionID}</td>
                <td class="type-cell ${tx.Type === 'Income' ? 'neon-green-text' : tx.Type === 'Expense' ? 'neon-red-text' : ''}">${tx.Type}</td>
                <td>${tx.Amount}</td>
                <td>${tx.Account ? tx.Account.Name : ''}</td>
                <td>${tx.Category}</td>
                <td>${tx.Account.Currency ?? ''}</td>
                <td>${formatDateTime(tx.Date)}</td>
            `;
            tbody.appendChild(tr);
        }
    }

    // Add a helper function for formatting date and time
    function formatDateTime(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        if (isNaN(date)) return dateString;
        // Format: yyyy-MM-dd HH:mm (24h)
        const yyyy = date.getFullYear();
        const mm = String(date.getMonth() + 1).padStart(2, '0');
        const dd = String(date.getDate()).padStart(2, '0');
        const hh = String(date.getHours()).padStart(2, '0');
        const min = String(date.getMinutes()).padStart(2, '0');
        return `${yyyy}-${mm}-${dd} ${hh}:${min}`;
    }

    // Inject neon CSS if not present
    if (!document.getElementById('neon-tx-style')) {
        const style = document.createElement('style');
        style.id = 'neon-tx-style';
        style.textContent = `
            .neon-green-text {
                color: #39ff14 !important;
                text-shadow: 0 0 4px #39ff14, 0 0 8px #39ff14;
                font-weight: bold;
            }
            .neon-red-text {
                color: #ff073a !important;
                text-shadow: 0 0 4px #ff073a, 0 0 8px #ff073a;
                font-weight: bold;
            }
            .income-row:hover {
                background: #39ff14 !important;
                color: #111 !important;
                box-shadow: 0 0 10px #39ff14, 0 0 20px #39ff14;
            }
            .expense-row:hover {
                background: #ff073a !important;
                color: #fff !important;
                box-shadow: 0 0 10px #ff073a, 0 0 20px #ff073a;
            }
            .income-row:hover .type-cell {
                /* Remove color override so neon stays */
                color: #39ff14 !important;
                text-shadow: 0 0 4px #39ff14, 0 0 8px #39ff14;
            }
            .expense-row:hover .type-cell {
                /* Remove color override so neon stays */
                color: #ff073a !important;
                text-shadow: 0 0 4px #ff073a, 0 0 8px #ff073a;
            }
            .table-striped tbody tr:nth-of-type(odd) {
                background-color: #222 !important;
                color: #fff !important;
            }
        `;
        document.head.appendChild(style);
    }

    // Show operation toast (success or error)
    function showOperationToast(success, message) {
        let toast = document.getElementById('operationToast');
        let body = document.getElementById('operationToastBody');
        if (!toast || !body) {
            alert(message);
            return;
        }
        toast.classList.remove('text-bg-success', 'text-bg-danger', 'text-bg-warning');
        if (success) {
            toast.classList.add('text-bg-success');
        } else {
            toast.classList.add('text-bg-danger');
        }
        body.textContent = message;
        new bootstrap.Toast(toast).show();
    }

    // Toolbar Edit button logic
    $('#editTransactionBtn').on('click', function () {
        const checked = document.querySelectorAll('#transactionsTable tbody input[type="checkbox"]:checked');
        if (checked.length !== 1) {
            const toast = new bootstrap.Toast(document.getElementById('editSelectionToast'));
            toast.show();
            return;
        }
        const transactionId = checked[0].dataset.transactionId;
        const tx = transactionsData.find(t => t.TransactionID == transactionId);
        if (!tx) return;
        document.getElementById('editTransactionId').value = tx.TransactionID;
        document.getElementById('editTransactionType').value = tx.Type;
        document.getElementById('editTransactionAmount').value = tx.Amount;
        // Populate account dropdown with AccountID as value using accountsList from view model
        const accountSelect = document.getElementById('editTransactionAccount');
        accountSelect.innerHTML = '';
        (window.accountsList || []).forEach(acc => {
            const opt = document.createElement('option');
            opt.value = acc.AccountID;
            opt.textContent = acc.Name;
            if (tx.Account && acc.AccountID == tx.Account.AccountID) opt.selected = true;
            accountSelect.appendChild(opt);
        });
        // Populate category dropdown
        const categorySelect = document.getElementById('editTransactionCategory');
        categorySelect.innerHTML = '';
        (window.categoriesList || []).forEach(cat => {
            const opt = document.createElement('option');
            opt.value = cat;
            opt.textContent = cat;
            if (cat === tx.Category) opt.selected = true;
            categorySelect.appendChild(opt);
        });
        const dateInput = document.getElementById('editTransactionDate');
        if (tx.Date) {
            const d = new Date(tx.Date);
            if (!isNaN(d)) {
                // For input type="datetime-local" or fallback to yyyy-MM-dd
                if (dateInput.type === 'datetime-local') {
                    const yyyy = d.getFullYear();
                    const mm = String(d.getMonth() + 1).padStart(2, '0');
                    const dd = String(d.getDate()).padStart(2, '0');
                    const hh = String(d.getHours()).padStart(2, '0');
                    const min = String(d.getMinutes()).padStart(2, '0');
                    dateInput.value = `${yyyy}-${mm}-${dd}T${hh}:${min}`;
                } else {
                    const yyyy = d.getFullYear();
                    const mm = String(d.getMonth() + 1).padStart(2, '0');
                    const dd = String(d.getDate()).padStart(2, '0');
                    dateInput.value = `${yyyy}-${mm}-${dd}`;
                }
            } else {
                dateInput.value = tx.Date;
            }
        } else {
            dateInput.value = '';
        }
        const editModal = new bootstrap.Modal(document.getElementById('editTransactionModal'));
        editModal.show();
    });

    // Add Transaction button logic
    $('#addTransactionBtn').on('click', function () {
        // Reset form fields
        document.getElementById('addTransactionType').value = 'Income';
        document.getElementById('addTransactionAmount').value = '';
        // Populate account dropdown with AccountID as value using accountsList from view model
        const accountSelect = document.getElementById('addTransactionAccount');
        accountSelect.innerHTML = '';
        (window.accountsList || []).forEach(acc => {
            const opt = document.createElement('option');
            opt.value = acc.AccountID;
            opt.textContent = acc.Name;
            accountSelect.appendChild(opt);
        });
        // Populate category dropdown
        const categorySelect = document.getElementById('addTransactionCategory');
        categorySelect.innerHTML = '';
        (window.categoriesList || []).forEach(cat => {
            const opt = document.createElement('option');
            opt.value = cat;
            opt.textContent = cat;
            categorySelect.appendChild(opt);
        });
        // Set default date to today in yyyy-MM-dd format
        const today = new Date();
        const yyyy = today.getFullYear();
        const mm = String(today.getMonth() + 1).padStart(2, '0');
        const dd = String(today.getDate()).padStart(2, '0');
        document.getElementById('addTransactionDate').value = `${yyyy}-${mm}-${dd}`;
        const addModal = new bootstrap.Modal(document.getElementById('addTransactionModal'));
        addModal.show();
    });

    // Toolbar Delete button logic
    $('#deleteTransactionsBtn').on('click', function () {
        const checked = document.querySelectorAll('#transactionsTable tbody input[type="checkbox"]:checked');
        if (checked.length === 0) return;
        const list = document.getElementById('transactionsToDeleteList');
        list.innerHTML = '';
        checked.forEach(cb => {
            const tx = transactionsData.find(t => t.TransactionID == cb.dataset.transactionId);
            if (tx) {
                const li = document.createElement('li');
                li.className = 'list-group-item bg-dark text-light border-secondary';
                li.textContent = `${tx.Type} - ${tx.Amount} (${tx.Account ? tx.Account.Name : ''}) [${tx.Category}] on ${formatDateTime(tx.Date)}`;
                list.appendChild(li);
            }
        });
        document.getElementById('deleteTransactionsForm').onsubmit = async function(e) {
            e.preventDefault();
            const idsToDelete = Array.from(checked).map(cb => parseInt(cb.dataset.transactionId));
            // Call backend to delete
            try {
                const response = await fetch(window.location.pathname + '?handler=Remove', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': $("input[name='__RequestVerificationToken']").val() || ''
                    },
                    body: JSON.stringify(idsToDelete)
                });
                const result = await response.json();
                if (result.success) {
                    transactionsData = transactionsData.filter(t => !idsToDelete.includes(t.TransactionID));
                    renderTransactionsTable();
                    renderPagination();
                    showOperationToast(true, result.message || 'Deleted successfully.');
                } else {
                    showOperationToast(false, result.message || 'Delete failed.');
                }
            } catch (err) {
                showOperationToast(false, 'Delete failed.');
            }
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteTransactionsModal'));
            modal.hide();
        };
        const deleteModal = new bootstrap.Modal(document.getElementById('deleteTransactionsModal'));
        deleteModal.show();
    });

    // Render pagination
    function renderPagination() {
        const totalPages = Math.ceil(filterTransactions().length / pageSize);
        const pagination = document.getElementById('transactionsPagination');
        pagination.innerHTML = '';
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = 'page-item' + (i === currentPage ? ' active' : '');
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.addEventListener('click', function (e) {
                e.preventDefault();
                currentPage = i;
                renderTransactionsTable();
                renderPagination();
            });
            pagination.appendChild(li);
        }
    }

    // Filter form events
    $('#searchInput, #categoryInput, #accountInput, #currencyInput').on('input change', function () {
        currentPage = 1;
        renderTransactionsTable();
        renderPagination();
    });

    // Initial load
    renderTransactionsTable();
    renderPagination();

    // Print logic
    $('#printBtn').on('click', function () {
        const checkboxes = $('#transactionsTable tbody input[type="checkbox"]:checked');
        let rowsToPrint = [];
        if (checkboxes.length > 0) {
            checkboxes.each(function () {
                rowsToPrint.push($(this).closest('tr')[0].outerHTML);
            });
        } else {
            $('#transactionsTable tbody tr').each(function () {
                rowsToPrint.push(this.outerHTML);
            });
        }
        const printWindow = window.open('', '', 'width=900,height=700');
        printWindow.document.write('<html><head><title>Print Transactions</title>');
        printWindow.document.write('<link rel="stylesheet" href="/lib/bootstrap/dist/css/bootstrap.min.css" />');
        printWindow.document.write('<style>body{background:#fff!important;color:#222!important;padding:32px;} .print-title{font-size:2rem;font-weight:600;margin-bottom:1.5rem;} .table th,.table td{vertical-align:middle!important;} .table thead th{background:#f8f9fa!important;color:#222;} .table-striped tbody tr:nth-of-type(odd){background-color:#222 !important;color:#fff !important;} .table{border-radius:12px;overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,0.08);} </style>');
        printWindow.document.write('</head><body>');
        printWindow.document.write('<div class="print-title">Transactions List</div>');
        printWindow.document.write('<table class="table table-bordered table-striped table-hover align-middle">');
        printWindow.document.write('<thead>' + document.querySelector('#transactionsTable thead').innerHTML + '</thead>');
        printWindow.document.write('<tbody>' + rowsToPrint.join('') + '</tbody></table></body></html>');
        printWindow.document.close();
        printWindow.print();
    });
});
