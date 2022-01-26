import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {isEnabled} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateCommentBoxText = (commentText) => {
	checkContainsText("element", selectors.documentPage.commentTextArea, "false", commentText);
	pause("1000");
};

export const validateFirstCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.firstCommentTextArea, "false", commentText);
	pause("1000");
};

export const validateSecondCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.secondCommentTextArea, "false", commentText);
	pause("1000");
};

export const validateThirdCommentBox = (commentText) => {
	checkContainsText("element", selectors.reviewPage.thirdCommentTextArea, "false", commentText);
	pause("1000");
};

export async function validateCommentBoxTitle(titleText: string): Promise<void> {
	await pause("1000");
	await checkContainsText("element", selectors.documentPage.commentBoxTitle, "false", titleText);
	await pause("1000");
};

export const validateCommentSaved = (commentText) => {
	checkContainsText("element", selectors.documentPage.saveIndicator, "false", commentText);
	pause("1000");
};

export const validateCommentBoxInactive = () => {
	isEnabled(selectors.reviewPage.firstCommentTextArea, "true");
	pause("1000");
};

export const validateAllCommentBoxesInactive = () => {
	isEnabled(selectors.reviewPage.firstCommentTextArea, "true");
	pause("1000");
	isEnabled(selectors.reviewPage.secondCommentTextArea, "true");
	pause("1000");
	isEnabled(selectors.reviewPage.thirdCommentTextArea, "true");
	pause("1000");
}

export default validateCommentBoxText;

