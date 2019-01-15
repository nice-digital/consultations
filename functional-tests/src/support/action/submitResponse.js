import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import selectors from "../selectors";

export const submitResponse = () => {
	waitForVisible(selectors.reviewPage.answerNoRepresentOrg);
	clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	waitForVisible(selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.submitResponseButton);
	browser.pause(2000);
	waitForVisible(selectors.reviewPage.reviewSubmittedCommentsButton);
	checkContainsText("element", selectors.reviewPage.responseSubmittedHeader, "Response submitted");
};

export const completeResponseMandatoryQuestions = () => {
	waitForVisible(selectors.reviewPage.answerNoRepresentOrg);
	clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	waitForVisible(selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	browser.pause(2000);
};

export default submitResponse;
