import React from 'react';
import { useRefresh } from "./hooks"

export const SubscribePage = () => {
    useRefresh();

    return (
        <div>Subscribing.</div>
    );
}