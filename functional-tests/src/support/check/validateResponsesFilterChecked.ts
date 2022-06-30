import {checkSelected} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkSelected";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function validateResponsesFilterChecked(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.yourResponsesFilter, "");
	await checkSelected(selectors.adminDownloadPage.yourResponsesFilter, "false");
	await pause("1000");
};

export default validateResponsesFilterChecked;
