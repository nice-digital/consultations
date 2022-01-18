// import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "@cucumber/cucumber";

import validateCommentBox, {
	validateCommentBoxText,
	validateCommentBoxTitle,
	validateCommentSaved,
	validateFirstCommentBox,
	validateSecondCommentBox,
	validateThirdCommentBox,
	validateCommentBoxInactive,
	validateAllCommentBoxesInactive,
} from "../support/check/validateCommentBox";
import deleteOneComment from "../support/action/deleteOneComment";
import validateSubmitResponseButtonInactive, {
	validateSubmitResponseValidationMessage,
} from "../support/check/validateSubmitResponseButton";
import validateDownloadPageResultCount, {
	validateDownloadPageAllResults,
	validateFirstLinkInPagination,
} from "../support/check/validateAdminDownloadPage";
import clickCancelFilter from "../support/action/clickCancelFilter";
import clickPaginationOption, {
	clickSecondPaginationOption,
	clickNextPagination,
	clickPreviousPagination,
} from "../support/action/clickPaginationOption";
import validateResponsesFilterChecked from "../support/check/validateResponsesFilterChecked";
import { enterEmailaddress } from "../support/action/enterEmailaddress";
import { generateOrganisationCode } from "../support/action/generateOrganisationCode";

import validateAlertCautionText from "../support/check/validateAlertCautionText";

Then(/^I expect the comment box contains "([^"]*)"$/, async () => { await validateCommentBox});

Then(/^I expect the first comment box contains "([^"]*)"$/,	async () => { await validateFirstCommentBox});

Then(/^I expect the second comment box contains "([^"]*)"$/,	async () => { await validateSecondCommentBox});

Then(/^I expect the third comment box contains "([^"]*)"$/,	async () => { await validateThirdCommentBox});

Then(/^I expect the comment box title contains "([^"]*)"$/,	async () => { await validateCommentBoxTitle});

Then(/^I expect the comment save button displays "([^"]*)"$/,	async () => { await validateCommentSaved});

Then(/^I expect the comment box is inactive$/, async () => { await validateCommentBoxInactive});

Then(/^I expect all comment boxes are inactive$/,	async () => { await validateAllCommentBoxesInactive});

Then(/^I expect the Submit Response button is inactive$/,	async () => { await validateSubmitResponseButtonInactive});

Then(/^I expect the feedback message "([^"]*)" to be displayed$/,	async () => { await validateSubmitResponseValidationMessage});

Then(/^I click delete comment$/, async () => { await deleteOneComment});

Then(/^I click on the cancel filter$/, async () => { await clickCancelFilter});

Then(/^I click the second pagination option$/, async () => { await clickSecondPaginationOption});

Then(/^I click the previous pagination option$/, async () => { await clickPreviousPagination});

Then(/^I click the next pagination option$/, async () => { await clickNextPagination});

Then(/^I expect the result list count contains "([^"]*)"$/,	async () => { await validateDownloadPageResultCount});

Then(/^I expect the first pagination option is "([^"]*)"$/,	async () => { await validateFirstLinkInPagination});

Then(/^I expect all results are displayed$/, async () => { await validateDownloadPageAllResults});

Then(/^I expect the your responses filter to be selected by default$/,	async () => { await validateResponsesFilterChecked});

Then(/^I enter code user emailaddress "([A-Z0-9_]+)"$/, async () => { await enterEmailaddress});

Then(/^I click on consultation to generate and copy the organisation code$/,	async () => { await generateOrganisationCode});

Then(/^I expect the alert caution to contain "([^"]*)"$/,	async () => { await validateAlertCautionText});
