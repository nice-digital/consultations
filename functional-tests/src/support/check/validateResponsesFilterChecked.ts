import checkSelected from "../check/checkSelected.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function validateResponsesFilterChecked(): Promise<void> {

	await waitForDisplayed(selectors.adminDownloadPage.myConsultationsFilter, "");
	await checkSelected(selectors.adminDownloadPage.myConsultationsFilter, true);
	await pause("1000");

};

export default validateResponsesFilterChecked;
