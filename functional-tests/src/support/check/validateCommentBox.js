import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import isEnabled from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateCommentBoxText = (commentText) => {
	checkContainsText("element", selectors.documentPage.commentTextArea, commentText);
	pause(1000);
};

export const validateFirstCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.firstCommentTextArea, commentText);
	pause(1000);
};

export const validateSecondCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.secondCommentTextArea, commentText);
	pause(1000);
};

export const validateThirdCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.thirdCommentTextArea, commentText);
	pause(1000);
};

export const validateCommentBoxTitle = (titleText) => {
	checkContainsText("element", selectors.documentPage.commentBoxTitle, titleText);
	pause(1000);
};

export const validateCommentSaved = (commentText) => {
	checkContainsText("element", selectors.documentPage.saveIndicator, commentText);
	pause(1000);
};

export const validateCommentBoxInactive = () => {
	isEnabled(selectors.reviewPage.firstCommentTextArea, 1);
	pause(1000);
};

export const validateAllCommentBoxesInactive = () => {
	isEnabled(selectors.reviewPage.firstCommentTextArea, 1);
	pause(1000);
	isEnabled(selectors.reviewPage.secondCommentTextArea, 1);
	pause(1000);
	isEnabled(selectors.reviewPage.thirdCommentTextArea, 1);
	pause(1000);
}

export default validateCommentBoxText;

