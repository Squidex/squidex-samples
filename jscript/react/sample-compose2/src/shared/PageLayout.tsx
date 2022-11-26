import { ReactNode } from 'react';

interface PageLayoutProps {
    children: ReactNode;
}

const PageLayout = (props: PageLayoutProps) => {
    const { children } = props;

    return (
        <div>
            {children}
        </div>
    )
}

export default PageLayout;