import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function selectMyConsultationFilter(): Promise<void> {

	await waitForDisplayed(selectors.adminDownloadPage.myConsultationsFilter, "");
	await clickElement("click", "button", selectors.adminDownloadPage.myConsultationsFilter);
	await pause("1000");

};

export default selectMyConsultationFilter;
