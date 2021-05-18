import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const CodeLogin = () => {

	browser.refresh();
	browser.pause(2000);
	browser.waitForVisible("[for='respondingAsOrg--organisation']");
	browser.click("[for='respondingAsOrg--organisation']");
	browser.pause(2000);
	browser.waitForVisible("[for='respondingAsOrgType--code']");
	browser.click("[for='respondingAsOrgType--code']");
	browser.waitForVisible("[data-qa-sel='OrganisationCodeLogin']");
	browser.pause(5000);
	browser.setValue('input#orgCode-undefined', ['Control', 'V'])
	browser.pause(5000);
	browser.click(".btn--cta");
	browser.pause(2000);
	waitForVisible("[data-qa-sel='open-commenting-panel']");
  browser.click("[data-qa-sel='open-commenting-panel']");
};
export default CodeLogin;
