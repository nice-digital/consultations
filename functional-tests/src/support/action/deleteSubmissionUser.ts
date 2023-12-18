import openWebsite from "../action/openWebsite.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import selectors from "../selectors.js";

export async function deleteSubmissionUser(userId: string, returnUrl: string): Promise<void> {
	await openWebsite("url", "admin/DeleteAllSubmissionsFromUser?userId=" + process.env[userId]);
	await openWebsite("url", returnUrl);
	await browser.refresh();
	await browser.pause(15000);
	await waitForDisplayed(selectors.documentPage.pageHeader, "");
	await browser.pause(2000);
};

export default deleteSubmissionUser;
