import { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import { useFetch } from '../utils';
import { NavLink } from './NavLink';

interface PageLayoutProps {
    children: ReactNode;
}

export const PageLayout = (props: PageLayoutProps) => {
    const { children } = props;
    
    const { result: landingPages } = useFetch(async (fetcher, api) => {
        const query = `
            {
                queryLandingPagesContents {
                    id,
                    flatData {
                        slug,
                        title
                    }
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryLandingPagesContents as any[];
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: result.map(x => x.id) };
    });

    const { result: pages } = useFetch(async (fetcher, api) => {
        const query = `
            {
                queryPagesContents {
                    id,
                    flatData {
                        slug,
                        title
                    }
                }
            }`;
    
        const response = await fetcher(`api/content/${api.appName}/graphql`, { query });
        const result = response.data.queryPagesContents as any[];
    
        // This hook also returns a list of IDs. All the IDs are monitored using a GraphQL subscription.
        return { result, ids: result.map(x => x.id) };
    });

    return (
        <>
            <nav className='navbar fixed-top navbar-dark bg-primary navbar-expand-sm'>
                <div className='container'>
                    <Link to='/' className='navbar-brand'>Squidex Compose Sample</Link>

                    <ul className='navbar-nav mr-auto'>
                        {pages &&
                            <>
                                {pages.map(page => (
                                    <li key={page.id} className='nav-item'>
                                        <NavLink className='nav-link' to={`/pages/${page.flatData.slug}`}>{page.flatData.title}</NavLink>
                                    </li>
                                ))}
                            </>
                        }
                        
                        <li className='nav-item'>
                            <NavLink className='nav-link' to='/blog'>Blog</NavLink>
                        </li>

                        <li className='nav-item'>
                            <NavLink className='nav-link' to='/hotels'>Hotels</NavLink>
                        </li>
                    </ul>
                </div>
            </nav>
            
            <div className='container body-content'>
                {children}
            </div>

            <hr />

            <div className='container'>
                {landingPages && landingPages.length > 0 &&
                    <ul>
                        {landingPages.map(page =>
                            <li key={page.id}>
                                <NavLink to={`/landing-page/${page.flatData.slug}`}>
                                    {page.flatData.title}
                                </NavLink>
                            </li>
                        )}
                    </ul>
                }
            </div>
        </>
    )
}