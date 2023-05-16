import { NgModule } from '@angular/core';
import { SquidexClient } from '@squidex/squidex';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PageSiteComponent } from './page-site/page-site.component';
import { PageComponent } from './page/page.component';
import { PostSiteComponent } from './post-site/post-site.component';
import { PostComponent } from './post/post.component';
import { PostsSiteComponent } from './posts-site/posts-site.component';
import { TopNavComponent } from './top-nav/top-nav.component';

const Client = new SquidexClient({
    appName: 'sample-blog',
    clientId: 'sample-blog:blog',
    clientSecret: 'ZxmQGgouOUmyVU4fh38QOCqKja3IH1vPu1dUx40KDec=',
    environment: 'https://cloud.squidex.io',
});

@NgModule({
    declarations: [
        AppComponent,
        PostComponent,
        PostSiteComponent,
        PostsSiteComponent,
        PageComponent,
        PageSiteComponent,
        TopNavComponent
    ],
    imports: [
        BrowserModule,
        AppRoutingModule
    ],
    providers: [
        { provider: SquidexClient, useValue: Client }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
