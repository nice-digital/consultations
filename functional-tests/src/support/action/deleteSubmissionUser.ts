import { openWebsite } from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export const deleteSubmissionUser = (userId) => {
	openWebsite("url", "admin/DeleteAllSubmissionsFromUser?userId=" + userId);
	openWebsite("url", "1/");
	browser.pause(15000);
	waitForDisplayed(selectors.documentPage.pageHeader, 'false');
	browser.pause(2000);
};

export default deleteSubmissionUser;
