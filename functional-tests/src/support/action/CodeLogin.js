import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const CodeLogin = () => {

	browser.pause(2000);
	browser.waitForVisible("[data-qa-sel='OrganisationCodeLogin']");
	browser.pause(5000);
	browser.setValue('input#orgCode-document', ['Control', 'V'])
	browser.pause(5000);
	browser.click(".btn--cta");
	browser.pause(2000);
};
export default CodeLogin;
