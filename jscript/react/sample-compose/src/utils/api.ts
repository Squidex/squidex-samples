/* eslint-disable react-hooks/exhaustive-deps */
import { useEffect, useMemo, useState } from 'react';
import { ApiContextType, useApiContext } from './api-context';
import { useRefresh } from './refresh';

function getContent(api: ApiContextType, url: string, body: any) {
    if (url.length > 0 && url.startsWith('/')) {
        url = url.substring(1);
    }

    const absoluteUrl = `${api.apiUrl}/${url}`

    return getContentInternal(api, absoluteUrl, body, true);
}

async function getContentInternal(api: ApiContextType, absoluteUrl: string, body: any, retry = false): Promise<any> {
    // Fetch the bearer token.
    const token = await api.accessToken(!retry);

    const response = await fetch(absoluteUrl, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Accept-Encoding': 'utf8',
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    });     

    if (!response.ok) {
        if (response.status === 403 || response.status === 401) {
            if (retry) {
                return getContentInternal(api, absoluteUrl, body, false);
            }
        }

        throw new Error(`Failed to retrieve content, got ${response.statusText}`);
    }

    return await response.json();
}

type FetcherMethod = (url: string, body: any) => any;
type FetcherResult<T> = { result: T, ids: string[] };
type FetcherState<T> = { result?: T; error?: any; loading: boolean; ids?: string[] };

export function useFetch<T>(query: (fetcher: FetcherMethod, api: ApiContextType) => Promise<FetcherResult<T>>, deps: any[] = []): FetcherState<T> {
    const api = useApiContext();
    const [state, setState] = useState<FetcherState<T>>({ loading: true });

    const fetcher = useMemo(() => {
        return (url: string, body: any) => getContent(api, url, body);
    }, [api]);

    const needsUpdate = useRefresh(state.ids);

    useEffect(() => {
        async function fetchData() {
            try {
                setState(x => ({ ...x, loading: true }));

                const { result, ids } = await query(fetcher, api);
        
                setState(x => ({ ...x, error: null, result, ids }));
            } catch (error) {
                setState(x => ({ ...x, error }));
            } finally {
                setState(x => ({ ...x, loading: false }));
            }
        }

        fetchData();
    }, [fetcher, needsUpdate, ...deps]);

    return state;

}