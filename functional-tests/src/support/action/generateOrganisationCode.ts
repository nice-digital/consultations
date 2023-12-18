import waitForDisplayed from "../action/waitForDisplayed.js";
import clickElement from "../action/clickElement.js";
import pause from	"../action/pause.js";
import pressButton from "../action/pressButton.js";

export async function generateOrganisationCode(): Promise<void> {

	await clickElement("click", "selector", "[data-qa-sel='share-with-org-button']");
	await clickElement("click", "selector", "[data-qa-sel='generate-code-button']");
	await waitForDisplayed("[data-qa-sel='copy-code-button']", "");
	await clickElement("click", "selector", "[data-qa-sel='copy-code-button']");
	await pressButton("['CTRL', 'C']");
	await pause("2000");
	await clickElement("click", "selector", "#my-account-button");
	await pause("2000")
	// await clickElement("click", "selector", ".gn_2nlnx");
	await clickElement("click", "selector", "a[href='/consultations/account/logout']");
	await pause("2000");
};
