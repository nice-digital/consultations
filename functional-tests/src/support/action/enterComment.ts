import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export async function enterComment(commentText: string): Promise<void> {
	await browser.pause(1000);
	await setInputField("set", commentText, selectors.documentPage.commentTextArea);
	await browser.pause(2000);
};

export const enterGIDToFilter = (gID) => {
	setInputField("set", gID, selectors.adminDownloadPage.filterByGID);
	browser.pause(5000);
};

export async function enterCommentAndSubmit(commentText: string): Promise<void> {
	await browser.pause(2000);
	await enterComment(commentText);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export const enterCommentToFirstInListAndSubmit = (commentText) => {
	setInputField(
		"set",
		commentText,
		selectors.documentPage.firstCommentTextArea
	);
	browser.pause(2000);
	$(selectors.documentPage.submitButton).click();
	browser.pause(2000);
};

export const enterCommentToFirstInList = (commentText) => {
	browser.pause(2000);
	setInputField(
		"set",
		commentText,
		selectors.documentPage.firstCommentTextArea
	);
	browser.pause(2000);
};

export async function enterCommentToFirstInListReviewPage(commentText): Promise<void> {
	await browser.pause(2000);
	await setInputField("add", commentText, selectors.reviewPage.firstCommentTextArea);
	await browser.pause(2000);
};

export default enterComment;
