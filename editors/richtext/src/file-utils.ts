/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/ban-types */

export async function getAllowedFiles(dataTransfer: DataTransfer | null) {
    const files: File[] = [];

    if (!dataTransfer) {
        return files;
    }
  
    const items = getItems(dataTransfer);
  
    // Loop over files first, otherwise Chromes deletes them in the async call.
    for (const item of items) {
        const file = item.getAsFile();
  
        if (file) {
            files.push(file);
        }
    }
  
    for (const item of items) {
        if (isFunction(item['webkitGetAsEntry'])) {
            const webkitEntry = item.webkitGetAsEntry();
  
            if (webkitEntry && webkitEntry.isDirectory) {
                await traverseWebkitTree(webkitEntry, files);
            }
        }
    }
    
    return files;
}
  
async function traverseWebkitTree(item: any, files: File[]) {
    if (item.isFile) {
        const file = await getFilePromise(item);
  
        if (file) {
            files.push(file);
        }
    } else if (item.isDirectory) {
        const entries = await getFilesPromise(item);
  
        for (const entry of entries) {
            await traverseWebkitTree(entry, files);
        }
    }
}
  
function getFilesPromise(item: any): Promise<ReadonlyArray<any>> {
    return new Promise((resolve, reject) => {
        try {
            const reader = item.createReader();
  
            reader.readEntries(resolve);
        } catch (ex) {
            reject(ex);
        }
    });
}
  
function getFilePromise(item: any): Promise<File> {
    return new Promise((resolve, reject) => {
        try {
            item.file(resolve);
        } catch (ex) {
            reject(ex);
        }
    });
}
  
function getItems(dataTransfer: DataTransfer) {
    const result: DataTransferItem[] = [];
  
    if (dataTransfer.files) {
        for (let i = 0; i < dataTransfer.items.length; i++) {
            const item = dataTransfer.items[i];
  
            if (item) {
                result.push(item);
            }
        }
    }
  
    return result;
}

function isFunction(value: any): value is Function {
    return typeof value === 'function';
}