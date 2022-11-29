import { useParams } from 'react-router-dom';
import { GRAPHQL_FIELDS, Post, useFetch } from './../../shared';

export const PostPage = () => {
    const { id } = useParams();
    const { result: page, loading } = useFetch(async (fetcher, api) => {
        const query = `
            {
                findPostsContent(id: "${id}") {
                    ${GRAPHQL_FIELDS.post}
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.findPostsContent;
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: [result?.id] };
    }, [id]);

    if (page) {
        return <Post post={page} />
    } else if (!page && !loading) {
        return <div>Page not found.</div>
    } else {
        return <div>Loading Page...</div>
    }
}
