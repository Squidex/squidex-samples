import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { defer, Observable } from 'rxjs';
import { finalize, startWith, switchMap } from 'rxjs/operators';

import { ContentService, Post } from '../services/content.service';

@Component({
  selector: 'app-post-site',
  templateUrl: './post-site.component.html',
  styleUrls: ['./post-site.component.scss']
})
export class PostSiteComponent implements OnInit {
    public post$: Observable<Post | null>;

    public isLoading = false;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly contentsService: ContentService
    ) {
    }

    public ngOnInit() {
        this.post$ = this.route.paramMap.pipe(
            switchMap(params =>
                defer(() => {
                    this.isLoading = true;

                    return this.contentsService.getPost(params.get('id')).pipe(
                        startWith(null),
                        finalize(() => {
                            this.isLoading = false;
                        })
                    );
                })
            )
        );
    }
}
