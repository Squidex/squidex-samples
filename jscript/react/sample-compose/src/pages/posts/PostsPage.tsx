import { GRAPHQL_FIELDS, Post, useFetch } from './../../shared';

export const PostsPage = () => {
    const { result: posts, loading } = useFetch(async (fetcher, api) => {
        const query = `
        {
            queryPostsContentsWithTotal {
                total
                items {
                    ${GRAPHQL_FIELDS.post}
                }
            }
        }`;

        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryPostsContentsWithTotal.items as any[];

        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: getPostsIds(result) };
    });

    if (loading || !posts) {
        return <div>Loading Post...</div>
    }

    return (
        <>
            {posts.length === 0 ? (
                <>No Post found</>
            ) : (
                <div>
                    {posts.map(post => (
                        <div key={post.id}>
                            <Post key={post.id} post={post} withLink={true} />

                            <hr />
                        </div>
                    ))}
                </div>
            )}
        </>
    );
}

export function getPostsIds(posts: any[]) {
    const result = [];

    if (posts) {
        for (const post of posts) {
            if (post) {
                result.push(post.id);
            }

            if (post.flatData.text.contents) {
                for (const content of post.flatData.text.contents) {
                    result.push(content.id);
                }
            }
        }
    }

    return result;
}