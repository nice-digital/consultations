import openWebsite from "@nice-digital/wdio-cucumber-steps/lib/support/action/openWebsite";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const addQuestionsToConsultation = (consultationId) => {
	openWebsite("url", "admin/InsertQuestionsForDocument1And2InConsultation?consultationId=" + consultationId);
	openWebsite("url", "" + consultationId);
	waitFor(".page-header");
	browser.pause(2000);
};

export default addQuestionsToConsultation;
