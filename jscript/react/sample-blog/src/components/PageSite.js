import React from 'react';
import { useParams } from 'react-router-dom';
import { Page } from './Page.js';
import { getPage } from './../service';

export const PageSite = () => {
    const { slug } = useParams();

    const [page, setPage] = React.useState(undefined);

    React.useEffect(() => {
        setPage(undefined);

        async function fetchData() {
            try {
                const result = await getPage(slug);

                setPage(result);
            } catch (ex) {
                setPage(null);
            }
        }

        fetchData();
    }, [slug]);

    if (page) {
        return <Page page={page} />
    } else if (page === null) {
        return <div>Page not found.</div>
    } else {
        return <div>Loading Page...</div>
    }
}
