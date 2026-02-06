const ContactManager = {
    config: {
        tableId: 'managersTable',
        endpoints: { edit: '', delete: '' },
        currentEditCell: null,
        filterTimeout: null,
        deleteModal: null,
        pendingDeleteId: null,
        pendingDeleteRow: null
    },

    formatters: {
        isMarried: v => (String(v) === 'true' ? 'Yes' : 'No'),
        salary: v => (v !== null && v !== '') ? parseFloat(v).toLocaleString('uk-UA', { minimumFractionDigits: 2 }) : '',
        dateOfBirth: v => new Date(v).toLocaleDateString(),
        default: v => v
    },

    parsers: {
        isMarried: (input) => input.value === 'true',
        salary: (input) => {
            const val = input.value.replace(/\s/g, '').replace(',', '.');
            if (val === '') return null;
            if (isNaN(val)) throw new Error('Please enter a valid number');
            return parseFloat(val);
        },
        default: (input) => input.value
    },

    init(endpoints) {
        this.config.endpoints = endpoints;
        const table = document.getElementById(this.config.tableId);

        const modalEl = document.getElementById('deleteModal');
        if (modalEl && window.bootstrap) {
            this.config.deleteModal = new bootstrap.Modal(modalEl);

            document.getElementById('confirmDeleteBtn')?.addEventListener('click', () => {
                this.executeDelete();
            });
        }

        if (!table) return;

        table.addEventListener('click', (e) => {
            const target = e.target;
            const th = target.closest('th');
            const editable = target.closest('.editable');
            const deleteBtn = target.closest('.delete-btn');

            if (deleteBtn) {
                this.showDeleteModal(deleteBtn);
            } else if (th?.dataset.sortable === 'true') {
                this.sort(th.cellIndex, th);
            } else if (editable && !target.closest('.edit-input')) {
                this.startEdit(editable);
            }
        });

        table.querySelector('.filter-row')?.addEventListener('input', (e) => {
            if (e.target.classList.contains('filter-input')) {
                clearTimeout(this.config.filterTimeout);
                this.config.filterTimeout = setTimeout(() => this.filterByColumn(), 300);
            }
        });
    },

    filterByColumn() {
        const table = document.getElementById(this.config.tableId);
        const filters = Array.from(table.querySelectorAll('.filter-input'));
        const rows = table.querySelectorAll('tbody tr');

        const activeFilters = filters
            .map(f => ({ col: parseInt(f.dataset.column), val: f.value.toLowerCase() }))
            .filter(f => f.val);

        rows.forEach(row => {
            const isVisible = activeFilters.every(({ col, val }) => {
                const cell = row.cells[col];
                const content = cell.dataset.field === 'isMarried'
                    ? (cell.dataset.value === 'true' ? 'yes' : 'no')
                    : cell.textContent.trim().toLowerCase();
                return content.includes(val);
            });
            row.style.display = isVisible ? '' : 'none';
        });
    },

    sort(index, th) {
        const tbody = document.getElementById(this.config.tableId).tBodies[0];
        const rows = Array.from(tbody.rows);
        const dir = th.dataset.sortDir === 'asc' ? 'desc' : 'asc';

        document.querySelectorAll('th').forEach(h => delete h.dataset.sortDir);
        th.dataset.sortDir = dir;

        const mult = dir === 'asc' ? 1 : -1;
        rows.sort((a, b) => {
            const vA = this.getCellValue(a.cells[index]);
            const vB = this.getCellValue(b.cells[index]);
            return (vA === vB ? 0 : (vA > vB ? 1 : -1)) * mult;
        });

        tbody.append(...rows);
    },

    getCellValue(cell) {
        const v = cell.dataset.value;
        const n = parseFloat(v);
        return isNaN(n) ? v.toLowerCase() : n;
    },

    startEdit(cell) {
        const existingInput = cell.querySelector('.edit-input');
        if (existingInput) {
            existingInput.focus();
            if (cell.dataset.field !== 'isMarried') existingInput.select?.();
            return;
        }

        if (this.config.currentEditCell && this.config.currentEditCell !== cell) {
            this.finishEdit(this.config.currentEditCell);
        }

        const { field, value } = cell.dataset;
        cell.querySelector('.display-value').style.display = 'none';

        const input = this.createInputElement(field, value);
        cell.appendChild(input);

        setTimeout(() => {
            input.focus();
            if (input.tagName === 'INPUT' && input.type !== 'date') {
                input.select();
            }
        }, 0);

        this.config.currentEditCell = cell;
    },

    createInputElement(field, value) {
        let input;

        if (field === 'isMarried') {
            input = document.createElement('select');
            input.classList.add('form-select', 'form-select-sm', 'edit-input');

            const optYes = new Option('Yes', 'true');
            const optNo = new Option('No', 'false');

            input.add(optYes);
            input.add(optNo);

            input.value = value === 'true' ? 'true' : 'false';

            input.addEventListener('change', () => input.blur());
        }
        else {
            input = document.createElement('input');
            input.classList.add('form-control', 'form-control-sm', 'edit-input');

            switch (field) {
                case 'dateOfBirth':
                    input.type = 'date';
                    input.value = value;
                    break;
                case 'salary':
                    input.type = 'text';
                    input.value = value ? parseFloat(value).toFixed(2).replace('.', ',') : '';
                    input.addEventListener('input', () => input.value = input.value.replace(/[^0-9.,]/g, ''));
                    break;
                default:
                    input.type = 'text';
                    input.value = value;
            }
        }

        input.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') { e.preventDefault(); input.blur(); }
            if (e.key === 'Escape') { e.preventDefault(); this.cancelEdit(input.closest('td')); }
        });

        input.addEventListener('blur', () => {
            setTimeout(() => {
                if (document.body.contains(input)) this.finishEdit(input.closest('td'));
            }, 100);
        });

        return input;
    },

    async finishEdit(cell) {
        if (!cell) return;
        const input = cell.querySelector('.edit-input');
        if (!input) return;

        const { field, value: oldVal } = cell.dataset;
        let newVal;

        try {
            const parser = this.parsers[field] || this.parsers.default;
            newVal = parser(input);
        } catch (error) {
            this.showError(cell, error.message);
            input.focus();
            return;
        }

        cell.dataset.value = newVal === null ? '' : newVal;

        const data = this.collectData(cell.closest('tr'));
        this.clearErrors(cell);

        try {
            const res = await fetch(this.config.endpoints.edit, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });

            if (res.ok) {
                this.cleanupCell(cell, newVal);
            } else {
                cell.dataset.value = oldVal;
                const err = await res.json();
                this.showError(cell, err.errors || err);
                input.focus();
            }
        } catch (e) {
            cell.dataset.value = oldVal;
            alert('Network error');
            this.cancelEdit(cell);
        }
    },

    cleanupCell(cell, newVal) {
        cell.querySelector('.edit-input')?.remove();
        const display = cell.querySelector('.display-value');

        const formatter = this.formatters[cell.dataset.field] || this.formatters.default;
        display.textContent = formatter(newVal);

        display.style.display = '';
        this.clearErrors(cell);

        if (this.config.currentEditCell === cell) {
            this.config.currentEditCell = null;
        }
    },

    collectData(row) {
        const data = { Id: parseInt(row.dataset.id) };
        row.querySelectorAll('.editable').forEach(cell => {
            const { field, value } = cell.dataset;
            if (field === 'salary') data[field] = value === '' ? null : parseFloat(value);
            else if (field === 'isMarried') data[field] = value === 'true';
            else data[field] = value;
        });
        return data;
    },

    showError(cell, errorContent) {
        this.clearErrors(cell);
        cell.querySelector('.edit-input')?.classList.add('is-invalid');

        const div = document.createElement('div');
        div.className = 'invalid-feedback d-block';

        let msg = typeof errorContent === 'string' ? errorContent : 'Error';
        if (typeof errorContent === 'object') {
            const field = cell.dataset.field.toLowerCase();
            const key = Object.keys(errorContent).find(k => k.toLowerCase().includes(field));
            if (key) msg = Array.isArray(errorContent[key]) ? errorContent[key][0] : errorContent[key];
        }

        div.textContent = msg;
        cell.appendChild(div);
    },

    clearErrors(cell) {
        cell.querySelector('.is-invalid')?.classList.remove('is-invalid');
        cell.querySelector('.invalid-feedback')?.remove();
    },

    cancelEdit(cell) {
        const field = cell.dataset.field;
        let val = cell.dataset.value;
        if (field === 'salary' && val !== '') val = parseFloat(val);
        this.cleanupCell(cell, val);
    },

    showDeleteModal(btn) {
        const row = btn.closest('tr');
        const id = parseInt(row.dataset.id);

        const nameCell = row.querySelector('[data-field="name"]');
        const name = nameCell ? nameCell.dataset.value : 'this item';

        this.config.pendingDeleteId = id;
        this.config.pendingDeleteRow = row;

        document.getElementById('deleteTargetName').textContent = name;
        this.config.deleteModal?.show();
    },

    async executeDelete() {
        if (!this.config.pendingDeleteId) return;

        const id = this.config.pendingDeleteId;
        const btn = document.getElementById('confirmDeleteBtn');

        try {
            btn.disabled = true;
            btn.textContent = "Deleting...";

            const res = await fetch(this.config.endpoints.delete, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(id)
            });

            if (res.ok) {
                this.config.pendingDeleteRow.remove();
                this.config.deleteModal.hide();
            } else {
                alert('Error deleting record on server');
            }
        } catch (e) {
            alert('Network error occurred');
        } finally {
            btn.disabled = false;
            btn.textContent = "Yes, Delete it!";
            this.config.pendingDeleteId = null;
            this.config.pendingDeleteRow = null;
        }
    }
};