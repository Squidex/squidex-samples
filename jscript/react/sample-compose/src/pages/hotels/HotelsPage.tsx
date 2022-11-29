import { GRAPHQL_FIELDS, Hotel, useFetch } from './../../shared';

export const HotelsPage = () => {
    const { result: hotels, loading } = useFetch(async (fetcher, api) => {
        const query = `
        {
            queryHotelsContentsWithTotal {
                total, 
                items {
                    ${GRAPHQL_FIELDS.hotel}
                }
            }
        }`;

        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryHotelsContentsWithTotal.items as any[];

        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: result.map(x => x.id) };
    });

    if (loading || !hotels) {
        return <div>Loading Hotels...</div>
    }

    return (
        <>
            {hotels.length === 0 ? (
                <>No Hotel found</>
            ) : (
                <div>
                    {hotels.map(hotel => (
                        <div key={hotel.id}>
                            <Hotel key={hotel.id} hotel={hotel} withLink={true} />

                            <hr />
                        </div>
                    ))}
                </div>
            )}
        </>
    );
}
