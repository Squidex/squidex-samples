import React from 'react';
import { useParams } from 'react-router-dom';
import { Hotel } from './Hotel.js';
import { getHotel } from './../service';
import { useRefresh } from './hooks.js';

export const HotelSite = () => {
    const { id } = useParams();

    const [hotel, setHotel] = React.useState(undefined);
    const needsUpdate = useRefresh([hotel?.id]);

    React.useEffect(() => {
        async function fetchData() {
            try {
                const result = await getHotel(id);
        
                setHotel(result);
            } catch (ex) {
                setHotel(null);
            }
        }

        fetchData();
    }, [id, needsUpdate]);

    if (hotel) {
        return <Hotel hotel={hotel} />
    } else if (hotel === null) {
        return <div>Hotel not found.</div>
    } else {
        return <div>Loading Hotel...</div>
    }
}
