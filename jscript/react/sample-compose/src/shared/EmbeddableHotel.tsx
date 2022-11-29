import { Link } from 'react-router-dom';
import { useApiContext } from '../utils';
import { Markdown } from './Markdown';

export const EmbeddableHotel = ({ hotel }: { hotel: any }) => {
    const { id, flatData: data } = hotel;
    const api = useApiContext();

    return (
        <div className='card mt-2'>
            <div className='card-body'>
                <div className='row'>
                    <div className='col-auto'>
                        <div style={{ width: '150px' }}>
                            {data.photos?.length > 0 &&
                                <img alt={hotel.name} src={`${api.apiUrl}/api/assets/${api.appName}/${data.photos[0].id}?width=150&height=150`} />
                            }
                        </div>
                    </div>
                    <div className='col'>
                        <Link to={`/hotels/${id}`}>
                            <h4>{data.name}</h4>
                        </Link>

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
                    </div>
                </div>  
            </div>
        </div>
    );
};