// wdio config for docker/teamcity

var config = require("./wdio.conf.js").config;

config.services = [];

// Run headless on TeamCity
config.capabilities = [
     {
		 maxInstances: 2,
         browserName: "chrome",
         chromeOptions: {
             args: ["--headless", "--window-size=1366,768"]
         }
     },
    /* {
        browserName: "firefox",
        "moz:firefoxOptions": {
			args: ["-headless"],
			prefs: {
				"security.warn_viewing_mixed": false
			}
        }
    } */
];

config.reporters = ["spec", "teamcity"];

exports.config = config;
