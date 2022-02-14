import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function clickCancelFilter(): Promise<void> {
	await clickElement("click", "button", selectors.adminDownloadPage.cancelFilter);
	await pause("2000");
};

export default clickCancelFilter;
