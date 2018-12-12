import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import selectors from "../selectors";

export const validateCommentBoxText = (commentText) => {
	checkContainsText("element", selectors.documentPage.commentTextArea, commentText);
};

export const validateCommentSaved = (commentText) => {
	checkContainsText("element", selectors.documentPage.saveIndicator, commentText);
};

export default validateCommentBoxText;

