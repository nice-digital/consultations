import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import selectors from "../selectors";

export const reviewResponse = () => {
clickElement("click", "element", selectors.reviewPage.reviewSubmittedCommentsButton);
waitForVisible(selectors.reviewPage.commentTextArea);
}
