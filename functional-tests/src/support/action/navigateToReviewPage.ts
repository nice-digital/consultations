import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import selectors from "../selectors";

export const navigateToReviewPage = () => {
	clickElement("click", "button", selectors.documentPage.reviewAllButton);
	pause("2000");
	refresh();
	pause("2000");
	waitForDisplayed(selectors.reviewPage.commentTextArea, "flase");
	waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "flase");
	waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "flase");
};

export const clickReviewPageLink = () => {
	clickElement("click", "button", selectors.documentPage.reviewAllButton);
	pause("2000");
};

export default navigateToReviewPage;
