import { useParams } from 'react-router-dom';
import { GRAPHQL_FIELDS, Page, useFetch } from './../../shared';

export const PagePage = () => {
    const { slug } = useParams();
    const { result: page, loading } = useFetch(async (fetcher, api) => {
        const query = `
            {
                queryPagesContents(filter: "data/slug/iv eq '${slug}'") {
                    ${GRAPHQL_FIELDS.post}
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryPagesContents[0];
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: [result?.id] };
    }, [slug]);

    if (page) {
        return <Page page={page} />
    } else if (!page && !loading) {
        return <div>Page not found.</div>
    } else {
        return <div>Loading Page...</div>
    }
}
