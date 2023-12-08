import openWebsite from "../action/openWebsite";
import waitFor from "../action/waitFor";
import selectors from "../selectors";

export async function addQuestionsToConsultation(consultationId: string): Promise<void> {
	await openWebsite(
		"url",
		"admin/InsertQuestionsForDocument1And2InConsultation?consultationId=" +
			consultationId
	);
	await openWebsite("url", "" + consultationId);
	await browser.pause(2000);
	// await waitFor("body pre:nth-child(1)", "3000", "", "exist");
	// await browser.pause(2000);
};

export default addQuestionsToConsultation;
