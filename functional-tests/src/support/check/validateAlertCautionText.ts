import checkContainsText from "../check/checkContainsText";
import pause from "../action/pause";
import waitForDisplayed from "../action/waitForDisplayed";

export async function validateAlertCautionText(CautionText: string): Promise<void> {
	await pause("2000");
	await waitForDisplayed("[data-qa-sel='submission-alert']", "");
	await browser.pause(2000);
	await checkContainsText("element", "[data-qa-sel='submission-alert']", null, CautionText);
	};
export default validateAlertCautionText;
