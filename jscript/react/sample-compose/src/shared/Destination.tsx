import { flatten } from '../utils';
import { Markdown } from './Markdown';

export const Destination = ({ destination }: { destination: any }) => {
    const destinationData = flatten(destination);

    return (
        <div>
            <h2>{destinationData.name}</h2>

            <Markdown markdown={destinationData.description} />
        </div>
    );
};