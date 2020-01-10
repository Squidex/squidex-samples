import React from 'react';

export const Page = ({ page }) => {
    return (
        <div>
            <h2>{page.title}</h2>

            <div dangerouslySetInnerHTML={{ __html: page.text }} />
        </div>
    );
};