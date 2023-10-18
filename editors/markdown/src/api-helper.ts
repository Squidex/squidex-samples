export class ApiHelper {
    private trimmedApiUrl = '';

    public get appName() {
        return this.editorSDK.getContext().appName;
    }

    public get accessToken() {
        return this.editorSDK.getContext().user.accessToken;
    }

    public get apiUrl() {
        return this.trimmedApiUrl;
    }

    constructor(
        private readonly editorSDK: SquidexFormField
    ) {

        editorSDK.onInit(() => {
            let apiUrl = editorSDK.getContext().apiUrl;

            if (apiUrl.endsWith('/')) {
                apiUrl = apiUrl.substring(0, apiUrl.length - 1);
            }

            if (apiUrl.endsWith('/api')) {
                apiUrl = apiUrl.substring(0, apiUrl.length - 4);
            }

            this.trimmedApiUrl = apiUrl;
        });
    }

    public contentUrl(content: any) {
        return `${this.apiUrl}${content._links['self'].href}`;
    }

    public assetUrl(asset: any) {
        return `${this.apiUrl}${asset._links['content'].href}`;
    }

    public async upload(file: File) {
        const uploadUrl = `${this.apiUrl}/api/apps/${this.appName}/assets`;
        const uploadToken = this.accessToken;
    
        try {
            const body = new FormData();
    
            body.append('file', file);
    
            const response = await fetch(uploadUrl, {
                method: 'POST',
                headers: {
                    Authorization: `Bearer ${uploadToken}`,
                },
                body
            });
    
            const asset = await response.json();
    
            return asset;
        } catch {
            return null;
        }
    }
}