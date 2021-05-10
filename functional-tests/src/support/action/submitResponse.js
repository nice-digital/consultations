import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import selectors from "../selectors";

export const submitResponse = () => {
	waitForVisible(selectors.reviewPage.answerNoRepresentOrg);
	clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	waitForVisible(selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.submitResponseButton);
	browser.pause(2000);
	waitForVisible(selectors.reviewPage.YessubmitResponseButton);
	clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
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

export const responseMandatoryQuestions_answerYestoOrg = () => {
	waitForVisible(selectors.reviewPage.answerYesRepresentOrg);
	clickElement("click", "element", selectors.reviewPage.answerYesRepresentOrg);
	waitForVisible(selectors.reviewPage.enterOrg);
	setInputField("set", "Fake Org", selectors.reviewPage.enterOrg);
	waitForVisible(selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	browser.pause(2000);
};

export const clickSubmitResponseButton = () => {
	waitForVisible(selectors.reviewPage.submitResponseButton);
	clickElement("click", "element", selectors.reviewPage.submitResponseButton);
};

export const clickSendYourResponseToYourOrganisationButton = () => {
	waitForVisible(selectors.reviewPage.SendYourResponseToYourOrganisationButton);
	clickElement("click", "element", selectors.reviewPage.SendYourResponseToYourOrganisationButton);
export const clickYesSubmitResponseButton = () => {
	waitForVisible(selectors.reviewPage.YessubmitResponseButton);
	clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
};

export default submitResponse;
