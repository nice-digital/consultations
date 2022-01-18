import {checkSelected} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkSelected";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateResponsesFilterChecked = () => {
	waitForDisplayed(selectors.adminDownloadPage.yourResponsesFilter, "false");
	checkSelected(selectors.adminDownloadPage.yourResponsesFilter, "false");
	pause("1000");
};

export default validateResponsesFilterChecked;
