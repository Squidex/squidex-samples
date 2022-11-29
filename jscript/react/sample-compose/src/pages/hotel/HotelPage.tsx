import { useParams } from 'react-router-dom';
import { GRAPHQL_FIELDS, Hotel, useFetch } from './../../shared';

export const HotelPage = () => {
    const { id } = useParams();
    const { result, loading } = useFetch(async (fetcher, api) => {
        const query = `
            {
                findHotelsContent(id: "${id}") {
                    ${GRAPHQL_FIELDS.hotel}
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.findHotelsContent;
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: [result.id] };
    }, [id]);

    if (result) {
        return <Hotel hotel={result} />
    } else if (!result && !loading) {
        return <div>Hotel not found.</div>
    } else {
        return <div>Loading Hotel...</div>
    }
}
