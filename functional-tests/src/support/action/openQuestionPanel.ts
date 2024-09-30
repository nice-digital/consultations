import clickElement from "../action/clickElement.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import checkWithinViewport from "../check/checkWithinViewport.js";
import selectors from "../selectors.js";

export async function openQuestionPanel(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.openQuestionPanel, "");
	await clickElement("click", "selector", selectors.documentPage.openQuestionPanel);
	await checkWithinViewport(selectors.documentPage.commentPanel, false);
	await waitForDisplayed(selectors.documentPage.commentTextArea, "");
};

export default openQuestionPanel;
