import { useState } from 'react'
import slate from '@react-page/plugins-slate';
import image from '@react-page/plugins-image';
import Editor, { Value } from '@react-page/editor';
import PageLayout from '../../shared/PageLayout';

const cellPlugins = [slate(), image];

const EditorPage = () => {
    const [value, setValue] = useState<Value | null>(null);

    return (
        <PageLayout>
            <Editor cellPlugins={cellPlugins} value={value} onChange={setValue} />
        </PageLayout>
    );
};

export default EditorPage