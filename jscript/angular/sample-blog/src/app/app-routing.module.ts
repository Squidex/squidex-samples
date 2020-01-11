import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PageSiteComponent } from './page-site/page-site.component';
import { PostSiteComponent } from './post-site/post-site.component';
import { PostsSiteComponent } from './posts-site/posts-site.component';

const routes: Routes = [
    {
        path: 'blog/:id/:slug?',
        component: PostSiteComponent
    },
    {
        path: 'blog',
        pathMatch: 'full',
        component: PostsSiteComponent
    },
    {
        path: 'pages/:slug',
        component: PageSiteComponent
    },
    {
        path: '**',
        component: PostsSiteComponent
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
