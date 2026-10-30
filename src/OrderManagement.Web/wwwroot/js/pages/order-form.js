import { getJson, postJson } from '../modules/http.js';
import { debounce } from '../modules/debounce.js';
import { isFormValid, setFieldError, showErrors } from '../modules/validation.js';

const form = document.getElementById('order-form');
const itemRows = document.getElementById('item-rows');
const rowTemplate = document.getElementById('item-row-template');
const formErrors = document.getElementById('form-errors');
const discountType = document.getElementById('discountType');
const discountValue = document.getElementById('discountValue');
const submitButton = form.querySelector('button[type="submit"]');
const existingOrder = document.getElementById('order-data');

let isSubmitting = false;
const summary = {
    subTotal: document.getElementById('summary-subtotal'),
    discount: document.getElementById('summary-discount'),
    total: document.getElementById('summary-total')
};

const formatMoney = amount =>
    amount.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });

function readItems() {
    return [...itemRows.rows].map(row => ({
        productName: row.querySelector('[name="productName"]').value.trim(),
        quantity: Number(row.querySelector('[name="quantity"]').value),
        unitPrice: Number(row.querySelector('[name="unitPrice"]').value)
    }));
}

const lineTotal = item => item.quantity * item.unitPrice;

function updateLineTotals(items) {
    [...itemRows.rows].forEach((row, index) => {
        row.querySelector('[data-line-total]').textContent = formatMoney(lineTotal(items[index]));
    });
}

function subTotalOf(items) {
    const subTotal = items.reduce((sum, item) => sum + lineTotal(item), 0);

    return Math.round(subTotal * 100) / 100;
}

function showSummary(subTotal, discountAmount) {
    summary.subTotal.textContent = formatMoney(subTotal);
    summary.discount.textContent = discountAmount > 0 ? `-${formatMoney(discountAmount)}` : formatMoney(0);
    summary.total.textContent = formatMoney(subTotal - discountAmount);
}

async function applyDiscount() {
    const items = readItems();
    updateLineTotals(items);

    const subTotal = subTotalOf(items);
    const hasValue = discountType.value !== 'None';

    discountValue.disabled = !hasValue;
    discountValue.required = hasValue;

    if (!hasValue) {
        discountValue.value = '';
    }

    if (hasValue && discountValue.value === '') {
        setFieldError(discountValue, '');
        showSummary(subTotal, 0);
        return;
    }

    const query = new URLSearchParams({
        handler: 'Discount',
        discountType: discountType.value,
        discountValue: discountValue.value || '0',
        subTotal: subTotal.toFixed(2)
    });

    const { ok, data } = await getJson(`?${query}`);

    setFieldError(discountValue, ok ? '' : data.errors?.join(' ') ?? 'Could not calculate the discount.');
    showSummary(subTotal, ok ? data.discountAmount : 0);
}

const applyDiscountDebounced = debounce(applyDiscount, 300);

function addItemRow(item) {
    const row = rowTemplate.content.firstElementChild.cloneNode(true);

    if (item) {
        row.querySelector('[name="productName"]').value = item.productName;
        row.querySelector('[name="quantity"]').value = item.quantity;
        row.querySelector('[name="unitPrice"]').value = item.unitPrice;
    }

    itemRows.append(row);

    return row;
}

document.getElementById('add-item').addEventListener('click', () => {
    addItemRow().querySelector('[name="productName"]').focus();
});

itemRows.addEventListener('input', () => {
    updateLineTotals(readItems());
    applyDiscountDebounced();
});

itemRows.addEventListener('click', event => {
    const removeButton = event.target.closest('[data-remove-item]');
    if (removeButton) {
        removeButton.closest('tr').remove();
        applyDiscount();
    }
});

discountType.addEventListener('change', applyDiscount);
discountValue.addEventListener('input', applyDiscountDebounced);

form.addEventListener('submit', async event => {
    event.preventDefault();

    if (isSubmitting) {
        return;
    }

    showErrors(formErrors, []);

    if (itemRows.rows.length === 0) {
        showErrors(formErrors, ['Add at least one item.']);
        return;
    }

    if (!isFormValid(form)) {
        return;
    }

    isSubmitting = true;
    submitButton.disabled = true;

    const order = {
        discountType: discountType.value,
        discountValue: Number(discountValue.value) || 0,
        items: readItems()
    };

    if (form.elements.customerId) {
        order.customerId = Number(form.elements.customerId.value);
    }

    const { ok, data } = await postJson(window.location.pathname, order);

    if (ok) {
        window.location.href = data.redirectUrl;
        return;
    }

    showErrors(formErrors, data.errors ?? ['Something went wrong. Please try again.']);
    isSubmitting = false;
    submitButton.disabled = false;
});

if (existingOrder) {
    const order = JSON.parse(existingOrder.textContent);
    order.items.forEach(addItemRow);
    discountType.value = order.discountType;
    discountValue.value = order.discountType === 'None' ? '' : order.discountValue;
} else {
    addItemRow();
}

applyDiscount();
