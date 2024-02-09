import clickElement from "../action/clickElement.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function selectMyConsultationFilter(): Promise<void> {

	await waitForDisplayed(selectors.adminDownloadPage.myConsultationsFilter, "");
	await clickElement("click", "selector", selectors.adminDownloadPage.myConsultationsFilter);
	await pause("1000");

};

export default selectMyConsultationFilter;
