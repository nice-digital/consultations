import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const generateOrganisationCode = () => {
	browser.pause(2000);
	browser.click("[data-qa-sel='share-with-org-button']");
	browser.waitForVisible("[data-qa-sel='generate-code-button']");
	browser.click("[data-qa-sel='generate-code-button']");
	browser.pause(2000);
	browser.waitForVisible("[data-qa-sel='copy-code-button']");
	browser.click("[data-qa-sel='copy-code-button']");
	browser.keys(['CTRL', 'C'])
	browser.pause(2000);
	browser.click(".gn_2hlYN")
	browser.click(".gn_2nlnx")
	browser.click(".gn_2nlnx a[href='/consultations/account/logout']", 20000)
};
