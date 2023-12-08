// import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from "@cucumber/cucumber";
import openWebsite from "../support/action/openWebsite";
import waitForDisplayed from "../support/action/waitForDisplayed";
import { deleteCommentsOnReviewPage } from "../support/action/deleteComments";
import { deleteSubmissionUser } from "../support/action/deleteSubmissionUser";
import { addQuestionsToConsultation } from "../support/action/addQuestionsToConsultation";
import { validateStatusFilterChecked } from "../support/check/validateStatusFilterChecked";
import clickElement from "../support/action/clickElement";
import pause from "../support/action/pause";
import { clickLeadInfoLink } from "../support/action/clickLeadInfoLink";
import { validateConsultationIsClosed } from "../support/check/validateConsultationIsCLosed";
import { validateConsultationClosedMessage } from "../support/check/validateConsultationIsCLosed";

Given(
    /^I open the (url|site) "([^"]*)?"$/,
    openWebsite
);

Given(/^I delete all comments on the page$/, deleteCommentsOnReviewPage);

Given(/^I comment on a Document$/, async () => {
	await pause("1000");
	await waitForDisplayed("[data-qa-sel='comment-on-consultation-document']", "");
	await pause("1000");
	await clickElement("click", "selector", "[data-qa-sel='comment-on-consultation-document']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "");
	await pause("1000");
});

Given(/^I comment on a Document again$/, async () => {
	await pause("1000");
	await waitForDisplayed("[data-qa-sel='comment-on-consultation-document']", "");
	await pause("1000");
	await clickElement("click", "selector", "[data-qa-sel='comment-on-consultation-document']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "");
	await pause("1000");
});

Given(/^I comment on a Chapter$/, async () => {
	await pause("1000");
	await waitForDisplayed(".document-comment-container", "");
	await pause("1000");
	await clickElement("click", "selector", ".chapter > .title [data-qa-sel='in-text-comment-button']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "");
});

Given(/^I comment on a Section$/, async () => {
	await pause("1000");
	//await clickElement("click", "selector", "[data-qa-sel='nav-list-item']:nth-of-type(2)");
	await waitForDisplayed(".document-comment-container", "");
	await pause("1000");
	await waitForDisplayed(
		".section:first-of-type > .title [data-qa-sel='in-text-comment-button']", ""
	);
	await pause("1000");
	await clickElement("click", "selector", ".section:first-of-type > .title [data-qa-sel='in-text-comment-button']");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "");
});

Given(/^I comment on a Sub-section$/, async () => {
	await pause("1000");
	await waitForDisplayed(
		"[data-qa-sel='in-text-comment-button']:nth-of-type(2)", ""
	);
	await clickElement("click", "selector", "[data-qa-sel='in-text-comment-button']:nth-of-type(2)");
	await waitForDisplayed(".document-comment-container", "");
	await pause("1000");
	await waitForDisplayed("body [data-qa-sel='comment-box-title']", "");
	await pause("1000");
});

Given(/^I delete submissions for userid "([A-Z0-9_]+)" and navigate to review page "([^"]*)?"$/, deleteSubmissionUser);

Given(
	/^I add questions to Consultation "([^"]*)?"$/, addQuestionsToConsultation);

Given(/^I select open and closed status filter$/, validateStatusFilterChecked);

Given(/^I click on the request commenting lead permision link$/,  clickLeadInfoLink);

Given(/^I appear on a closed consultation$/, validateConsultationIsClosed);

Given(/^I expect the not current guidance banner to appear$/, validateConsultationClosedMessage);
Given(/^I click on the request commenting lead permision link$/, clickLeadInfoLink);
