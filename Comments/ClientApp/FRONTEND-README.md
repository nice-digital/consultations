# Front End Readme

## Getting it running

If you only want to run the front end you can do so (at the moment) by proxying API requests to the test server (meaning that you don't need to run the backend to test the frontend).

There's a variable at the bottom of `/package.json` of `proxy`. Set this to `http://test.nice.org.uk` to send API requests to the test server.

So it should look like..

```
// package.json
"proxy": "http://test.nice.org.uk"
```

1. Make sure Node is installed on your system [Node Website](https://nodejs.org)
- Open your command prompt at `/Comments/ClientApp/`
- Given you have Node installed, install necessary dependencies with `npm install`
- Run the test server with `npm run start`

-

### Modernizr

`npm run modernizr` will build a file `modernizr-custom.js` into the `public/vendor` folder, based on the settings in `modernizr-config.json` in the root of `ClientApp`.

The resultant file `public/vendor/modernizr-custom.js` is excluded from version control and is generated at each `npm start` or `npm run build`.
