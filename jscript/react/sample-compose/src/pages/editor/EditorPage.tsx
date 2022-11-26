import { useEffect, useRef, useState } from 'react'
import slate from '@react-page/plugins-slate';
import image from '@react-page/plugins-image';
import Editor, { Value } from '@react-page/editor';
import PageLayout from '../../shared/PageLayout';

const cellPlugins = [slate(), image];

const EditorPage = () => {
    const [value, setValue] = useState<Value | null>(null);
    const [isDisabled, setDisabled] = useState<boolean>(false);
    const fieldForm = useRef<SquidexFormField | undefined>();

    useEffect(() => {
        try {
            const field = new SquidexFormField();

            field.onValueChanged((value) => {
                setValue(value);
            });

            field.onDisabled((isDisabled) => {
                setDisabled(isDisabled);
            });

            fieldForm.current = field;
        } catch {
            console.log('Not embedded in Squidex.');
        }
    }, []);

    useEffect(() => {
        if (!fieldForm.current) {
            return;
        }

        fieldForm.current.valueChanged(value);
        fieldForm.current.touched();
    }, [value]);

    return (
        <PageLayout>
            <div className={isDisabled ? 'disabled' : 'none'}>
                <Editor cellPlugins={cellPlugins} value={value} onChange={setValue} />
            </div>
        </PageLayout>
    );
};

export default EditorPage