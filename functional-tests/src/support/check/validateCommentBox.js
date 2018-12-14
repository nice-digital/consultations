import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import selectors from "../selectors";

export const validateCommentBoxText = (commentText) => {
	checkContainsText("element", selectors.documentPage.commentTextArea, commentText);
	browser.pause(1000);
};

export const validateCommentBoxTitle = (titleText) => {
	checkContainsText("element", selectors.documentPage.commentBoxTitle, titleText);
	browser.pause(1000);
};

export const validateCommentSaved = (commentText) => {
	checkContainsText("element", selectors.documentPage.saveIndicator, commentText);
	browser.pause(1000);
};

export default validateCommentBoxText;

