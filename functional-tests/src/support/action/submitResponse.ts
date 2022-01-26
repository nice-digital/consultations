import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function submitResponse(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	await clickElement("click", "element", selectors.reviewPage.submitResponseButton);
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "");
	await clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.reviewSubmittedCommentsButton, "");
	await checkContainsText("element", selectors.reviewPage.responseSubmittedHeader, "false", "Response submitted");
};

export async function completeResponseMandatoryQuestions(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await clickElement("click", "element", selectors.reviewPage.answerNoRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	await pause("2000");
};

export async function responseMandatoryQuestions_answerYestoOrg(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerYesRepresentOrg, "");
	await clickElement("click", "element", selectors.reviewPage.answerYesRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.enterOrg, "");
	await setInputField("set", "Fake Org", selectors.reviewPage.enterOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "element", selectors.reviewPage.answerNoTobacLink);
	await pause("2000");
};

export async function clickSubmitResponseButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.submitResponseButton, "");
	await clickElement("click", "element", selectors.reviewPage.submitResponseButton);
};

export async function clickSendYourResponseToYourOrganisationButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.SendYourResponseToYourOrganisationButton, "");
	await clickElement("click", "element", selectors.reviewPage.SendYourResponseToYourOrganisationButton);
};

export async function clickYesSubmitResponseButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "");
	await clickElement("click", "element", selectors.reviewPage.YessubmitResponseButton);
};

export default submitResponse;
