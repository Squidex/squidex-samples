import React from 'react';
import { Markdown } from './Markdown';

export const Page = ({ page }) => {
    const { flatData: data } = page;

    return (
        <div>
            <h2>{data.title}</h2>

            <Markdown markdown={data.text.text} references={data.text.contents} />
        </div>
    );
};