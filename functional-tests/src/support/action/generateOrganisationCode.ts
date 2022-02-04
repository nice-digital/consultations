import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {pressButton} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pressButton";

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
