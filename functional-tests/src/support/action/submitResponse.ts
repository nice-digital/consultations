import waitForDisplayed from "../action/waitForDisplayed";
import clickElement from "../action/clickElement";
import checkContainsText from "../check/checkContainsText";
import setInputField from "../action/setInputField";
import pause from "../action/pause";
import selectors from "../selectors";

export async function submitResponse(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await clickElement("click", "selector", selectors.reviewPage.answerNoRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "selector", selectors.reviewPage.answerNoTobacLink);
	await clickElement("click", "selector", selectors.reviewPage.submitResponseButton);
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "");
	await clickElement("click", "selector", selectors.reviewPage.YessubmitResponseButton);
	await pause("2000");
	await waitForDisplayed(selectors.reviewPage.reviewSubmittedCommentsButton, "");
	await checkContainsText("element", selectors.reviewPage.responseSubmittedHeader, null, "Response submitted");
};

export async function completeResponseMandatoryQuestions(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerNoRepresentOrg, "");
	await clickElement("click", "selector", selectors.reviewPage.answerNoRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "selector", selectors.reviewPage.answerNoTobacLink);
	await pause("2000");
};

export async function responseMandatoryQuestions_answerYestoOrg(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.answerYesRepresentOrg, "");
	await clickElement("click", "selector", selectors.reviewPage.answerYesRepresentOrg);
	await waitForDisplayed(selectors.reviewPage.enterOrg, "");
	await setInputField("set", "Fake Org", selectors.reviewPage.enterOrg);
	await waitForDisplayed(selectors.reviewPage.answerNoTobacLink, "");
	await clickElement("click", "selector", selectors.reviewPage.answerNoTobacLink);
	await pause("2000");
};

export async function clickSubmitResponseButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.submitResponseButton, "");
	await clickElement("click", "selector", selectors.reviewPage.submitResponseButton);
};

export async function clickSendYourResponseToYourOrganisationButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.SendYourResponseToYourOrganisationButton, "");
	await clickElement("click", "selector", selectors.reviewPage.SendYourResponseToYourOrganisationButton);
};

export async function clickYesSubmitResponseButton(): Promise<void> {
	await waitForDisplayed(selectors.reviewPage.YessubmitResponseButton, "");
	await clickElement("click", "selector", selectors.reviewPage.YessubmitResponseButton);
};

export default submitResponse;
