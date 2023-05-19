import { Injectable } from '@angular/core';
import { SquidexClient } from '@squidex/squidex';
import { from, Observable } from 'rxjs';

export interface Post {
    id: string;
    text: string;
    title: string;
    slug: string;
}

export interface Page {
    id: string;
    text: string;
    title: string;
    slug: string;
}

@Injectable({
    providedIn: 'root'
})
export class ContentService {
    constructor(
        private readonly client: SquidexClient,
    ) {
    }

    public getPosts(): Observable<{ total: number, posts: Post[] }> {
        return from((async () => {
            const { total, items } = await this.client.contents.getContents('posts');

            return { total, posts: items.map((x: any) => parsePost(x)) };
        })());
    }

    public getPages(): Observable<{ total: number, pages: Page[] }> {
        return from((async () => {
            const { total, items } = await this.client.contents.getContents('pages');

            return { total, pages: items.map((x: any) => parsePage(x)) };
        })());
    }

    public getPost(id: string): Observable<Page | null> {
        return from((async () => {
            try {
                const item = await this.client.contents.getContent('pages', id);
    
                return parsePost(item);
            } catch {
                return null;
            }
        })());
    }

    public getPage(slug: string): Observable<Page | null> {
        return from((async () => {
            const { items } = await this.client.contents.getContents('pages', { filter: `data/slug/iv eq '${slug}'` });

            if (items.length === 0) {
                return null;
            }

            return parsePage(items[0]);
        })());
    }
}


function parsePost(response: any): Post {
    return {
        id: response.id,
        title: response.data.title.iv,
        text: response.data.text.iv,
        slug: response.data.slug.iv
    };
}

function parsePage(response: any): Page {
    return {
        id: response.id,
        title: response.data.title.iv,
        text: response.data.text.iv,
        slug: response.data.slug.iv
    };
}
