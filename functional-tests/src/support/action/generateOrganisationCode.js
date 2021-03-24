import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const generateOrganisationCode = () => {
	browser.pause(2000);
	browser.click("[data-qa-sel='share-with-org-button']");
	browser.click("[data-qa-sel='generate-code-button']");
	browser.waitForVisible("[data-qa-sel='copy-code-button']");
	browser.click("[data-qa-sel='copy-code-button']");
	browser.pause(2000);
	browser.click(".my-account-button");
	browser.waitForExist(
		"body #header-menu a[href*='/consultations/account/logout']",
		20000);
		browser.click(
			"body #header-menu a[href*='/consultations/account/logout']",
			20000);
};
