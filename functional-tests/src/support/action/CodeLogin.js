import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import pause from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export const CodeLogin = () => {

	browser.click("[data-qa-sel='loginpanel-label-organisation']", 2000);
	browser.click("[data-qa-sel='loginpanel-label-code']");
	waitForVisible("[data-qa-sel='OrganisationCodeLogin']");
	setInputField("setValue",  ['Control', 'V'], "[data-qa-sel='OrganisationCodeLogin']");
  	pause(5000);
	waitForVisible("[data-qa-sel='ConfirmOrgNameButton']");
	browser.click("[data-qa-sel='ConfirmOrgNameButton']");
	browser.click("[data-qa-sel='open-commenting-panel']");
};
export default CodeLogin;
