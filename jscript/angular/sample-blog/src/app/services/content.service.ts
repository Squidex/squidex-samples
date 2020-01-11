import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { Config } from './config';

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
        private readonly httpClient: HttpClient,
        private readonly config: Config
    ) {
    }

    public getPosts(): Observable<{ total: number, posts: Post[] }> {
        const url = this.config.buildUrl(`api/content/{app}/posts`);

        return this.httpClient.get<any>(url)
            .pipe(map(payload => {
                const { total, items } = payload;

                return { total, posts: items.map((x: any) => parsePost(x)) };
            })
        );
    }

    public getPages(): Observable<{ total: number, pages: Page[] }> {
        const url = this.config.buildUrl(`api/content/{app}/pages`);

        return this.httpClient.get<any>(url)
            .pipe(map(payload => {
                const { total, items } = payload;

                return { total, pages: items.map((x: any) => parsePage(x)) };
            })
        );
    }

    public getPost(id: string): Observable<Page | null> {
        const url = this.config.buildUrl(`api/content/{app}/posts/${id}`);

        return this.httpClient.get<any>(url)
            .pipe(map(payload => {
                return parsePost(payload);
            })
        );
    }

    public getPage(slug: string): Observable<Page | null> {
        const url = this.config.buildUrl(`api/content/{app}/pages?$filter=data/slug/iv eq '${slug}'`);

        return this.httpClient.get<any>(url)
            .pipe(map(payload => {
                const { items } = payload;

                if (items.length === 0) {
                    return null;
                }

                return parsePage(items[0]);
            })
        );
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
