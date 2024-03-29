import { hooks } from './src/support/hooks.js';
// const isInDocker = !!process.env.IN_DOCKER,
// 	isTeamCity = !!process.env.TEAMCITY_VERSION;

export const config: WebdriverIO.Config = {
	// Use devtools to control Chrome when we're running tests locally
	// Avoids issues with having the wrong ChromeDriver installed via selenium-standalone when Chrome updates every 6 weeks.
	// We need to use webdriver protocol in Docker because we use the selenium grid.
	automationProtocol: "webdriver",

	runner:'local',
	// hostname: 'selenium-hub',
	// port: 4444,
	path: "/wd/hub",
	maxInstances: 1,
	// services: ['devtools'],

	specs: [
					"./src/features/**/*.feature"
					// "./src/features/**/answerQuestion.feature",
					//"./src/features/**/closedForCommenting.feature",
					//"./src/features/**/codeuserAddCommentSubmitResponseMessage.feature",
					//"./src/features/**/commentOnDocumentChapter.feature",
					// "./src/features/**/commentOnSection.feature",
					// "./src/features/**/filterAdminPageByTitleStatusGIDNumberofResults.feature",
					// "./src/features/**/filterAdminPagePreSelectedExternal.feature",
					// "./src/features/**/hiddenConsultation.feature"
					// "./src/features/**/orderingOnReviewPage.feature",
					// "./src/features/**/paginationOnAdminPage.feature",
					// "./src/features/**/submitComments.feature",
					// "./src/features/**/submitResponseNoManQuestions.feature"
					// "./src/features/**/submitResponseSupportQualityStandardMan.feature",
					// "./src/features/**/unhiddenConsultation.feature",
					// "./src/features/**/unsavedCommentDocPage.feature",
					// "./src/features/**/unsavedCommentReviewPage.feature"
				],
	specFileRetries: 1,
	specFileRetriesDelay: 2,
	specFileRetriesDeferred: true,
	capabilities: [
		{
			acceptInsecureCerts: true, // Because of self-signed cert inside Docker
			// acceptSslCerts: true,
			maxInstances: 1,
			browserName: "chrome",
			"goog:chromeOptions": {
				args: ['--disable-web-security', /*'--headless',*/ '--disable-dev-shm-usage', '--no-sandbox', '--window-size=1920,1080']
			},
		},
		{

        // maxInstances can get overwritten per capability. So if you have an in-house Selenium
        // grid with only 5 firefox instances available you can make sure that not more than
        // 5 instances get started at a time.
        maxInstances: 2,
        //
        browserName: 'firefox',
				specs: ["./src/features/**/answerQuestion.feature",
					"./src/features/**/closedForCommenting.feature",
					"./src/features/**/commentOnDocumentChapter.feature",
					"./src/features/**/commentOnSection.feature",],
        // 'moz:firefoxOptions': {
        //     args: ['-headless']
        // },
        acceptInsecureCerts: true
        // If outputDir is provided WebdriverIO can capture driver session logs
        // it is possible to configure which logTypes to include/exclude.
        // excludeDriverLogs: ['*'], // pass '*' to exclude all driver session logs
        // excludeDriverLogs: ['bugreport', 'server'],
    },
		{

        // maxInstances can get overwritten per capability. So if you have an in-house Selenium
        // grid with only 5 firefox instances available you can make sure that not more than
        // 5 instances get started at a time.
        maxInstances: 2,
        //
        browserName: 'MicrosoftEdge',
				specs: ["./src/features/**/answerQuestion.feature",
					"./src/features/**/closedForCommenting.feature",
					"./src/features/**/commentOnDocumentChapter.feature",
					"./src/features/**/commentOnSection.feature",],
        // 'moz:firefoxOptions': {
        //     args: ['-headless']
        // },
        acceptInsecureCerts: true
        // If outputDir is provided WebdriverIO can capture driver session logs
        // it is possible to configure which logTypes to include/exclude.
        // excludeDriverLogs: ['*'], // pass '*' to exclude all driver session logs
        // excludeDriverLogs: ['bugreport', 'server'],
    }
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
						'./src/steps/given.ts',
            './src/steps/then.ts',
            './src/steps/when.ts',
		],
		tags: "not @pending", // See https://docs.cucumber.io/tag-expressions/
		timeout: 1500000,
	},

	afterHook: async function (_test, _scenario, { error, passed }) {
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
