import {checkEqualsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkEqualsText";
import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {isEnabled} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isEnabled";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function validateCommentBoxText(commentText: string): Promise<void> {
	await pause("1000");
	let CommentBoxText = await $(selectors.documentPage.commentTextArea).getValue();
	expect(CommentBoxText).toContain(commentText);
	await pause("1000");
};

export async function validateFirstCommentBox(commentText: string): Promise<void> {
	await pause("1000");
	let firstCommentBoxText = await $(selectors.reviewPage.firstCommentTextArea).getValue();
	expect(firstCommentBoxText).toContain(commentText);
	await pause("1000");
};

export async function validateSecondCommentBox(commentText: string): Promise<void> {
	await pause("1000");
	let secondCommentBoxText = await $(selectors.reviewPage.secondCommentTextArea).getValue();
	expect(secondCommentBoxText).toContain(commentText);
	await pause("1000");
};

export async function validateThirdCommentBox(commentText: string): Promise<void> {
	await pause("1000");
	let thirdCommentBoxText = await $(selectors.reviewPage.thirdCommentTextArea).getValue();
	expect(thirdCommentBoxText).toContain(commentText);
	await pause("1000");
};

export async function validateCommentBoxTitle(titleText: string): Promise<void> {
	await pause("1000");
	await checkContainsText("element", selectors.documentPage.commentBoxTitle, "false", titleText);
	await pause("1000");
};

export async function validateCommentSaved(commentText: string): Promise<void> {
	await checkContainsText("element", selectors.documentPage.saveIndicator, "false", commentText);
	await pause("1000");
};

export async function validateCommentBoxInactive(): Promise<void> {
	await isEnabled(selectors.reviewPage.firstCommentTextArea, "true");
	await pause("1000");
};

export async function validateAllCommentBoxesInactive(): Promise<void> {
	await isEnabled(selectors.reviewPage.firstCommentTextArea, "true");
	await pause("1000");
	await isEnabled(selectors.reviewPage.secondCommentTextArea, "true");
	await pause("1000");
	await isEnabled(selectors.reviewPage.thirdCommentTextArea, "true");
	await pause("1000");
}

export default validateCommentBoxText;

