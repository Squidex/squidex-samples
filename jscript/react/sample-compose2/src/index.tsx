import React from 'react'
import ReactDOM from 'react-dom'
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import EditorPage from './pages/editor/EditorPage';
import RootPage from './pages/root/RootPage';
import './index.css'

const router = createBrowserRouter([
    {
        path: "/",
        element: <RootPage />,
    },
    {
        path: "/editor",
        element: <EditorPage />,
    },
]);

ReactDOM.render(
    <React.StrictMode>
        <RouterProvider router={router} />
    </React.StrictMode>,
    document.getElementById('root') as HTMLElement)
