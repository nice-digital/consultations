import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import { checkContainsText } from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {isExisting} from "@nice-digital/wdio-cucumber-steps/lib/support/check/isExisting";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import { clickElement } from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import selectors from "../selectors";

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
