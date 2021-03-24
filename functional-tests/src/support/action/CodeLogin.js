import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const CodeLogin = () => {
	browser.pause(2000);
	browser.waitForVisible("[data-qa-sel='OrganisationCodeLogin']");
	browser.click("[data-qa-sel='OrganisationCodeLogin']");
	browser.waitForVisible(".btn--cta", 40000);
	browser.click(".btn--cta");
	browser.pause(2000);
};
