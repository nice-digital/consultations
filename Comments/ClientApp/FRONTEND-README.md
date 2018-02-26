# Front End Readme

## Browser support additions (Modernizr etc)

Running `npm run supporting-js` will do two things:

- It will build a file `modernizr-custom.js` into the `public/vendor` folder, based on the settings in `modernizr-config.json` in the root of `ClientApp`. 
- It will run `browser-support.js` which will concat the minified versions of:
    - HTML5Shiv
    - Respond.js
    - ES5 Shim / Sham into `public/vendor/browser-support.js`.
    
This runs as part of both `npm start` and `npm run build`.