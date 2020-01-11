import { Component, Input } from '@angular/core';

import { Post } from '../services/content.service';

@Component({
    selector: 'app-post',
    templateUrl: './post.component.html',
    styleUrls: ['./post.component.scss']
})
export class PostComponent {
    @Input()
    public post: Post;

    @Input()
    public withLink: boolean;
}
