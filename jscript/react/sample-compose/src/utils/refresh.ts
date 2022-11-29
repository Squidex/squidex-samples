import { createClient } from 'graphql-ws';
import React, { createContext, useContext } from 'react';
import { ApiContextType } from './api-context';

class Subscription {
    constructor(
        private readonly elementIds: string[],
        private readonly elementUpdater: () => void
    ) {
    }

    public refresh(id: string) {
        if (this.elementIds.indexOf(id) >= 0) {
            this.elementUpdater();
            console.log('Data has been reloaded');
        }
    }
}

export class RefreshListener {
    private readonly subscriptions: Subscription[] = [];

    constructor(api: ApiContextType) {
         this.init(api);
    }

    public add(subscription: Subscription) {
        this.subscriptions.push(subscription);
    }

    public remove(subscription: Subscription) {
        this.subscriptions.splice(this.subscriptions.indexOf(subscription), 1);
    }

    private async init(api: ApiContextType) {
        if (!api.apiUrl || !api.accessToken || !api.appName) {
            return;
        }

        const token = await api.accessToken(false);

        if (!token) {
            return;
        }
        
        let url = `${api.apiUrl}/api/content/${api.appName}/graphql?access_token=${token}`;

        url = url.replace('http://', 'ws://');
        url = url.replace('https://', 'wss://');

        const client = createClient({
            url
        });

        client.subscribe({
            query:
                `subscription {
                    contentChanges {
                        id
                    }
                }`,
        }, {
            next: (event: any) => {
                const id = event.data.contentChanges.id;

                for (const subscription of this.subscriptions) {
                    subscription.refresh(id);
                }
            },
            error: error => {
                console.log(`GRAPHQL error ${error}`);
            },
            complete: () => {
                console.log('GRAPHQL Subscription closed.');
            }
        })
    }
}

export type RefreshContextType = {
    // The refresh listener.
    listener?: RefreshListener | null;
}

export const RefreshContext = createContext<RefreshContextType>({} as any);

export function useRefreshContext() {
    return useContext(RefreshContext);
}

export function useRefresh(ids?: string[]) {
    const listener = useRefreshContext()?.listener;
    const [updateIds, setUpdateIds] = React.useState<string[]>([]);
    const [updateNeeded, setUpdateNeeded] = React.useState(0);

    React.useEffect(() => {
        setUpdateIds(previous => {
            if (!ids || isSame(previous, ids)) {
                return previous;
            } else {
                return ids;
            }
        });
    }, [ids]);

    React.useEffect(() => {
        if (!listener) {
            return;
        }

        if (!updateIds || updateIds.length === 0 || !updateIds[0]) {
            return;
        }

        const subscription = new Subscription(updateIds, () => {
            setUpdateNeeded(x => x + 1);
        });

        listener.add(subscription);

        return () => {
            listener!.remove(subscription);
        };
    }, [listener, updateIds]);

    return updateNeeded;
}

function isSame(a: string[], b: string[] | undefined) {
    if (!a && !b) {
        return true;
    }

    if (!a || !b) {
        return false;
    }

    if (a.length !== b.length) {
        return false;
    }

    for (let i = 0 ; i < a.length; i++) {
        if (a[i] !== b[i]) {
            return false;
        }
    }

    return true;
}