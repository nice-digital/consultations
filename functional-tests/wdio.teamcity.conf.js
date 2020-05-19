// wdio config for docker/teamcity

var config = require("./wdio.conf.js").config;

config.services = [];

// Run headless on TeamCity
config.capabilities = [
	{
		acceptInsecureCerts: true, // Because of self-signed cert inside Docker
	 	acceptSslCerts: true,
		maxInstances: 2,
		browserName: "chrome",
		'goog:chromeOptions': {
			'w3c': false,
			args: ["--headless", "--window-size=1366,768"]
		}
	}//,
	// {
	// 	acceptInsecureCerts: true, // Because of self-signed cert inside Docker
	// 	acceptSslCerts: true,
    //     browserName: "firefox",
    //     "moz:firefoxOptions": {
	// 		args: ["-headless"]
    //     }
    // }
];

config.reporters = ["spec", "teamcity"];

exports.config = config;
