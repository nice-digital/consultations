import clickElement from "../action/clickElement";
import waitForDisplayed from "../action/waitForDisplayed";
import checkWithinViewport from "../check/checkWithinViewport";
import selectors from "../selectors";

export async function openQuestionPanel(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.openQuestionPanel, "");
	await clickElement("click", "selector", selectors.documentPage.openQuestionPanel);
	await checkWithinViewport(selectors.documentPage.commentPanel, true);
	await waitForDisplayed(selectors.documentPage.commentTextArea, "");
};

export default openQuestionPanel;
