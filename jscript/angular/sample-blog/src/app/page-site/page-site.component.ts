import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { finalize, startWith, switchMap } from 'rxjs/operators';

import { defer, Observable } from 'rxjs';
import { ContentService, Page } from '../services/content.service';

@Component({
    selector: 'app-page-site',
    templateUrl: './page-site.component.html',
    styleUrls: ['./page-site.component.scss']
})
export class PageSiteComponent implements OnInit {
    public page$: Observable<Page | null>;

    public isLoading = false;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly contentsService: ContentService
    ) {
    }

    public ngOnInit() {
        this.page$ = this.route.paramMap.pipe(
            switchMap(params =>
                defer(() => {
                    this.isLoading = true;

                    return this.contentsService.getPage(params.get('slug')).pipe(
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
