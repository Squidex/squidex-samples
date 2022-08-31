import React from 'react';
import { Hotel } from './Hotel.js';
import { getHotels } from './../service';
import { useRefresh } from './hooks.js';

export const HotelsSite = () => {
    const [hotels, setHotels] = React.useState();
    const needsUpdate = useRefresh(hotels?.map(x => x.id));

    React.useEffect(() => {
        async function fetchData() {
            const results = await getHotels();

            setHotels(results.items);
        }

        fetchData();
    }, [needsUpdate]);

    if (!hotels) {
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
