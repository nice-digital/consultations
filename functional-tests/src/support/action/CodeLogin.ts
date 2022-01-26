import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement"
import {pause} from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";

export async function CodeLogin(): Promise<void> {

	await clickElement("click", "selector", "[data-qa-sel='loginpanel-label-organisation']");
	await clickElement("click", "selector", "[data-qa-sel='loginpanel-label-code']");
	await waitForDisplayed("[data-qa-sel='OrganisationCodeLogin']", "");
	await setInputField("set",  "['Control', 'V']", "[data-qa-sel='OrganisationCodeLogin']");
  await pause("5000");
	await waitForDisplayed("[data-qa-sel='ConfirmOrgNameButton']", "");
	await clickElement("click", "selector", "[data-qa-sel='ConfirmOrgNameButton']");
	await clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
};
export default CodeLogin;
