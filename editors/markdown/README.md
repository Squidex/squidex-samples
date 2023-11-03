## Markdown Editor

This is the old markdown editor as editor extension. The markdown editor has been replaced with a new implementation. If you want to have the old editor back, you can use this implementation. This package can also be used to make extensions to the editor, for example to add more features or to reduce the feature set.

Read more about custom editors: https://docs.squidex.io/02-documentation/developer-guides/editors

## How to use it

Install Node LTS: https://nodejs.org/en

You have to checkout the code, then run:

```
npm i
npm run build
```

If you want to run the editor from a subfolder of your domain instead of the domains root you have to configure your base (url) inside the vite.config.js before running the build.

The build process will put all files to the dist folders. Put these files to a web server and use the URL to your webserver in the Squidex UI, for example:

```
https://your-webserver.com/markdown-editor/
```

## How to customize it?

This package is based on vite: https://vitejs.dev/

If you want to customize it just run the following commands

```
npm i
npm run dev
```

The console will give you the URL to the dev server. Whenever you make a change, the editor will recompile automatically. You can just use this URL in your Squidex field configuration.