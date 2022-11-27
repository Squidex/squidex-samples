import React from 'react'
import ReactDOM from 'react-dom'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { ApiContext, createDefaultApiContext } from './utils';
import { EditorPage } from './pages/editor/EditorPage';
import { ErrorPage } from './pages/error/ErrorPage';
import { HotelPage } from './pages/hotel/HotelPage';
import { HotelsPage } from './pages/hotels/HotelsPage';
import { PagePage } from './pages/page/PagePage';
import { PostPage } from './pages/post/PostPage';
import { PostsPage } from './pages/posts/PostsPage';
import { RootPage } from './pages/root/RootPage';
import './index.css'
import { LandingPage } from './pages/lading-page/LandingPage';

const router = createBrowserRouter([
    {
        path: '/',
        element: <RootPage />,
        errorElement: <ErrorPage />,
        children: [
            {
                path: '/',
                element: <PostsPage />,
            },
            {
                path: '/editor',
                element: <EditorPage />,
            },
            {
                path: '/blog/',
                element: <PostsPage />,
            },
            {
                path: '/blog/:id/:slug/',
                element: <PostPage />,
            },
            {
                path: '/hotels',
                element: <HotelsPage />,
            },
            {
                path: '/hotels/:id',
                element: <HotelPage />,
            },
            {
                path: '/pages/:slug/',
                element: <PagePage />,
            },
            {
                path: '/landing-page/:slug/',
                element: <LandingPage />,
            }
        ]
    }
]);

const apiSetup = createDefaultApiContext();

ReactDOM.render(
    <React.StrictMode>
        <ApiContext.Provider value={apiSetup}>
            <RouterProvider router={router} />
        </ApiContext.Provider>
    </React.StrictMode>,
    document.getElementById('root') as HTMLElement)
