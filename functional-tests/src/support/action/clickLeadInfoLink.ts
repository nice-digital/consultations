import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function clickLeadInfoLink(): Promise<void> {
	await clickElement("click", "selector", "a[href='/consultations/leadinformation']");
	await pause("2000");
};

export default clickLeadInfoLink;
