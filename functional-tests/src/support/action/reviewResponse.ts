import clickElement from "../action/clickElement.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import selectors from "../selectors.js";

export async function reviewResponse(): Promise<void> {
await clickElement("click", "selector", selectors.reviewPage.reviewSubmittedCommentsButton);
await waitForDisplayed(selectors.reviewPage.commentTextArea, "");
}
