import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export const enterComment = (commentText) => {
	setInputField("set", commentText, selectors.documentPage.commentTextArea);
};

export const enterCommentAndSubmit = (commentText) => {
	enterComment(commentText);
	browser.click(selectors.documentPage.submitButton);
};

export default enterComment;
