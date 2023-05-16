import { SquidexClient } from '@squidex/squidex';

const Client = new SquidexClient({
    appName: 'sample-blog',
    clientId: 'sample-blog:blog',
    clientSecret: 'ZxmQGgouOUmyVU4fh38QOCqKja3IH1vPu1dUx40KDec=',
    environment: 'https://cloud.squidex.io'
});

export async function getPost(id) {
    const json = await Client.contents.getContent('posts', id);

    return parsePost(json);
}

export async function getPage(slug) {
    const { items } = await Client.contents.getContents('pages', { filter: `data/slug/iv eq '${slug}'` });

    if (items.length === 0) {
        return null;
    }

    return parsePage(items[0]);
}

export async function getPosts() {
    const { total, items } = await Client.contents.getContents(`posts`);

    return { total, posts: items.map(x => parsePost(x)) };
}

export async function getPages() {
    const { total, items } = await Client.contents.getContents(`pages`);

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