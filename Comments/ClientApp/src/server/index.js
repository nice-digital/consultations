const path = require('path');
const rootPath = path.join(__dirname, '../../.babelrc');

// We use babel register so we can load ES6 code on the server without
// having to have an extra build step ie through webpack.

require("@babel/register")({
	extends: rootPath,
	ignore: [/node_modules/],
	cache: false
});

require("ignore-styles");

module.exports = require("./renderer");
