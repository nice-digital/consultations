import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import selectors from "../selectors";

export async function navigateToReviewPage(): Promise<void> {
	await clickElement("click", "button", selectors.documentPage.reviewAllButton);
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.commentTextArea, "");
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await pause("2000");
};

export async function clickReviewPageLink(): Promise<void> {
	await clickElement("click", "button", selectors.documentPage.reviewAllButton);
	await pause("2000");
};

export default navigateToReviewPage;
