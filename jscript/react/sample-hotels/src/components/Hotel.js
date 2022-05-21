import React from 'react';
import { Link } from 'react-router-dom';
import { CONFIG } from '../service';
import { Markdown } from './Markdown';

export const Hotel = ({ hotel, withLink }) => {
    const { id, flatData: data } = hotel;

    return (
        <div>
            {withLink ? (
                <Link to={`/hotels/${id}`}>
                    <h2>{data.name}</h2>
                </Link>
            ) : (
                <h2>{data.name}</h2>
            )}

            {data.rooms &&
                <div>
                    <small>{data.rooms} Rooms</small>
                </div>
            }

            <Markdown markdown={data.description} />

            {data.minPrice &&
                <div className='mt-2'>
                    Price: {data.minPrice} EUR
                </div>
            }

            {data.photos && data.photos.length > 0 &&
                <div>
                    {data.photos.map(photo => 
                        <img key={photo.id} className='mr-1' alt={hotel.name} src={`${CONFIG.url}/api/assets/${CONFIG.appName}/${photo.id}?width=150&height=150`} />
                    )}
                </div>
            }
        </div>
    );
};