import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export const enterComment = (commentText) => {
	setInputField("set", commentText, selectors.documentPage.commentTextArea);
	browser.pause(2000);
};

export const enterCommentAndSubmit = (commentText) => {
	browser.pause(2000);
	enterComment(commentText);
	browser.pause(2000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(2000);
};

export const enterCommentToFirstInListAndSubmit = (commentText) => {
	setInputField("set", commentText, selectors.documentPage.firstCommentTextArea);
	browser.pause(2000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(2000);
};

export const enterCommentToFirstInList = (commentText) => {
	browser.pause(2000);
	setInputField("set", commentText, selectors.documentPage.firstCommentTextArea);
	browser.pause(2000);
};

export const enterCommentToFirstInListReviewPage = (commentText) => {
	browser.pause(2000);
	setInputField("set", commentText, selectors.reviewPage.firstCommentTextArea);
	browser.pause(2000);
};

export default enterComment;
