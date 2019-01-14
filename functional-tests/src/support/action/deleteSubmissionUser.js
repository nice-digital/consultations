import openWebsite from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import selectors from "../selectors";

export const deleteSubmissionUser = (userId) => {
	openWebsite("url", "admin/DeleteAllSubmissionsFromUser?userId=" + userId);
	openWebsite("url", "1/review");
	waitForVisible(selectors.reviewPage.commentTextArea);
	browser.pause(2000);

	// Given I open the url "admin/DeleteAllSubmissionsFromUser?userId=38bb6df2-9ab8-4248-bb63-251b5424711a"
	// Given I open the url "1/review"
	// When I wait on element "[data-qa-sel='Comment-text-area']" to be visible
	// And I pause for 1000ms
};

export default deleteSubmissionUser;
