import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PageSiteComponent } from './page-site/page-site.component';
import { PageComponent } from './page/page.component';
import { PostSiteComponent } from './post-site/post-site.component';
import { PostComponent } from './post/post.component';
import { PostsSiteComponent } from './posts-site/posts-site.component';
import { TopNavComponent } from './top-nav/top-nav.component';

import { Config, DefaultConfig } from './services/config';

import { AuthInterceptor } from './services/auth.interceptor';

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
        HttpClientModule,
        AppRoutingModule
    ],
    providers: [
        { provide: Config, useValue: DefaultConfig },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
