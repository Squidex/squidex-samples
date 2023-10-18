/* eslint-disable @typescript-eslint/no-explicit-any */
import { marked } from "marked";
import './style.css';
import 'easymde/dist/easymde.min.css';
import 'font-awesome/css/font-awesome.min.css';
import SimpleMDE, { Options } from "easymde";
import { getAllowedFiles } from "./file-utils";
import { ApiHelper } from "./api-helper";

const editorSDK = new SquidexFormField();
const editorElement = document.getElementById('editor');
const apiHelper = new ApiHelper(editorSDK);

const options: Options = {
    previewRender: (text: string) => {
        return marked(text, { pedantic: true, mangle: false, headerIds: false });
    },
    autoDownloadFontAwesome: true,
    spellChecker: false,
    status: ['lines', 'words', 'cursor'],
    toolbar: [
        {
            name: 'bold',
            action: SimpleMDE.toggleBold,
            className: 'fa fa-bold',
            title: 'Bold',
        }, {
            name: 'italic',
            action: SimpleMDE.toggleItalic,
            className: 'fa fa-italic',
            title: 'Italic',
        }, {
            name: 'heading',
            action: SimpleMDE.toggleHeadingSmaller,
            className: 'fa fa-header',
            title: 'Heading',
        }, {
            name: 'quote',
            action: SimpleMDE.toggleBlockquote,
            className: 'fa fa-quote-left',
            title: 'Quote',
        }, {
            name: 'unordered-list',
            action: SimpleMDE.toggleUnorderedList,
            className: 'fa fa-list-ul',
            title: 'Generic List',
        }, {
            name: 'ordered-list',
            action: SimpleMDE.toggleOrderedList,
            className: 'fa fa-list-ol',
            title: 'Numbered List',
        },
        '|',
        {
            name: 'link',
            action: SimpleMDE.drawLink,
            className: 'fa fa-link',
            title: 'Create Link',
        }, {
            name: 'image',
            action: SimpleMDE.drawImage,
            className: 'fa fa-picture-o',
            title: 'Insert Image',
        },
        '|',
        {
            name: 'preview',
            action: SimpleMDE.togglePreview,
            className: 'fa fa-eye no-disable',
            title: 'Toggle Preview',
        }, {
            name: 'fullscreen',
            action: SimpleMDE.toggleFullScreen,
            className: 'fa fa-arrows-alt no-disable no-mobile',
            title: 'Toggle Fullscreen',
        }, {
            name: 'side-by-side',
            action: SimpleMDE.toggleSideBySide,
            className: 'fa fa-columns no-disable no-mobile',
            title: 'Toggle Side by Side',
        },
        '|',
        {
            name: 'guide',
            action: 'https://simplemde.com/markdown-guide',
            className: 'fa fa-question-circle',
            title: 'Markdown Guide',
        },
        '|',
        {
            name: 'assets',
            action: insertAssets,
            className: 'icon-assets icon-bold',
            title: 'Insert Assets',
        },
        {
            name: 'contents',
            action: insertContents,
            className: 'icon-contents icon-bold',
            title: 'Insert Contents',
        },
    ],
    element: editorElement!,
};

const simplemde = new SimpleMDE(options);

simplemde.codemirror.on('blur', () => {
    editorSDK.touched();
});

simplemde.codemirror.on('change', () => {
    const value = simplemde.value();

    editorSDK.valueChanged(value);
});

editorSDK.onDisabled(disabled => {
    simplemde.codemirror.setOption('readOnly', disabled);
});

editorSDK.onValueChanged(value => {
    simplemde.value(value || '');
});

simplemde.codemirror.on('refresh', () => {
    const isFullscreen = simplemde.isFullscreenActive();

    if (isFullscreen !== editorSDK.isFullscreen()) {
        editorSDK.toggleFullscreen();
    }
});

document.getElementById('container')?.addEventListener('drop', async event => {
    const files = await getAllowedFiles(event.dataTransfer);

    if (files.length === 0) {
        return;
    }

    const doc = simplemde.codemirror.getDoc();

    for (const file of files) {
        const uploadCursor = doc.getCursor();
        const uploadText = `![Uploading file...${new Date()}]()`;

        doc.replaceSelection(uploadText);

        const replaceText = (replacement: string) => {
            const cursor = doc.getCursor();
  
            const text = doc.getValue().replace(uploadText, replacement);
  
            doc.setValue(text);
  
            if (uploadCursor && uploadCursor.line === cursor.line) {
                const offset = replacement.length - uploadText.length;
  
                doc.setCursor({ line: cursor.line, ch: cursor.ch + offset });
            } else {
                doc.setCursor(cursor);
            }
        };

        const uploaded = await apiHelper.upload(file);

        if (uploaded != null) {
            replaceText(buildAssetMarkup(uploaded));
        } else {
            replaceText('ERROR');
        }
    }  
});

function insertAssets() {
    editorSDK.pickAssets(assets => {
        const content = buildAssetsMarkup(assets);

        if (content.length > 0) {
            simplemde.codemirror.replaceSelection(content);
        }
    });
}

function insertContents() {
    editorSDK.pickContents([], assets => {
        const content = buildContentsMarkup(assets);

        if (content.length > 0) {
            simplemde.codemirror.replaceSelection(content);
        }
    });
}

function buildAssetsMarkup(assets: ReadonlyArray<any>) {
    let markup = '';

    for (const asset of assets) {
        markup += buildAssetMarkup(asset);
    }

    return markup;
}

function buildContentsMarkup(contents: ReadonlyArray<any>) {
    let markup = '';

    for (const content of contents) {
        markup += buildContentMarkup(content);
    }

    return markup;
}

function buildContentMarkup(content: any) {
    const language = editorSDK.getLanguage()!;

    const name =
    content.referenceFields
        .map((f: any) => content.data[f.name])
        .map((f: any) => f ? f[language] || f?.iv : undefined)
        .filter((f: any) => !!f)
        .join(', ') || 'content';

    return `[${name}](${apiHelper.contentUrl(content)})`;
}

function buildAssetMarkup(asset: any) {
    let name = asset.fileName;

    const index = name.lastIndexOf('.');

    if (index > 0) {
        name = name.substring(0, index);
    }

    const url = apiHelper.assetUrl(asset);

    if (asset.type === 'Image' || asset.mimeType === 'image/svg+xml' || asset.fileName.endsWith('.svg')) {
        return `![${name}](${url} '${name}')`;
    } else if (asset.type === 'Video') {
        return `[${name}](${url})`;
    } else {
        return `[${name}](${url})`;
    }
}