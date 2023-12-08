import clickElement from "../action/clickElement";
import waitForDisplayed from "../action/waitForDisplayed";
import selectors from "../selectors";

export async function reviewResponse(): Promise<void> {
await clickElement("click", "selector", selectors.reviewPage.reviewSubmittedCommentsButton);
await waitForDisplayed(selectors.reviewPage.commentTextArea, "");
}
