import clickElement from "../action/clickElement";
import waitForDisplayed from "../action/waitForDisplayed";
import pause from "../action/pause";
import selectors from "../selectors";

export async function selectMyConsultationFilter(): Promise<void> {

	await waitForDisplayed(selectors.adminDownloadPage.myConsultationsFilter, "");
	await clickElement("click", "selector", selectors.adminDownloadPage.myConsultationsFilter);
	await pause("1000");

};

export default selectMyConsultationFilter;
