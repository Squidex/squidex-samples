# React/Javascript Blog Sample

You can create an app with prepulated schemas directly in Squidex.

## How to run it with a custom app?

Have a look to the configuration first: [configuration](src/service/index.js)

You have to change the following settings:

* `appName`: The name of your app.
* `clientId`: The client id, usually: '`appName`:default'
* `clientSecret`: The secret for your client. You can get it from the Squidex UI.

Optionally:

* `url`: The url to your squidex instance, e.g. `http://localhost:5000` if you run it locally.#

## How to start it

### Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).
