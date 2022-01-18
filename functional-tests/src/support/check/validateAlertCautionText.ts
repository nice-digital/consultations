import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";

export const validateAlertCautionText = (CautionText) => {
	pause("2000");
	waitForDisplayed("[data-qa-sel='submission-alert']", "false");
	browser.pause(2000);
	checkContainsText("element", "[data-qa-sel='submission-alert']", "false", CautionText);
	};
export default validateAlertCautionText;
