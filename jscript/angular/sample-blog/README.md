# React/Angular Blog Sample

You can create an app with prepulated schemas directly in Squidex.

## How to run it with a custom app?

Have a look to the configuration first: [configuration](src/app/service/config.ts)

You have to change the following settings:

* `appName`: The name of your app.
* `clientId`: The client id, usually: '`appName`:default'
* `clientSecret`: The secret for your client. You can get it from the Squidex UI.

Optionally:

* `url`: The url to your squidex instance, e.g. `http://localhost:5000` if you run it locally.

## How to start it

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 8.3.22.

### Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

### Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

### Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.

### Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

### Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

### Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
