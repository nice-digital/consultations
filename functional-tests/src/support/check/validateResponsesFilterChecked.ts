import checkSelected from "../check/checkSelected";
import waitForDisplayed from "../action/waitForDisplayed";
import pause from "../action/pause";
import selectors from "../selectors";

export async function validateResponsesFilterChecked(): Promise<void> {

	await waitForDisplayed(selectors.adminDownloadPage.myConsultationsFilter, "");
	await checkSelected(selectors.adminDownloadPage.myConsultationsFilter, true);
	await pause("1000");

};

export default validateResponsesFilterChecked;
