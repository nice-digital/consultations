import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export const enterQuestionAnswer = (answerText) => {
	setInputField("set", answerText, selectors.documentPage.commentTextArea);
	browser.pause(2000);
};

export const enterQuestionAnswerAndSubmit = (answerText) => {
	browser.pause(2000);
	enterQuestionAnswer(answerText);
	browser.pause(2000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(2000);
};

export const enterQuestionAnswerToFirstInListAndSubmit = (answerText) => {
	setInputField("set", answerText, selectors.documentPage.firstCommentTextArea);
	browser.pause(2000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(2000);
};

export const enterQuestionAnswerToFirstInList = (answerText) => {
	browser.pause(2000);
	setInputField("set", answerText, selectors.documentPage.firstCommentTextArea);
	browser.pause(2000);
};

export const enterQuestionAnswerToSecondInListAndSubmit = (answerText) => {
	setInputField("set", answerText, selectors.documentPage.secondCommentTextArea);
	browser.pause(2000);
	browser.click(selectors.documentPage.submitButton);
	browser.pause(2000);
};

export default enterQuestionAnswer;
