import waitForDisplayed from "../action/waitForDisplayed.js";
import checkContainsText from "../check/checkContainsText.js";
import isEnabled from "../check/isEnabled.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function validateSubmitResponseButtonInactive(): Promise<void> {
	await isEnabled(selectors.reviewPage.submitResponseButton, true);
	await pause("1000");
};

export async function validateSubmitResponseValidationMessage(message: string): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.submitResponseFeedback, "");
	await checkContainsText(
		"element",
		selectors.reviewPage.submitResponseFeedback,
		"",
		message
	);
	await pause("1000");
};

export default validateSubmitResponseButtonInactive;
