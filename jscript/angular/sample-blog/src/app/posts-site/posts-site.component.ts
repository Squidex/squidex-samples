import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ContentService, Post } from '../services/content.service';

@Component({
  selector: 'app-posts-site',
  templateUrl: './posts-site.component.html',
  styleUrls: ['./posts-site.component.scss']
})
export class PostsSiteComponent implements OnInit {
    public posts$: Observable<Post[]>;

    constructor(
        public readonly contentsService: ContentService
    ) {
    }

    public ngOnInit() {
        this.posts$ = this.contentsService.getPosts().pipe(
            map(x => x.posts)
        );
    }
}
