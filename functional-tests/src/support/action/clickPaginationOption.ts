import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import selectors from "../selectors";

export const clickSecondPaginationOption = () => {
	clickElement("click", "button", selectors.adminDownloadPage.secondPager);
	pause("2000");
};

export const clickNextPagination = () => {
	waitForDisplayed(selectors.adminDownloadPage.nextPager, "false");
	clickElement("click", "button", selectors.adminDownloadPage.nextPager);
	pause("2000");
};

export const clickPreviousPagination = () => {
	clickElement("click", "button", selectors.adminDownloadPage.firstPager);
	pause("2000");
};

export default clickSecondPaginationOption;
