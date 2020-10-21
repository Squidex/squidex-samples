const CONFIG = {
    url: 'https://cloud.squidex.io',
    appName: 'sample-blog',
    clientId: 'sample-blog:blog',
    clientSecret: 'ZxmQGgouOUmyVU4fh38QOCqKja3IH1vPu1dUx40KDec='
};

function getBearerToken() {
    return localStorage.getItem('token');
}

function setBearerToken(token) {
    localStorage.setItem('token', token);
}

function clearBearerToken() {
    localStorage.removeItem('token');
}

export async function getPost(id) {
    const json = await getContent(`api/content/${CONFIG.appName}/posts/${id}`);

    return parsePost(json);
}

export async function getPage(slug) {
    const json = await getContent(`api/content/${CONFIG.appName}/pages/?$filter=data/slug/iv eq '${slug}'`);

    const { items } = json;

    if (items.length === 0) {
        return null;
    }

    return parsePage(items[0]);
}

export async function getPosts() {
    const json = await getContent(`api/content/${CONFIG.appName}/posts`);

    const { total, items } = json;

    return { total, posts: items.map(x => parsePost(x)) };
}

export async function getPages() {
    const json = await getContent(`api/content/${CONFIG.appName}/pages`);

    const { total, items } = json;

    return { total, pages: items.map(x => parsePage(x)) };
}

function parsePost(response) {
    return {
        id: response.id,
        title: response.data.title.iv,
        text: response.data.text.iv,
        slug: response.data.slug.iv
    };
}

function parsePage(response) {
    return {
        id: response.id,
        title: response.data.title.iv,
        text: response.data.text.iv,
        slug: response.data.slug.iv
    };
}

export async function fetchBearerToken() {
    // Check if we have already a bearer token in local store.
    let token = getBearerToken();

    if (token) {
        return token;
    }

    const body = `grant_type=client_credentials&scope=squidex-api&client_id=${CONFIG.clientId}&client_secret=${CONFIG.clientSecret}`;

    // Get the bearer token. Ensure that we use a client id with readonly permissions.
    const response = await fetch(buildUrl('identity-server/connect/token'), { 
        method: 'POST', 
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body 
    });

    if (!response.ok) {
        throw new Error(`Failed to retrieve token, got ${response.statusText}`);
    }

    const json = await response.json();

    token = json.access_token;

    // Cache the bearer token in the local store.
    setBearerToken(token);

    return token;
}

function getContent(url) {
    return getContentInternal(url, true);
}

async function getContentInternal(url, retry) {
    // Fetch the bearer token.
    const token = await fetchBearerToken();

    const response = await fetch(buildUrl(url), {
        headers: {
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });     

    if (!response.ok) {
        if (response.status === 403 || response.status === 401) {
            // If we get an error we clear the bearer token and retry the request.
            clearBearerToken();

            if (retry) {
                return getContentInternal(url);
            }
        }

        throw new Error(`Failed to retrieve content, got ${response.statusText}`);
    }

    return await response.json();
}

function buildUrl(url) {
    if (url.length > 0 && url.startsWith('/')) {
        url = url.substr(1);
    }

    const result = `${CONFIG.url}/${url}`;

    return result;
}