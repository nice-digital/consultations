import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "cucumber";

// import clickElement from "../support/action/clickElement";
import enterComment, { enterCommentAndSubmit, enterCommentToFirstInListAndSubmit, enterCommentToFirstInList, enterCommentToFirstInListReviewPage } from "../support/action/enterComment";
import enterQuestionAnswer, { enterQuestionAnswerAndSubmit, enterQuestionAnswerToFirstInListAndSubmit, enterQuestionAnswerToFirstInList, enterQuestionAnswerToSecondInListAndSubmit } from "../support/action/enterQuestionAnswer";
import navigateToReviewPage, { clickReviewPageLink } from "../support/action/navigateToReviewPage";
import submitResponse, { completeResponseMandatoryQuestions, clickSubmitResponseButton, responseMandatoryQuestions_answerYestoOrg } from "../support/action/submitResponse";
import { reviewResponse } from "../support/action/reviewResponse";
import { Login } from "../support/action/globalnavLogin";
import { openQuestionPanel } from "../support/action/openQuestionPanel";

// E.g. When I click on text "Title here" in ".ancestor"
When(
    /^I add the comment "([^"]*)"$/,
	enterComment
);

When(
	/^I add the question answer "([^"]*)"$/,
	enterQuestionAnswer
);

When(
	/^I add the comment "([^"]*)" and submit$/,
	enterCommentAndSubmit
);

When(
	/^I add the question answer "([^"]*)" and submit$/,
	enterQuestionAnswerAndSubmit
);

When(
	/^I add the comment "([^"]*)" to the first in the list and submit$/,
	enterCommentToFirstInListAndSubmit
);

When(
	/^I add the question answer "([^"]*)" to the first in the list and submit$/,
	enterQuestionAnswerToFirstInListAndSubmit
);

When(
	/^I add the question answer "([^"]*)" to the second in the list and submit$/,
	enterQuestionAnswerToSecondInListAndSubmit
);

When(
	/^I add the comment "([^"]*)" to the first in the list$/,
	enterCommentToFirstInList
);

When(
	/^I add the question answer "([^"]*)" to the first in the list$/,
	enterQuestionAnswerToFirstInList
);

When(
	/^I add the comment "([^"]*)" to the first in the list on the review page$/,
	enterCommentToFirstInListReviewPage
);

When(
	/^I navigate to the Review Page$/,
	navigateToReviewPage
);

When(
	/^I click on the Review Page link$/,
	clickReviewPageLink
);

When(
	/^I submit my response$/,
	submitResponse
);

When(
	/^I click submit my response button$/,
	clickSubmitResponseButton
);

When(
	/^I complete the mandatory response submission questions$/,
	completeResponseMandatoryQuestions
);

When(
	/^I review my response$/,
	reviewResponse
);

When(
	/^I log into accounts with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,
	Login
);

When(
	/^I open question panel$/,
	openQuestionPanel
);

When(
	/^I answer Yes to Organisation question and complete the organisation name$/,
	responseMandatoryQuestions_answerYestoOrg
);
