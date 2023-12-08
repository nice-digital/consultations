import setInputField from "../action/setInputField";
import selectors from "../selectors";

export async function enterComment(commentText: string): Promise<void> {
	await browser.pause(1000);
	await setInputField("set", commentText, selectors.documentPage.commentTextArea);
	await browser.pause(2000);
};

export async function enterGIDToFilter(gID: string): Promise<void> {
	await setInputField("set", gID, selectors.adminDownloadPage.filterByGID);
	await browser.pause(5000);
};

export async function enterCommentAndSubmit(commentText: string): Promise<void> {
	await browser.pause(2000);
	await enterComment(commentText);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export async function enterCommentToFirstInListAndSubmit(commentText: string): Promise<void> {
	await setInputField(
		"set",
		commentText,
		selectors.documentPage.firstCommentTextArea
	);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export async function enterCommentToFirstInList(commentText: string): Promise<void> {
	await browser.pause(2000);
	setInputField(
		"set",
		commentText,
		selectors.documentPage.firstCommentTextArea
	);
	await browser.pause(2000);
};

export async function enterCommentToFirstInListReviewPage(commentText: string): Promise<void> {
	await browser.pause(2000);
	await setInputField("add", commentText, selectors.reviewPage.firstCommentTextArea);
	await browser.pause(2000);
};

export default enterComment;
