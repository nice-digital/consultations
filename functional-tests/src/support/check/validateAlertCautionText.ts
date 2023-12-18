import checkContainsText from "../check/checkContainsText.js";
import pause from "../action/pause.js";
import waitForDisplayed from "../action/waitForDisplayed.js";

export async function validateAlertCautionText(CautionText: string): Promise<void> {
	await pause("2000");
	await waitForDisplayed("[data-qa-sel='submission-alert']", "");
	await browser.pause(2000);
	await checkContainsText("element", "[data-qa-sel='submission-alert']", "", CautionText);
	};
export default validateAlertCautionText;
