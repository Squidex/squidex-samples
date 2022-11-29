import { useParams } from 'react-router-dom';
import { Viewer } from '../editor/Viewer';
import { GRAPHQL_FIELDS, useFetch } from './../../shared';

export const LandingPage = () => {
    const { slug } = useParams();
    const { result: page, loading } = useFetch(async (fetcher, api) => {
        const query = `
            {
                queryLandingPagesContents(filter: "data/slug/iv eq '${slug}'") {
                    ${GRAPHQL_FIELDS.landingPage}
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryLandingPagesContents[0];
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: [result?.id] };
    }, [slug]);

    if (page) {
        return (
            <div>
                <h1 className='mb-4'>{page.flatData.title}</h1>

                <Viewer value={page.flatData.content?.page} preloaded={page.flatData.references} />
            </div>
        )
    } else if (!page && !loading) {
        return <div>Page not found.</div>
    } else {
        return <div>Loading Page...</div>
    }
}
