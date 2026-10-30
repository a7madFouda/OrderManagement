const antiforgeryToken = () =>
    document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';

async function send(url, options = {}) {
    const response = await fetch(url, options);
    const contentType = response.headers.get('Content-Type') ?? '';
    const data = contentType.includes('application/json') ? await response.json() : await response.text();

    return { ok: response.ok, data };
}

export const getHtml = url => send(url);

export const getJson = url => send(url, { headers: { Accept: 'application/json' } });

export const post = url => send(url, {
    method: 'POST',
    headers: { RequestVerificationToken: antiforgeryToken() }
});

export const postJson = (url, body) => send(url, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        RequestVerificationToken: antiforgeryToken()
    },
    body: JSON.stringify(body)
});
