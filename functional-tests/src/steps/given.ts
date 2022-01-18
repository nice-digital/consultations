// import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from "@cucumber/cucumber";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {deleteComments} from "../support/action/deleteComments";
import { deleteSubmissionUser } from "../support/action/deleteSubmissionUser";
import { addQuestionsToConsultation } from "../support/action/addQuestionsToConsultation";
import { validateStatusFilterChecked } from "../support/check/validateStatusFilterChecked";
import { clickElement } from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import { pause } from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";

Given(/^I delete all comments on the page$/, async () => { await deleteComments});

Given(/^I comment on a Document$/, async () => {
	await pause("1000");
	await waitForDisplayed("[data-qa-sel='comment-on-consultation-document']", "false");
	await clickElement("click", "selector", "[data-qa-sel='comment-on-consultation-document']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "false");
});

Given(/^I comment on a Document again$/, async () => {
	await pause("1000");
	await waitForDisplayed("[data-qa-sel='comment-on-consultation-document']", "false");
	await clickElement("click", "selector", "[data-qa-sel='comment-on-consultation-document']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "false");
});

Given(/^I comment on a Chapter$/, async () => {
	await pause("1000");
	await waitForDisplayed(".document-comment-container", "false");
	await pause("1000");
	await clickElement("click", "selector", ".chapter > .title [data-qa-sel='in-text-comment-button']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "false");
});

Given(/^I comment on a Section$/, async () => {
	await pause("1000");
	await clickElement("click", "selector", "[data-qa-sel='nav-list-item']:nth-of-type(2)");
	await waitForDisplayed(".document-comment-container", "false");
	await pause("1000");
	await waitForDisplayed(
		".section:first-of-type > .title [data-qa-sel='in-text-comment-button']", "false"
	);
	await pause("1000");
	await clickElement("click", "selector", ".section:first-of-type > .title [data-qa-sel='in-text-comment-button']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "false");
});

Given(/^I comment on a Sub-section$/, async () => {
	await pause("1000");
	await waitForDisplayed(
		"[data-qa-sel='in-text-comment-button']:nth-of-type(2)", "false"
	);
	await clickElement("click", "selector", "[data-qa-sel='in-text-comment-button']:nth-of-type(2)");
	await waitForDisplayed(".document-comment-container", "false");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "false");
	await pause("1000");
});

Given(/^I delete submissions for userid "([^"]*)?"$/, async () => { await deleteSubmissionUser});

Given(
	/^I add questions to Consultation "([^"]*)?"$/, async () => { await addQuestionsToConsultation});

Given(/^I select open and closed status filter$/, async () => { await  validateStatusFilterChecked});
