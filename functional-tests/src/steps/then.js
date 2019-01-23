import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";

import validateCommentBox, { validateCommentBoxText, validateCommentBoxTitle, validateCommentSaved, validateFirstCommentBox, validateSecondCommentBox, validateThirdCommentBox, validateCommentBoxInactive }  from '../support/check/validateCommentBox';
import deleteOneComment from '../support/action/deleteOneComment';
import validateSubmitResponseButtonInactive, { validateSubmitResponseValidationMessage } from '../support/check/validateSubmitResponseButton';
// import validateCommentSaved from "../support/check/validateCommentSaved";

/*Then(
    /^I expect that (button|element) "([^"]*)?"( not)* matches the text "([^"]*)?"$/,
    checkContainsText
);*/
Then(
	/^I expect the comment box contains "([^"]*)"$/,
	validateCommentBox
);

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
)

Then(
	/^I expect the comment save button displays "([^"]*)"$/,
	validateCommentSaved
);

Then(
	/^I expect the comment box is inactive$/,
	validateCommentBoxInactive
);

Then(
	/^I expect the Submit Response button is inactive$/,
	validateSubmitResponseButtonInactive
);

Then(
	/^I expect the feedback message "([^"]*)" to be displayed$/,
	validateSubmitResponseValidationMessage
);

Then(
	/^I click delete comment$/,
	deleteOneComment
);
