import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import selectors from "../selectors";

export const navigateToReviewPage = () => {
	clickElement("click", "button", selectors.documentPage.reviewAllButton);
	browser.pause(2000);
	waitForVisible(selectors.reviewPage.commentTextArea);
	waitForVisible(selectors.reviewPage.answerNoRepresentOrg);
	waitForVisible(selectors.reviewPage.answerNoTobacLink);
};

export default navigateToReviewPage;

