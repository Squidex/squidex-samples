import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

import { Page } from '../services/content.service';

@Component({
    selector: 'app-page',
    templateUrl: './page.component.html',
    styleUrls: ['./page.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class PageComponent {
    @Input()
    public page: Page;
}
