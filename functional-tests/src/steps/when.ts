// import "@nice-digital/wdio-cucumber-steps/lib/when";
import { When } from "@cucumber/cucumber";

// import clickElement from "../support/action/clickElement";
import enterComment, {
	enterCommentAndSubmit,
	enterCommentToFirstInListAndSubmit,
	enterCommentToFirstInList,
	enterCommentToFirstInListReviewPage,
	enterGIDToFilter,
} from "../support/action/enterComment";
import enterQuestionAnswer, {
	enterQuestionAnswerAndSubmit,
	enterQuestionAnswerToFirstInListAndSubmit,
	enterQuestionAnswerToFirstInList,
	enterQuestionAnswerToSecondInListAndSubmit,
} from "../support/action/enterQuestionAnswer";
import navigateToReviewPage, {
	clickReviewPageLink,
} from "../support/action/navigateToReviewPage";
import submitResponse, {
	completeResponseMandatoryQuestions,
	clickSubmitResponseButton,
	responseMandatoryQuestions_answerYestoOrg,
	clickSendYourResponseToYourOrganisationButton,
	clickYesSubmitResponseButton,
} from "../support/action/submitResponse";
import { reviewResponse } from "../support/action/reviewResponse";
import { Login } from "../support/action/Login";
import { LoginAdmin } from "../support/action/LoginAdmin";
import { sidebarLogin } from "../support/action/sidebarLogin";
import { openQuestionPanel } from "../support/action/openQuestionPanel";
import { scrollDeleteButtonIntoView } from "../support/action/scrollDeleteButtonIntoView";
import { selectValueFromDropdown } from "../support/action/selectFromDropdownByIndex";
import { CodeLogin } from "../support/action/CodeLogin";

// E.g. When I click on text "Title here" in ".ancestor"
When(/^I add the comment "([^"]*)"$/, async () => { await enterComment});

When(/^I add the indev GID "([^"]*)" to the filter$/, async () => { await enterGIDToFilter});

When(/^I change the number of results on the page by selecting index "([^"]*)"$/, async () => { await selectValueFromDropdown});

When(/^I add the question answer "([^"]*)"$/, async () => { await enterQuestionAnswer});

When(/^I add the comment "([^"]*)" and submit$/, async () => { await enterCommentAndSubmit});

When(/^I add the question answer "([^"]*)" and submit$/, async () => { await enterQuestionAnswerAndSubmit});

When(/^I add the comment "([^"]*)" to the first in the list and submit$/,	async () => { await enterCommentToFirstInListAndSubmit});

When(/^I add the question answer "([^"]*)" to the first in the list and submit$/,	async () => { await enterQuestionAnswerToFirstInListAndSubmit});

When(/^I add the question answer "([^"]*)" to the second in the list and submit$/,	async () => { await enterQuestionAnswerToSecondInListAndSubmit});

When(/^I add the comment "([^"]*)" to the first in the list$/,	async () => { await enterCommentToFirstInList});

When(/^I add the question answer "([^"]*)" to the first in the list$/,	async () => { await enterQuestionAnswerToFirstInList});

When(/^I add the comment "([^"]*)" to the first in the list on the review page$/,	async () => { await enterCommentToFirstInListReviewPage});

When(/^I navigate to the Review Page$/, async () => { await navigateToReviewPage});

When(/^I click on the Review Page link$/, async () => { await clickReviewPageLink});

When(/^I submit my response$/, async () => { await submitResponse});

When(/^I click submit my response button$/, async () => { await clickSubmitResponseButton});

When(/^I complete the mandatory response submission questions$/,	async () => { await completeResponseMandatoryQuestions});

When(/^I review my response$/, async () => { await reviewResponse});

When(/^I log into accounts with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,
	{ wrapperOptions: { retry: 2 } },
	async (username, password) => { await Login }
);

When(/^I log into the admin page with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,	async () => { await LoginAdmin});

When(/^I log in using sidebar with username "([A-Z0-9_]+)" and password "([A-Z0-9_]+)"$/,	async () => { await sidebarLogin});

When(/^I open question panel$/, async () => { await openQuestionPanel});

When(/^I answer Yes to Organisation question and complete the organisation name$/,	async () => { await responseMandatoryQuestions_answerYestoOrg});

When(/^I scroll the delete button into view$/, async () => { await scrollDeleteButtonIntoView});

When(/^I log into consultation with copied organisation code$/, async () => { await CodeLogin});

When(/^I click send your response to your organisation button$/,	async () => { await clickSendYourResponseToYourOrganisationButton});

When(/^I click yes submit my response button$/, async () => { await clickYesSubmitResponseButton});
