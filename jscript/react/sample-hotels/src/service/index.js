import { createClient } from "graphql-ws";

const DEV = true;

export const CONFIG = !DEV ? {
    url: 'https://cloud.squidex.io',
    appName: 'hotels',
    clientId: 'hotels:default',
    clientSecret: 'ZxmQGgouOUmyVU4fh38QOCqKja3IH1vPu1dUx40KDec='
} : {
    url:  'https://localhost:5001',
    appName: 'hotels3',
    clientId: 'hotels3:default',
    clientSecret: 'hevj7xpxuhk3ofoqb9lqa3d2oxctzrxctruic7tz124x'
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

const HOTEL_FIELDS = `
    id,
    flatData {
        name,
        description,
        minPrice,
        rooms,
        photos {
            id,
            fileName
        }
    }`;

const POST_FIELDS = `
    id
    flatData {
        title
        text {
            text
            contents {
                ...on Hotels {
                    ${HOTEL_FIELDS}
                }
            }
        }
    }`

export async function buildSubscriptionClient() {
    const token = await getBearerToken();

    const url =
        buildUrl(`api/content/${CONFIG.appName}/graphql?access_token=${token}`)
            .replace('http://', 'ws://')
            .replace('https://', 'wss://');

    return createClient({
        url
    });
}

export async function getPost(id) {
    const query = `
        {
            findPostsContent(id: "${id}") {
                ${POST_FIELDS}
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.findPostsContent;
}

export async function getHotel(id) {
    const query = `
        {
            findHotelsContent(id: "${id}") {
                ${HOTEL_FIELDS}
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.findHotelsContent;
}

export async function getPage(slug) {
    const query = `
        {
            queryPagesContents(filter: "data/slug/iv eq '${slug}'") {
                ${POST_FIELDS}
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.queryPagesContents[0];
}

export async function getPosts() {
    const query = `
        {
            queryPostsContentsWithTotal {
                total
                items {
                    ${POST_FIELDS}
                }
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.queryPostsContentsWithTotal;
}

export async function getPages() {
    const query = `
        {
            queryPagesContentsWithTotal {
                total
                items {
                    ${POST_FIELDS}
                }
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.queryPagesContentsWithTotal;
}

export async function getHotels() {
    const query = `
        {
            queryHotelsContentsWithTotal {
                total, 
                items {
                    ${HOTEL_FIELDS}
                }
            }
        }`;

    const json = await getContent(`api/content/${CONFIG.appName}/graphql`, { query });

    return json.data.queryHotelsContentsWithTotal;
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

function getContent(url, body) {
    return getContentInternal(url, body, true);
}

async function getContentInternal(url, body, retry) {
    // Fetch the bearer token.
    const token = await fetchBearerToken();

    const response = await fetch(buildUrl(url), {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
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

export function getPostsIds(posts) {
    const result = [];

    if (posts) {
        for (const post of posts) {
            if (post) {
                result.push(post.id);
            }

            if (post.flatData.text.contents) {
                for (const content of post.flatData.text.contents) {
                    result.push(content.id);
                }
            }
        }
    }

    return result;
}