import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import selectors from "../selectors";

export const navigateToReviewPage = () => {
	clickElement("click", "button", selectors.documentPage.reviewAllButton);
	browser.pause(1000);
	browser.waitForVisible(selectors.reviewPage.commentTextArea, 2000);
};

export default navigateToReviewPage;

