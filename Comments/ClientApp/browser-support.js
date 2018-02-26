/* eslint-disable */

// This script gets Respond.JS, HTML5Shiv, ES5-Shim and ES5-Sham from node_modules and concats them into a file 'browser-support.js' in 'public/vendor/'.

const concat = require("concat-files");
const destination = './public/vendor/browser-support.js';

concat(
	[   "./node_modules/respond.js/dest/respond.min.js",
		"./node_modules/html5shiv/dist/html5shiv.min.js",
		"./node_modules/es5-shim/es5-shim.min.js",
		"./node_modules/es5-shim/es5-sham.min.js" ],
	    destination, function(err) {
		if (err) throw err;
		console.log(`Concat of supporting files written to ${destination}`);
	}
);
