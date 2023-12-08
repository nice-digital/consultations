import clickElement from "../action/clickElement";
import waitForDisplayed from "../action/waitForDisplayed";
import pause from "../action/pause";
import selectors from "../selectors";

export async function navigateToReviewPage(): Promise<void> {
	await clickElement("click", "selector", selectors.documentPage.reviewAllButton);
	await pause("4000");
	await waitForDisplayed(selectors.reviewPage.commentTextArea, "");
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await pause("2000");
};

export async function clickReviewPageLink(): Promise<void> {
	await clickElement("click", "selector", selectors.documentPage.reviewAllButton);
	await pause("2000");
};

export default navigateToReviewPage;
