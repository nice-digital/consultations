import setInputField from "../action/setInputField";
import selectors from "../selectors";

export async function enterQuestionAnswer(answerText: string): Promise<void> {
	await setInputField("set", answerText, selectors.documentPage.commentTextArea);
	await browser.pause(2000);
};

export async function enterQuestionAnswerAndSubmit(answerText: string): Promise<void> {
	await browser.pause(2000);
	await enterQuestionAnswer(answerText);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export async function enterQuestionAnswerToFirstInListAndSubmit(answerText: string): Promise<void> {
	await setInputField("set", answerText, selectors.documentPage.firstCommentTextArea);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export async function enterQuestionAnswerToFirstInList(answerText: string): Promise<void> {
	await browser.pause(2000);
	await setInputField("set", answerText, selectors.documentPage.firstCommentTextArea);
	await browser.pause(2000);
};

export async function enterQuestionAnswerToSecondInListAndSubmit(answerText: string): Promise<void> {
	await setInputField("set", answerText, selectors.documentPage.secondCommentTextArea);
	await browser.pause(2000);
	await $(selectors.documentPage.submitButton).click();
	await browser.pause(2000);
};

export default enterQuestionAnswer;
