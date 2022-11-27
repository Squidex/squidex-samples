import { createClient } from 'graphql-ws';
import React, { useMemo } from 'react';
import { ApiContextType, useApiContext } from './api-context';

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

const ACTIVE = window.location.search?.indexOf('preview=1') >= 0 || !!window.parent;

class RefreshListener {
    private readonly subscriptions: Subscription[] = [];
    private static readonly INSTANCES: { [appName: string]: RefreshListener } = {};

    constructor(api: ApiContextType, isActive: boolean) {
        if (isActive) {
            this.init(api);
        }
    }

    public static create(api: ApiContextType) {
        let listener = RefreshListener.INSTANCES[api.appName!];

        if (!listener) {
            listener = new RefreshListener(api, ACTIVE);
            RefreshListener.INSTANCES[api.appName!] = listener;
        }

        return listener;
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

export function useRefresh(ids?: string[]) {
    const apiContext = useApiContext();
    const [updateIds, setUpdateIds] = React.useState<string[]>([]);
    const [updateNeeded, setUpdateNeeded] = React.useState(0);

    const listener = useMemo(() => {
        return RefreshListener.create(apiContext);
    }, [apiContext]);

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
        if (!updateIds || updateIds.length === 0 || !updateIds[0]) {
            return;
        }

        const subscription = new Subscription(updateIds, () => {
            setUpdateNeeded(x => x + 1);
        });

        listener.add(subscription);

        return () => {
            listener.remove(subscription);
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