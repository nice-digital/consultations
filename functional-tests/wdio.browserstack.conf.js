const config = require("./wdio.conf").config,
	pkg = require("./package.json");

// Assumption that running on port 8000 either on localhost:8000 or tophat:8000 in Docker
// means we need to use browserstack local
const isLocal = (process.env.BASE_URL || "").indexOf("8000") > -1;

const configOverrides = {
	user: process.env.BROWSERSTACK_USERNAME,
	key: process.env.BROWSERSTACK_ACCESS_KEY,
	specs: [
		"./test/functional/**/*.js"
	],
	maxInstances: 10,
	capabilities: [
		// Use https://www.browserstack.com/automate/webdriverio#configure-capabilities
		{
			// We use Edge as it's free, but also run the functional tests in headless Chrome in Docker
			project: "TopHat",
			name: "Functional tests - Edge",
			build: "TopHat " + pkg.version,
			browser: "Edge",
			os: "Windows",
			os_version: "10",
			browser_version: "16.0",
			resolution: "1024x768",
			"browserstack.debug": true,
			"browserstack.local": isLocal
		}
	],
	baseUrl: process.env.BASE_URL || "http://dev-tophat.nice.org.uk/",
	waitforTimeout: 10000,
	connectionRetryTimeout: 90000,
	connectionRetryCount: 3,
	services: ["browserstack"],
	browserstackLocal: true
};

exports.config = Object.assign({}, config, configOverrides);
