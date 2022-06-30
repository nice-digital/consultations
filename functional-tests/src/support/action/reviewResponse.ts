import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export async function reviewResponse(): Promise<void> {
await clickElement("click", "element", selectors.reviewPage.reviewSubmittedCommentsButton);
await waitForDisplayed(selectors.reviewPage.commentTextArea, "");
}
