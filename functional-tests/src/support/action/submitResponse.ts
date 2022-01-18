import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const submitResponse = () => {
	waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "false");
	clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "false");
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	clickElement("click", "element", selectors.reviewPage.submitResponseButton);
	pause("2000");
	waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "false");
	clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
	pause("2000");
	waitForDisplayed(selectors.reviewPage.reviewSubmittedCommentsButton, "false");
	checkContainsText("element", selectors.reviewPage.responseSubmittedHeader, "false", "Response submitted");
};

export const completeResponseMandatoryQuestions = () => {
	waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "false");
	clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "false");
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	pause("2000");
};

export const responseMandatoryQuestions_answerYestoOrg = () => {
	waitForDisplayed(selectors.reviewPage.answerYesRepresentOrg, "false");
	clickElement("click", "element", selectors.reviewPage.answerYesRepresentOrg);
	waitForDisplayed(selectors.reviewPage.enterOrg, "false");
	setInputField("set", "Fake Org", selectors.reviewPage.enterOrg);
	waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "false");
	clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	pause("2000");
};

export const clickSubmitResponseButton = () => {
	waitForDisplayed(selectors.reviewPage.submitResponseButton, "false");
	clickElement("click", "element", selectors.reviewPage.submitResponseButton);
};

export const clickSendYourResponseToYourOrganisationButton = () => {
	waitForDisplayed(selectors.reviewPage.SendYourResponseToYourOrganisationButton, "false");
	clickElement("click", "element", selectors.reviewPage.SendYourResponseToYourOrganisationButton);
};

export const clickYesSubmitResponseButton = () => {
	waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "false");
	clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
};

export default submitResponse;
