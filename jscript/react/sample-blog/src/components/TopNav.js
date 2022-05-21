import React from 'react';
import { NavLink } from 'react-router-dom';
import { getPages } from './../service';

export const TopNav = () => {
    const [pages, setPages] = React.useState([]);

    React.useEffect(() => {
        async function fetchData() {
            const results = await getPages();
    
            setPages(results.pages);
        }

        fetchData();
    }, []);

    return (
        <ul className='navbar-nav mr-auto'>
            {pages.map(page => (
                <li key={page.id} className='nav-item'>
                    <NavLink activeClassName='active' className='nav-link' to={`/pages/${page.slug}`}>{page.title}</NavLink>
                </li>
            ))}
        </ul>
    )
};