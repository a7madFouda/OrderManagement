export function isFormValid(form) {
    form.classList.add('was-validated');

    return form.checkValidity();
}

export function setFieldError(input, message) {
    const feedback = input.parentElement.querySelector('.invalid-feedback');
    feedback.dataset.defaultMessage ??= feedback.textContent;
    feedback.textContent = message || feedback.dataset.defaultMessage;

    input.setCustomValidity(message);
    input.classList.toggle('is-invalid', message !== '');
}

export function showErrors(container, errors) {
    container.replaceChildren(...errors.map(error => {
        const line = document.createElement('div');
        line.textContent = error;
        return line;
    }));

    container.classList.toggle('d-none', errors.length === 0);
}
