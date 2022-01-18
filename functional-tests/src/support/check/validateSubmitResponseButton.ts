import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import { checkContainsText } from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {isEnabled} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateSubmitResponseButtonInactive = () => {
	isEnabled(selectors.reviewPage.submitResponseButton, "true");
	pause("1000");
};

export const validateSubmitResponseValidationMessage = (message: string) => {
	waitForDisplayed(selectors.reviewPage.submitResponseFeedback, "false");
	checkContainsText(
		"element",
		selectors.reviewPage.submitResponseFeedback,
		"false",
		message
	);
	pause("1000");
};

export default validateSubmitResponseButtonInactive;
