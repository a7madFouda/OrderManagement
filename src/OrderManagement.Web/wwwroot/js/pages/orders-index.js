import { getHtml, post } from '../modules/http.js';
import { debounce } from '../modules/debounce.js';

const filters = document.getElementById('order-filters');
const ordersTable = document.getElementById('orders-table');
const pageAlert = document.getElementById('page-alert');
const deleteModal = bootstrap.Modal.getOrCreateInstance(document.getElementById('delete-modal'));
const deleteOrderId = document.getElementById('delete-order-id');
const confirmDeleteButton = document.getElementById('confirm-delete');

let currentPage = 1;
let orderIdToDelete = null;

function showAlert(type, message) {
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.textContent = message;
    pageAlert.replaceChildren(alert);
}

async function loadOrders(page = currentPage) {
    currentPage = page;

    const query = new URLSearchParams(new FormData(filters));
    query.set('handler', 'List');
    query.set('page', page);

    const { ok, data } = await getHtml(`?${query}`);

    if (ok) {
        ordersTable.innerHTML = data;
    } else {
        showAlert('danger', 'Could not load the orders. Please try again.');
    }
}

filters.addEventListener('submit', event => event.preventDefault());

filters.elements.search.addEventListener('input', debounce(() => loadOrders(1)));

filters.addEventListener('change', event => {
    if (event.target.name !== 'search') {
        loadOrders(1);
    }
});

ordersTable.addEventListener('click', event => {
    const pageButton = event.target.closest('[data-page]');
    if (pageButton) {
        loadOrders(Number(pageButton.dataset.page));
        return;
    }

    const deleteButton = event.target.closest('[data-delete-order]');
    if (deleteButton) {
        orderIdToDelete = deleteButton.dataset.deleteOrder;
        deleteOrderId.textContent = orderIdToDelete;
        deleteModal.show();
    }
});

confirmDeleteButton.addEventListener('click', async () => {
    confirmDeleteButton.disabled = true;

    const { ok } = await post(`?handler=Delete&id=${orderIdToDelete}`);
    deleteModal.hide();
    confirmDeleteButton.disabled = false;

    if (ok) {
        showAlert('success', `Order #${orderIdToDelete} was deleted.`);
        await loadOrders();

        if (currentPage > 1 && !ordersTable.querySelector('tbody tr')) {
            await loadOrders(currentPage - 1);
        }
    } else {
        showAlert('danger', 'Could not delete the order. Please try again.');
    }
});
