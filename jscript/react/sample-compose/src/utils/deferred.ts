export class Deferred<T> {
    public readonly promise: Promise<T>;
    private resolveFn!: (value: T | PromiseLike<T>) => void;
    private rejectFn!: (reason?: any) => void;

    public constructor() {
        this.promise = new Promise<T>((resolve, reject) => {
            this.resolveFn = resolve
            this.rejectFn = reject
        })
    }

    public reject(reason?: any) {
        this.rejectFn(reason);
        return this;
    }

    public resolve(param: T) {
        this.resolveFn(param);
        return this;
    }

    public static value<T>(value: T) {
        return new Deferred<T>().resolve(value);
    }
}