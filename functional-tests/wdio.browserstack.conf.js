var path = require("path");
// wdio.conf.js
exports.config = {
	// ...
	services: ["browserstack"],
	user: process.env.BROWSERSTACK_USERNAME,
	key: process.env.BROWSERSTACK_ACCESS_KEY,
	browserstackLocal: true,
	specs: [
		"./src/features/**/commentOnChapter.feature",
		"./src/features/**/commentOnDocument.feature",
		"./src/features/**/commentOnSection.feature",
		"./src/features/**/commentOnSubSection.feature",
	],
	capabilities: [
		// {
		// 	project: "Comment Collection",
		// 	name: "Functional tests - Chrome",
		// 	build: "Comment Collection" + "BUILD_NUMBER",
		// 	browser: "Chrome",
		// 	os: "Windows",
		// 	os_version: "10",
		// 	browser_version: "62.0",
		// 	resolution: "1024x768",
		// 	acceptInsecureCerts: true, // Because of self-signed cert inside Docker
		// 	acceptSslCerts: true,
		// 	maxInstances: 2,
		// }
		{
			project: "Comment Collection",
			name: "Functional tests - Edge",
			build:
				"Comment Collection" + " EDGE 84.0 beta " + process.env.BUILD_NUMBER,
			os: "Windows",
			os_version: "10",
			browser: "Edge",
			browser_version: "84.0 beta",
			selenium_version: "3.5.2",
			resolution: "1024x768",
			acceptInsecureCerts: true, // Because of self-signed cert inside Docker
			acceptSslCerts: true,
			maxInstances: 2,
		},
	],

	logLevel: "verbose",
	// Change this to verbose if you want more detailed logging in the terminal
	coloredLogs: true,
	screenshotPath: "./errorShots/",
	baseUrl: "https://niceorg/consultations/",
	reporters: ["spec"],

	// Use BDD with Cucumber
	framework: "cucumber",
	cucumberOpts: {
		compiler: ["js:babel-register"], // Babel so we can use ES6 in tests
		require: [
			"./src/steps/given.js",
			"./src/steps/when.js",
			"./src/steps/then.js",
		],
		tagExpression: "not @pending", // See https://docs.cucumber.io/tag-expressions/
		timeout: 30000,
	},

	// Set up global asssertion libraries
	before: function before() {
		const chai = require("chai");
		global.expect = chai.expect;
		global.assert = chai.assert;
		global.should = chai.should();
	},
};
