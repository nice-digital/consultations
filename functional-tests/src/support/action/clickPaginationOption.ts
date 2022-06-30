import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export async function clickSecondPaginationOption(): Promise<void> {
	await clickElement("click", "button", selectors.adminDownloadPage.secondPager);
	await pause("2000");
};

export async function clickNextPagination(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.nextPager, "");
	await clickElement("click", "button", selectors.adminDownloadPage.nextPager);
	await pause("2000");
};

export async function clickPreviousPagination(): Promise<void> {
	await clickElement("click", "button", selectors.adminDownloadPage.firstPager);
	await pause("2000");
};

export default clickSecondPaginationOption;
