// import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "@wdio/cucumber-framework";

// import clickElement from "../support/action/clickElement";
import enterComment, {
	enterCommentAndSubmit,
	enterCommentToFirstInListAndSubmit,
	enterCommentToFirstInList,
	enterCommentToFirstInListReviewPage,
	enterGIDToFilter,
} from "../support/action/enterComment.js";
import enterQuestionAnswer, {
	enterQuestionAnswerAndSubmit,
	enterQuestionAnswerToFirstInListAndSubmit,
	enterQuestionAnswerToFirstInList,
	enterQuestionAnswerToSecondInListAndSubmit,
} from "../support/action/enterQuestionAnswer.js";
import navigateToReviewPage, {
	clickReviewPageLink,
} from "../support/action/navigateToReviewPage.js";
import submitResponse, {
	completeResponseMandatoryQuestions,
	clickSubmitResponseButton,
	responseMandatoryQuestions_answerYestoOrg,
	clickSendYourResponseToYourOrganisationButton,
	clickYesSubmitResponseButton,
} from "../support/action/submitResponse.js";
import { reviewResponse } from "../support/action/reviewResponse.js";
import { Login } from "../support/action/Login.js";
import { LoginAdmin } from "../support/action/LoginAdmin.js";
import { sidebarLogin } from "../support/action/sidebarLogin.js";
import { openQuestionPanel } from "../support/action/openQuestionPanel.js";
import { scrollDeleteButtonIntoView } from "../support/action/scrollDeleteButtonIntoView.js";
import { selectValueFromDropdown } from "../support/action/selectFromDropdownByIndex.js";
import { CodeLogin } from "../support/action/CodeLogin.js";
import { selectMyConsultationFilter } from "../support/action/selectMyConsultationFilter.js";
import handleModal from "../support/action/handleModal.js";
import { logout } from "../support/action/logout.js";


// E.g. When I click on text "Title here" in ".ancestor"
When(/^I add the comment "([^"]*)"$/, enterComment);

When(/^I add the indev GID "([^"]*)" to the filter$/, enterGIDToFilter);

When(/^I change the number of results on the page by selecting index "([^"]*)"$/, selectValueFromDropdown);

When(/^I add the question answer "([^"]*)"$/, enterQuestionAnswer);

When(/^I add the comment "([^"]*)" and submit$/, enterCommentAndSubmit);

When(/^I add the question answer "([^"]*)" and submit$/, enterQuestionAnswerAndSubmit);

When(/^I add the comment "([^"]*)" to the first in the list and submit$/,	enterCommentToFirstInListAndSubmit);

When(/^I add the question answer "([^"]*)" to the first in the list and submit$/,	enterQuestionAnswerToFirstInListAndSubmit);

When(/^I add the question answer "([^"]*)" to the second in the list and submit$/,	enterQuestionAnswerToSecondInListAndSubmit);

When(/^I add the comment "([^"]*)" to the first in the list$/,	enterCommentToFirstInList);

When(/^I add the question answer "([^"]*)" to the first in the list$/,	enterQuestionAnswerToFirstInList);

When(/^I add the comment "([^"]*)" to the first in the list on the review page$/,	enterCommentToFirstInListReviewPage);

When(/^I navigate to the Review Page$/, navigateToReviewPage);

When(/^I click on the Review Page link$/, clickReviewPageLink);

When(/^I submit my response$/, submitResponse);

When(/^I click submit my response button$/, clickSubmitResponseButton);

When(/^I complete the mandatory response submission questions$/, completeResponseMandatoryQuestions);

When(/^I review my response$/, reviewResponse);

When(/^I log into accounts with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/, { wrapperOptions: { retry: 2 } }, Login);

When(/^I log into the admin page with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,	LoginAdmin);

When(/^I log in using sidebar with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,	sidebarLogin);

When (/^I logout$/, logout);

When(/^I open question panel$/, openQuestionPanel);

When(/^I answer Yes to Organisation question and complete the organisation name$/,	responseMandatoryQuestions_answerYestoOrg);

When(/^I scroll the delete button into view$/, scrollDeleteButtonIntoView);

When(/^I log into consultation with copied organisation code$/, CodeLogin);

When(/^I click send your response to your organisation button$/,	clickSendYourResponseToYourOrganisationButton);

When(/^I click yes submit my response button$/, clickYesSubmitResponseButton);

When(/^I select my consultations filter$/, selectMyConsultationFilter);

When(/^I (accept|dismiss) the (alertbox|confirmbox|prompt)$/, handleModal);

