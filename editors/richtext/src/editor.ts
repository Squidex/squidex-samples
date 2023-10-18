import { ApiHelper } from "./api-helper";
import tinymce, { Editor, RawEditorSettings } from 'tinymce';
import 'tinymce/icons/default/icons';
import 'tinymce/plugins/advlist';
import 'tinymce/plugins/code';
import 'tinymce/plugins/image';
import 'tinymce/plugins/link';
import 'tinymce/plugins/lists';
import 'tinymce/plugins/media';
import 'tinymce/plugins/paste';
import 'tinymce/themes/silver';
import 'tinymce/skins/ui/oxide/skin.css';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
type Any = any;

const editorSDK = new SquidexFormField();
const editorElement = document.getElementById('editor');
const apiHelper = new ApiHelper(editorSDK);

let tinyEditor: Editor | undefined = undefined;
let tinyValue: string | undefined = undefined;

const options: RawEditorSettings = {
    convert_fonts_to_spans: true,
    convert_urls: false,
    paste_data_images: true,
    plugins: 'code image media link lists advlist paste',
    min_height: 400,
    max_height: 800,
    removed_menuitems: 'newdocument',
    resize: true,
    toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter | bullist numlist outdent indent | link image media | assets contents',

    images_upload_handler: async (blob: Any, success: (url: string) => void, failure: (message: string) => void) => {
        const file = new File([blob.blob()], blob.filename(), { lastModified: new Date().getTime() });

        const uploaded = await apiHelper.upload(file);

        if (uploaded != null) {
            success(apiHelper.assetUrl(uploaded));
        } else {
            failure('Failed');
        }
    },

    setup: (editor: Editor) => {
        editor.ui.registry.addButton('assets', {
            onAction: insertAssets,
            icon: 'gallery',
            text: '',
            tooltip: 'Insert Assets',
        });

        editor.ui.registry.addButton('contents', {
            onAction: insertContents,
            icon: 'duplicate',
            text: '',
            tooltip: 'Insert Contents',
        });

        editor.on('init', () => {
            tinyEditor = editor;

            setContent();
            setReadOnly();
        });

        editor.on('change', () => {
            onValueChanged();
        });

        editor.on('paste', (event: ClipboardEvent) => {
            let hasFileDropped = false;

            if (event.clipboardData) {
                for (let i = 0; i < event.clipboardData.items.length; i++) {
                    const file = event.clipboardData.items[i].getAsFile();

                    if (file) {
                        uploadFile(file);

                        hasFileDropped = true;
                    }
                }
            }

            if (!hasFileDropped) {
                onValueChanged();
            } else {
                return false;
            }

            return undefined;
        });

        editor.on('drop', (event: DragEvent) => {
            let hasFileDropped = false;

            if (event.dataTransfer) {
                for (let i = 0; i < event.dataTransfer.files.length; i++) {
                    const file = event.dataTransfer.files.item(i);

                    if (file) {
                        uploadFile(file);

                        hasFileDropped = true;
                    }
                }
            }

            if (!hasFileDropped) {
                onValueChanged();
            }

            return false;
        });

        editor.on('blur', () => {
            editorSDK.touched();
        });
    },

    target: editorElement!,
};

tinymce.init(options);

function onValueChanged() {
    if (!tinyEditor) {
        return;
    }

    const value = tinyEditor.getContent();

    if (tinyValue == value) {
        return;
    }

    tinyValue = value;

    editorSDK.valueChanged(value);
}

function setContent() {
    const value = editorSDK.getValue();

    if (tinyValue === value) {
        return;
    }

    tinyValue = value;

    if (tinyEditor && tinyEditor.initialized) {
        tinyEditor.setContent(value || '');
    }
}

function setReadOnly() {
    if (tinyEditor && tinyEditor.initialized) {
        tinyEditor.setMode(editorSDK.isDisabled() ? 'readonly' : 'design');
    }
}

editorSDK.onDisabled(() => {
    setReadOnly();
});

editorSDK.onValueChanged(() => {
    setContent();
});

function uploadFile(file: File) {
    if (!tinyEditor) {
        return;
    }

    const uploadText = `[Uploading file...${new Date()}]`;

    tinyEditor.execCommand('mceInsertContent', false, uploadText);

    const replaceText = (replacement: string) => {
        const content = tinyEditor!.getContent().replace(uploadText, replacement);

        tinyEditor!.setContent(content);
    };

    const uploaded = apiHelper.upload(file);

    if (uploaded != null) {
        replaceText(buildAssetMarkup(uploaded));
    } else {
        replaceText('FAILED');
    }
}

function insertAssets() {
    editorSDK.pickAssets(assets => {
        const content = buildAssetsMarkup(assets);

        if (content.length > 0) {
            tinyEditor!.execCommand('mceInsertContent', false, content);
        }
    });
}

function insertContents() {
    editorSDK.pickContents([], assets => {
        const content = buildContentsMarkup(assets);

        if (content.length > 0) {
            tinyEditor!.execCommand('mceInsertContent', false, content);
        }
    });
}

function buildAssetsMarkup(assets: ReadonlyArray<Any>) {
    let markup = '';

    for (const asset of assets) {
        markup += buildAssetMarkup(asset);
    }

    return markup;
}

function buildContentsMarkup(contents: ReadonlyArray<unknown>) {
    let markup = '';

    for (const content of contents) {
        markup += buildContentMarkup(content);
    }

    return markup;
}

function buildContentMarkup(content: Any) {
    const language = editorSDK.getLanguage()!;

    let name =
        content.referenceFields
            .map((f: Any) => content.data[f.name])
            .map((f: Any) => f ? f[language] || f?.iv : undefined)
            .filter((f: Any) => !!f)
            .join(', ');

    name ||= 'Content';

    return `<a href="${apiHelper.contentUrl(content)}}" alt="${name}">${name}</a>`;
}

function buildAssetMarkup(asset: Any) {
    let name = asset.fileName;

    const index = name.lastIndexOf('.');

    if (index > 0) {
        name = name.substring(0, index);
    }

    const url = apiHelper.assetUrl(asset);

    if (asset.type === 'Image' || asset.mimeType === 'image/svg+xml' || asset.fileName.endsWith('.svg')) {
        return `<img src="${url}" alt="${name}" />`;
    } else if (asset.type === 'Video') {
        return `<video src="${url}" />`;
    } else {
        return `<a href="${url}" alt="${name}">${name}</a>`;
    }
}