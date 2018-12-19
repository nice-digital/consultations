import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export const enterComment = (commentText) => {
	setInputField("set", commentText, selectors.documentPage.commentTextArea);
	browser.pause(1000);
};

export const enterCommentAndSubmit = (commentText) => {
	browser.pause(1000);
	enterComment(commentText);
	browser.pause(1000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(1000);
};

export const enterCommentToFirstInListAndSubmit = (commentText) => {
	setInputField("set", commentText, selectors.documentPage.firstCommentTextArea);
	browser.pause(1000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(1000);
}

export default enterComment;
