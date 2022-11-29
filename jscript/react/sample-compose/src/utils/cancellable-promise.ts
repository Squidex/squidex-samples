export class CancellablePromise<T> implements Promise<T> {
    private inner: Promise<T>;
    private isCancelled = false;

    get [Symbol.toStringTag]() {
        return Object.prototype.toString.call(this.inner);
    }

    constructor(promise: Promise<T>) {
        this.inner = new Promise((resolve, reject) => {
            promise.then(value => {
                return !this.isCancelled && resolve(value);
            }, 
            reason => {
                return !this.isCancelled && reject(reason);
            });
        });
    }

    public then<TResult1 = T, TResult2 = never>(
        onfulfilled?: ((value: T) => TResult1 | PromiseLike<TResult1>) | null | undefined, 
        onrejected?: ((reason: any) => TResult2 | PromiseLike<TResult2>) | null | undefined): Promise<TResult1 | TResult2> {
        return this.inner.then(onfulfilled, onrejected);
    }

    public catch<TResult = never>(onrejected?: ((reason: any) => TResult | PromiseLike<TResult>) | null | undefined): Promise<T | TResult> {
        return this.inner.catch(onrejected);
    }

    public finally(onfinally?: (() => void) | null | undefined): Promise<T> {
        return this.inner.finally(onfinally);
    }
    
    public cancel() {
        this.isCancelled = true;
    }
}