import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ContentService, Page } from '../services/content.service';

@Component({
    selector: 'app-top-nav',
    templateUrl: './top-nav.component.html',
    styleUrls: ['./top-nav.component.scss']
})
export class TopNavComponent implements OnInit {
    public pages$: Observable<Page[]>;

    constructor(
        public readonly contentsService: ContentService
    ) {
    }

    public ngOnInit() {
        this.pages$ = this.contentsService.getPages().pipe(
            map(x => x.pages)
        );
    }
}
