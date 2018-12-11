import "@nice-digital/wdio-cucumber-steps/lib/then";
import { Then } from "cucumber";

import validateCommentBox, { validateCommentBoxText } from '../support/check/validateCommentBox';
import validateCommentSaved from "../support/check/validateCommentSaved";

/*Then(
    /^I expect that (button|element) "([^"]*)?"( not)* matches the text "([^"]*)?"$/,
    checkContainsText
);*/
Then(
	/^I expect the comment box contains "([^"]*)"$/,
	validateCommentBox
);

Then(
	/^I expect the comment save button displays "([^"]*)"$/,
	validateCommentSaved
);
