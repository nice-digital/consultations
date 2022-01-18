import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {pressButton} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pressButton";

export const generateOrganisationCode = () => {

	clickElement("click", "selector", "[data-qa-sel='share-with-org-button']");
	clickElement("click", "selector", "[data-qa-sel='generate-code-button']");
	waitForDisplayed("[data-qa-sel='copy-code-button']", "false");
	clickElement("click", "selector", "[data-qa-sel='copy-code-button']");
	pressButton("['CTRL', 'C']");
	pause("2000");
	clickElement("click", "selector", ".gn_2hlYN");
	clickElement("click", "selector", ".gn_2nlnx");
	clickElement("click", "selector", ".gn_2nlnx a[href='/consultations/account/logout']");
	pause("2000");
};
