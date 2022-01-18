import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export const reviewResponse = () => {
clickElement("click", "element", selectors.reviewPage.reviewSubmittedCommentsButton);
waitForDisplayed(selectors.reviewPage.commentTextArea, "false");
}
