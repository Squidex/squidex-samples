import React from 'react';
import { BrowserRouter as Router, Switch, Redirect, Route, Link } from 'react-router-dom';
import { HotelSite, HotelsSite, PageSite, PostSite, PostsSite, SubscribePage, TopNav } from './components';

function App() {
    return (
        <Router>
            <nav className='navbar fixed-top navbar-dark bg-primary navbar-expand-sm'>
                <div className='container'>
                    <Link to='/' className='navbar-brand'>Squidex Blog Sample</Link>

                    <TopNav />
                </div>
            </nav>

            <div className='container body-content'>
                <Switch>
                    <Route path='/' exact>
                        <PostsSite />
                    </Route>
                    <Route path='/blog' exact>
                        <PostsSite />
                    </Route>
                    <Route path='/blog/:id/:slug/'>
                        <PostSite />
                    </Route>
                    <Route path='/hotels' exact>
                        <HotelsSite />
                    </Route>
                    <Route path='/hotels/:id'>
                        <HotelSite />
                    </Route>
                    <Route path='/pages/:slug/'>
                        <PageSite />
                    </Route>
                    <Route path='/subscribe'>
                        <SubscribePage />
                    </Route>
                    <Route render={_ =>
                        <Redirect to='/blog' />
                    } />
                </Switch>
            </div>
        </Router>
    );
}

export default App;
