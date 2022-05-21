import React from 'react';
import { Hotel } from './Hotel.js';
import { getHotels } from './../service';

export const HotelsSite = () => {
    const [hotels, setHotels] = React.useState();

    React.useEffect(() => {
        async function fetchData() {
            const results = await getHotels();

            setHotels(results.items);
        }

        fetchData();
    }, []);

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
