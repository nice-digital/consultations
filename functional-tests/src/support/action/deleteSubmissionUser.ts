import { openWebsite } from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import { refresh } from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export async function deleteSubmissionUser(userId: string): Promise<void> {
	await openWebsite("url", "admin/DeleteAllSubmissionsFromUser?userId=" + userId);
	await openWebsite("url", "409/");
	await browser.refresh();
	await browser.pause(15000);
	await waitForDisplayed(selectors.documentPage.pageHeader, "");
	await browser.pause(2000);
};

export default deleteSubmissionUser;
