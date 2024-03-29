import waitForDisplayed from "../action/waitForDisplayed.js";
import checkContainsText from "../check/checkContainsText.js";
import pause from "../action/pause.js";
import clickElement from "../action/clickElement.js";
import selectors from "../selectors.js";

export async function validateConsultationIsClosed(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await checkContainsText(
		"element",
		selectors.documentPage.ConsultationStatusTag,
		"",
		"Closed for comments"
	);
	await pause("1000");
};

export async function validateConsultationClosedMessage(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await checkContainsText(
		"element",
		selectors.documentPage.closeConsultationMessage,
		"",
		"The content on this page is not current guidance and is only for the purposes of the consultation process."
	);
	await pause("1000");
};

export async function validateCommentOnDocIsDisabled(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await $(selectors.documentPage.commentOnDocButton).waitForExist({reverse: true});
	await pause("1000");
}

export async function validateCommentOnChapterIsDisabled(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await $(".chapter > .title [data-qa-sel='in-text-comment-button']").waitForExist({reverse: true})
	await pause("1000");
}

export async function validateCommentOnSectionIsDisabled(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await pause("1000")
	await clickElement("click", "selector", "[data-qa-sel='nav-list-item']:nth-of-type(2)");
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await $(".section:first-of-type > .title [data-qa-sel='in-text-comment-button']").waitForExist({reverse: true});
	await pause("1000");
}

export async function validateCommentOnSubSectionIsDisabled(): Promise<void> {
	await waitForDisplayed(selectors.documentPage.documentContainer, "");
	await $("[data-gtm-label='Comment on subsection']").waitForExist({reverse: true});
	await pause("1000");
}
export default validateConsultationIsClosed;
