import openWebsite from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import selectors from "../selectors";

export const deleteSubmissionUser = (userId) => {
	openWebsite("url", "admin/DeleteAllSubmissionsFromUser?userId=" + userId);
	openWebsite("url", "1/");
	browser.pause(15000);
	waitForVisible(selectors.documentPage.pageHeader);
	browser.pause(2000);
};

export default deleteSubmissionUser;
