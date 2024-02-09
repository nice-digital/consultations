import waitForDisplayed from "../action/waitForDisplayed.js";
import clickElement from "../action/clickElement.js"
import pause from	"../action/pause.js";

export async function CodeLogin(): Promise<void> {

	await clickElement("click", "selector", "[data-qa-sel='loginpanel-label-organisation']");
	await clickElement("click", "selector", "[data-qa-sel='loginpanel-label-code']");
	await waitForDisplayed("[data-qa-sel='OrganisationCodeLogin']", "");
	await clickElement("click", "selector", "[data-qa-sel='OrganisationCodeLogin']");
	await browser.keys(['Control', 'v']);
  await pause("5000");
	await waitForDisplayed("[data-qa-sel='ConfirmOrgNameButton']", "");
	await clickElement("click", "selector", "[data-qa-sel='ConfirmOrgNameButton']");
	await clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
};
export default CodeLogin;
