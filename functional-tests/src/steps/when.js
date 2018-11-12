import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";

import clickElement from "../support/action/clickElement";
import enterComment, { enterCommentAndSubmit } from "../support/action/enterComment";

// E.g. When I click on text "Title here" in ".ancestor"
When(
    /^I add the comment "([^"]*)"$/,
	enterComment
);

When(
	/^I add the comment "([^"]*)" and submit$/,
	enterCommentAndSubmit
);
