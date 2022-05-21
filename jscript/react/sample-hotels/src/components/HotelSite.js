import React from 'react';
import { useParams } from 'react-router-dom';
import { Hotel } from './Hotel.js';
import { getHotel } from './../service';

export const HotelSite = () => {
    const { id } = useParams();

    const [hotel, setHotel] = React.useState(undefined);

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
    }, [id]);

    if (hotel) {
        return <Hotel hotel={hotel} />
    } else if (hotel === null) {
        return <div>Hotel not found.</div>
    } else {
        return <div>Loading Hotel...</div>
    }
}
