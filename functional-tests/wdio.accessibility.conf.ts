const isInDocker = !!process.env.IN_DOCKER,
	isTeamCity = !!process.env.TEAMCITY_VERSION;

export const config: WebdriverIO.Config = {
	// Use devtools to control Chrome when we're running tests locally
	// Avoids issues with having the wrong ChromeDriver installed via selenium-standalone when Chrome updates every 6 weeks.
	// We need to use webdriver protocol in Docker because we use the selenium grid.
	automationProtocol: isInDocker ? "webdriver" : "devtools",

	maxInstances: 1,
	path: "/wd/hub",

	specs: ["./src/accessibilityfeatures/**/*.feature"],

	capabilities: [
		{
		acceptInsecureCerts: true, // Because of self-signed cert inside Docker
		// acceptSslCerts: true,
		maxInstances: 1,
		browserName: "chrome",
			"goog:chromeOptions": {
				args: ["--window-size=1366,768",
			// '--headless',
      '--no-sandbox',
      '--disable-gpu',
      '--disable-setuid-sandbox',
			'--ignore-certificate-errors',
      '--disable-dev-shm-usage'].concat(isInDocker ? "--headless" : []),
			},
		},
	],

	logLevel: "warn",

	baseUrl: "https://niceorg/consultations/",
	reporters: [
		"spec",
		"teamcity",
		["allure",
			{
				useCucumberStepReporter: true,
				// Turn on screenshot reporting for error shots
				disableWebdriverScreenshotsReporting: false,
			},
		]
	],

	framework: "cucumber",
	cucumberOpts: {
		require: [
			"./src/steps/**/*.ts",
			"./node_modules/@nice-digital/wdio-cucumber-steps/lib",
		],
		tagExpression: "not @pending", // See https://docs.cucumber.io/tag-expressions/
		timeout: 1500000,
	},

	afterStep: async function (_test, _scenario, { error, passed }) {
		// Take screenshots on error, these end up in the Allure reports
		var fileName = "errorShots/" + "ERROR_" + _scenario.name + ".png";
		if (error) await browser.takeScreenshot();
		if (error) await browser.saveScreenshot(fileName);
	},

	autoCompileOpts: {
		autoCompile: true,
		// see https://github.com/TypeStrong/ts-node#cli-and-programmatic-options
		// for all available options
		tsNodeOpts: {
			transpileOnly: true,
			project: "tsconfig.json",
		},
	},
};
