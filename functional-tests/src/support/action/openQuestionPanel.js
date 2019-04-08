import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import checkWithinViewport from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkWithinViewport";
import selectors from "../selectors";

export const openQuestionPanel = () => {
	waitForVisible(selectors.documentPage.openQuestionPanel);
	clickElement("click", "element", selectors.documentPage.openQuestionPanel);
	checkWithinViewport(selectors.documentPage.commentPanel);
	waitForVisible(selectors.documentPage.commentTextArea);
};

export default openQuestionPanel;
