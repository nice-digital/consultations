import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";

import validateCommentBox, { validateCommentBoxText, validateCommentBoxTitle, validateCommentSaved, validateFirstCommentBox, validateSecondCommentBox, validateThirdCommentBox }  from '../support/check/validateCommentBox';
import deleteOneComment from '../support/action/deleteOneComment';
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
	/^I click delete comment$/,
	deleteOneComment
);
