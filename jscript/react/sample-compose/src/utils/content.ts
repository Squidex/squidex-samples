export function flatten(content: any) {
    if (content.flatData) {
        return content.flatData;
    } else {
        const result: any = {};

        for (const item of Object.entries(content.data)) {
            const [key, value] = item;

            result[key] = (value as any)['iv'];
        }

        return result;
    }
}