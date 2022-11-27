import { Link } from 'react-router-dom';
import { flatten, useApiContext } from '../utils';
import { Markdown } from './Markdown'

export const Hotel = ({ hotel, withLink }: { hotel: any, withLink?: boolean }) => {
    const { id } = hotel;
    const api = useApiContext();
    const hotelData = flatten(hotel);
    const hotelPhotos = hotelData.photos as any[];

    return (
        <div>
            {withLink ? (
                <Link to={`/hotels/${id}`}>
                    <h2>{hotelData.name}</h2>
                </Link>
            ) : (
                <h2>{hotelData.name}</h2>
            )}

            {hotelData.rooms &&
                <div>
                    <small>{hotelData.rooms} Rooms</small>
                </div>
            }

            <Markdown markdown={hotelData.description} />

            {hotelData.minPrice &&
                <div className='mt-2'>
                    Price: {hotelData.minPrice} EUR
                </div>
            }

            {hotelPhotos && hotelPhotos.length > 0 &&
                <div>
                    {hotelPhotos.map(photo => 
                        <img key={photo.id || photo} className='mr-1' alt={photo.name} src={`${api.apiUrl}/api/assets/${api.appName}/${photo.id || photo}?width=150&height=150`} />
                    )}
                </div>
            }
        </div>
    );
};