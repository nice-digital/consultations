import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible"
import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import isEnabled from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateSubmitResponseButtonInactive = () => {
	isEnabled(selectors.reviewPage.submitResponseButton, 1);
	pause(1000);
};

export const validateSubmitResponseValidationMessage = (message) => {
	waitForVisible(selectors.reviewPage.submitResponseFeedback);
	checkContainsText("element", selectors.reviewPage.submitResponseFeedback, message);
	pause(1000);
};

export default validateSubmitResponseButtonInactive;

