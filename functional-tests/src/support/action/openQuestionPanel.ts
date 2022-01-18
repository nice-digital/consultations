import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {checkWithinViewport} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkWithinViewport";
import selectors from "../selectors";

export const openQuestionPanel = () => {
	waitForDisplayed(selectors.documentPage.openQuestionPanel, "false");
	clickElement("click", "element", selectors.documentPage.openQuestionPanel);
	checkWithinViewport(selectors.documentPage.commentPanel, "false");
	waitForDisplayed(selectors.documentPage.commentTextArea, "false");
};

export default openQuestionPanel;
