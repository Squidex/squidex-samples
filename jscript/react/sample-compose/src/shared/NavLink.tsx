import { forwardRef } from 'react';
import { NavLink as BaseNavLink, NavLinkProps } from 'react-router-dom';

export const NavLink = forwardRef<any, NavLinkProps>((props, ref) => {
    return (
        <BaseNavLink {...props} ref={ref}
            className={({ isActive }) => [props.className, isActive ? 'active' : null,].filter(Boolean).join(' ')}
        />
    );
}
);