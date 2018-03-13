var config = require("./wdio.conf.js").config;

// Assume Chrome installed on TC agent
config.port = "9515";
config.path = "/";
config.services = ["chromedriver"];
config.capabilities = [
    {
        browserName: "chrome"
    }
];

config.reporters = ["teamcity"];

exports.config = config;