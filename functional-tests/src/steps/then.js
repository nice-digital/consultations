import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";

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
// import validateCommentSaved from "../support/check/validateCommentSaved";

/*Then(
    /^I expect that (button|element) "([^"]*)?"( not)* matches the text "([^"]*)?"$/,
    checkContainsText
);*/
Then(/^I expect the comment box contains "([^"]*)"$/, validateCommentBox);

Then(
	/^I expect the first comment box contains "([^"]*)"$/,
	validateFirstCommentBox
);

Then(
	/^I expect the second comment box contains "([^"]*)"$/,
	validateSecondCommentBox
);

Then(
	/^I expect the third comment box contains "([^"]*)"$/,
	validateThirdCommentBox
);

Then(
	/^I expect the comment box title contains "([^"]*)"$/,
	validateCommentBoxTitle
);

Then(
	/^I expect the comment save button displays "([^"]*)"$/,
	validateCommentSaved
);

Then(/^I expect the comment box is inactive$/, validateCommentBoxInactive);

Then(
	/^I expect all comment boxes are inactive$/,
	validateAllCommentBoxesInactive
);

Then(
	/^I expect the Submit Response button is inactive$/,
	validateSubmitResponseButtonInactive
);

Then(
	/^I expect the feedback message "([^"]*)" to be displayed$/,
	validateSubmitResponseValidationMessage
);

Then(/^I click delete comment$/, deleteOneComment);

Then(/^I click on the cancel filter$/, clickCancelFilter);

Then(/^I click the second pagination option$/, clickSecondPaginationOption);

Then(/^I click the previous pagination option$/, clickPreviousPagination);

Then(/^I click the next pagination option$/, clickNextPagination);

Then(
	/^I expect the result list count contains "([^"]*)"$/,
	validateDownloadPageResultCount
);

Then(
	/^I expect the first pagination option is "([^"]*)"$/,
	validateFirstLinkInPagination
);

Then(/^I expect all results are displayed$/, validateDownloadPageAllResults);

Then(
	/^I expect the your responses filter to be selected by default$/,
	validateResponsesFilterChecked
);
