import { useCallback, useEffect, useMemo, useState } from 'react'
import slate from '@react-page/plugins-slate';
import image from '@react-page/plugins-image';
import Editor, { Value } from '@react-page/editor';
import PageLayout from '../../shared/PageLayout';
import { ContentRenderers, DynamicContentCache, EditorContext, NoopContentCache, resolveIds } from './EditorContext';
import { squidexImage } from './SquidexImage';
import { squidexContent } from './SquidexContent';

const cellPlugins = [slate(), image, squidexImage, squidexContent];

const contentRenderers: ContentRenderers = {
    'hotel': ({ content }) => {
        return (
            <div>
                CONTENT: {content.data.title.iv}
            </div>
        )
    },
    'destination': ({ content }) => {
        return (
            <div>
                DESTINATION: {content.data.title.iv}
            </div>
        )
    }
}

const EditorPage = () => {
    const [value, setValue] = useState<Value | null>(null);
    const [fieldDisabled, setFieldDisabled] = useState<boolean>(false);
    const [fieldEditor, setFieldEditor] = useState<SquidexFormField | undefined>(undefined);

    useEffect(() => {
        try {
            const field = new SquidexFormField();

            field.onValueChanged((value) => {
                setValue(value);
            });

            field.onDisabled((isDisabled) => {
                setFieldDisabled(isDisabled);
            });

            field.onInit(() => {
                setFieldEditor(field);
            });
        } catch (ex) {
            console.log(`Not embedded in Squidex: ${ex}`);
        }
    }, []);

    const setActualValue = useCallback((value: any) => {
        if (!fieldEditor) {
            return;
        }

        fieldEditor.valueChanged(value);
        fieldEditor.touched();
        setValue(value);
    }, [fieldEditor]);

    const contentCache = useMemo(() => {
        if (!fieldEditor) {
            return new NoopContentCache();
        } else {
            const context = fieldEditor.getContext();

            return new DynamicContentCache(
                context.apiUrl,
                context.user.accessToken,
                context.appName);
        }
    }, [fieldEditor]);

    useEffect(() => {
        const ids = resolveIds(value);

        contentCache.load(ids);
    }, [contentCache, value]);

    return (
        <PageLayout>
            <EditorContext.Provider value={{ 
                contentCache,
                contentRenderers,
                field: fieldEditor 
            }}>
                <div className={fieldDisabled ? 'disabled' : 'none'}>
                    <Editor cellPlugins={cellPlugins} value={value} onChange={setActualValue} />
                </div>
            </EditorContext.Provider>
        </PageLayout>
    );
};

export default EditorPage