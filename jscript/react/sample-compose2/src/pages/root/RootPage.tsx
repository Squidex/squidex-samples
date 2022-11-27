import { Outlet } from 'react-router-dom';
import { PageLayout } from '../../shared';

export const RootPage = () => {
    return (
        <PageLayout>
            <Outlet />
        </PageLayout>
    );
};