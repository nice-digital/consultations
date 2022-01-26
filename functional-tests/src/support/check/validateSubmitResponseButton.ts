import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import { checkContainsText } from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {isEnabled} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function validateSubmitResponseButtonInactive(): Promise<void> {
	await isEnabled(selectors.reviewPage.submitResponseButton, "true");
	await pause("1000");
};

export async function validateSubmitResponseValidationMessage(message: string): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.submitResponseFeedback, "");
	await checkContainsText(
		"element",
		selectors.reviewPage.submitResponseFeedback,
		"false",
		message
	);
	await pause("1000");
};

export default validateSubmitResponseButtonInactive;
