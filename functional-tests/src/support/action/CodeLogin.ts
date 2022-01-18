import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement"
import {pause} from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export const CodeLogin = () => {

	clickElement("click", "selector", "[data-qa-sel='loginpanel-label-organisation']");
	clickElement("click", "selector", "[data-qa-sel='loginpanel-label-code']");
	waitForDisplayed("[data-qa-sel='OrganisationCodeLogin']", "false");
	setInputField("set",  "['Control', 'V']", "[data-qa-sel='OrganisationCodeLogin']");
  pause("5000");
	waitForDisplayed("[data-qa-sel='ConfirmOrgNameButton']", "false");
	clickElement("click", "selector", "[data-qa-sel='ConfirmOrgNameButton']");
	clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
};
export default CodeLogin;
