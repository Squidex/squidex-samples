import React from 'react';
import { buildSubscriptionClient } from '../service';

class Subscription {
    elementIds;
    elementUpdater;

    constructor(elementIds, action) {
        this.elementIds = elementIds;
        this.elementUpdater = action;
    }

    refresh(id) {
        if (this.elementIds.indexOf(id) >= 0) {
            this.elementUpdater();
            console.log('Data has been reloaded');
        }
    }
}

const ACTIVE = window.location.search?.indexOf('preview=1') >= 0;

class RefreshListener {
    subscriptions = [];

    static INSTANCE = new RefreshListener(ACTIVE);

    constructor(isActive) {
        if (isActive) {
            this.init();
        }
    }

    add(subscription) {
        this.subscriptions.push(subscription);
    }

    remove(subscription) {
        this.subscriptions.splice(this.subscriptions.indexOf(subscription), 1);
    }

    async init() {
        const client = await buildSubscriptionClient();

        client.subscribe({
            query:
                `subscription {
                    contentChanges {
                        id
                    }
                }`,
        }, {
            next: event => {
                const id = event.data.contentChanges.id;

                for (const subscription of this.subscriptions) {
                    subscription.refresh(id);
                }
            },
            error: error => {
                console.log(`GRAPHQL error ${error}`);
            }
        })
    }
}

export function useRefresh(ids) {
    const [updateIds, setUpdateIds] = React.useState([]);
    const [updateNeeded, setUpdateNeeded] = React.useState(0);

    React.useEffect(() => {
        setUpdateIds(previous => {
            if (isSame(previous, ids)) {
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

        RefreshListener.INSTANCE.add(subscription);

        return () => {
            RefreshListener.INSTANCE.remove(subscription);
        };
    }, [updateIds]);

    return updateNeeded;
}

function isSame(a, b) {
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