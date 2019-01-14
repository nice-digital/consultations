import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";

// import clickElement from "../support/action/clickElement";
import enterComment, { enterCommentAndSubmit, enterCommentToFirstInListAndSubmit } from "../support/action/enterComment";
import navigateToReviewPage from "../support/action/navigateToReviewPage";
import { submitResponse } from "../support/action/submitResponse";
import { reviewResponse } from "../support/action/reviewResponse";
import { Login } from "../support/action/tophatLogin";

// E.g. When I click on text "Title here" in ".ancestor"
When(
    /^I add the comment "([^"]*)"$/,
	enterComment
);

When(
	/^I add the comment "([^"]*)" and submit$/,
	enterCommentAndSubmit
);

When(
	/^I add the comment "([^"]*)" to the first in the list and submit$/,
	enterCommentToFirstInListAndSubmit
)

When(
	/^I navigate to the Review Page$/,
	navigateToReviewPage
);

When(
	/^I submit my response$/,
	submitResponse
);

When(
	/^I review my response$/,
	reviewResponse
);

When(
	/^I log into accounts with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,
	Login
)
