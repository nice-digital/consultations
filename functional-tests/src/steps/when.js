import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";

// import clickElement from "../support/action/clickElement";
import enterComment, { enterCommentAndSubmit, enterCommentToFirstInListAndSubmit } from "../support/action/enterComment";
import navigateToReviewPage from "../support/action/navigateToReviewPage";

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
