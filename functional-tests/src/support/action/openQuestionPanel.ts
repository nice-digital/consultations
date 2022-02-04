import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {checkWithinViewport} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkWithinViewport";
import selectors from "../selectors";

export async function openQuestionPanel(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.openQuestionPanel, "");
	await clickElement("click", "element", selectors.documentPage.openQuestionPanel);
	await checkWithinViewport(selectors.documentPage.commentPanel, "");
	await waitForDisplayed(selectors.documentPage.commentTextArea, "");
};

export default openQuestionPanel;
