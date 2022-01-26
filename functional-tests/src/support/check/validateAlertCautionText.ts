import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";

export async function validateAlertCautionText(CautionText: string): Promise<void> {
	await pause("2000");
	await waitForDisplayed("[data-qa-sel='submission-alert']", "");
	await browser.pause(2000);
	await checkContainsText("element", "[data-qa-sel='submission-alert']", "false", CautionText);
	};
export default validateAlertCautionText;
