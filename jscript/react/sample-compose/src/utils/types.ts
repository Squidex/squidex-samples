export function isArray(value: any): value is any[] {
    return Array.isArray(value);
}

export function isStringArray(value: any): value is string[] {
    return isArray(value) && !value.find(x => !isString(x));
}

export function isString(value: any): value is string {
    return typeof value === 'string' || value instanceof String;
}

export function isObject(value: any): value  is Object {
    return value && typeof value === 'object' && value.constructor === Object;
}
