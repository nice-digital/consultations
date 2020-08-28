import checkSelected from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkSelected";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateResponsesFilterChecked = () => {
	waitForVisible(selectors.adminDownloadPage.yourResponsesFilter);
	checkSelected(selectors.adminDownloadPage.yourResponsesFilter);
	pause(1000);
};

export default validateResponsesFilterChecked;
