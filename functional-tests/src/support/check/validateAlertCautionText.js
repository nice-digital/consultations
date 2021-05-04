import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const validateAlertCautionText = (CautionText) => {
	browser.pause(2000);
	waitForVisible("[data-qa-sel='submission-alert']");
	browser.pause(2000);
	checkContainsText("element", "[data-qa-sel='submission-alert']", CautionText);
	};
export default validateAlertCautionText;
