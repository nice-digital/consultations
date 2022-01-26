import {openWebsite} from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export async function addQuestionsToConsultation(consultationId: string): Promise<void> {
	await openWebsite(
		"url",
		"admin/InsertQuestionsForDocument1And2InConsultation?consultationId=" +
			consultationId
	);
	await openWebsite("url", "" + consultationId);
	await waitFor(".page-header", "3000", "false", "exist");
	await browser.pause(2000);
};

export default addQuestionsToConsultation;
