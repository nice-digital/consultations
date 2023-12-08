import waitForDisplayed from "../action/waitForDisplayed";
import checkContainsText from "../check/checkContainsText";
import isEnabled from "../check/isEnabled";
import pause from "../action/pause";
import selectors from "../selectors";

export async function validateSubmitResponseButtonInactive(): Promise<void> {
	await isEnabled(selectors.reviewPage.submitResponseButton, true);
	await pause("1000");
};

export async function validateSubmitResponseValidationMessage(message: string): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.submitResponseFeedback, "");
	await checkContainsText(
		"element",
		selectors.reviewPage.submitResponseFeedback,
		null,
		message
	);
	await pause("1000");
};

export default validateSubmitResponseButtonInactive;
