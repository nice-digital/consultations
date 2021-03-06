import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import selectors from "../selectors";

export const clickSecondPaginationOption = () => {
	clickElement("click", "button", selectors.adminDownloadPage.secondPager);
	pause(2000);
};

export const clickNextPagination = () => {
	waitForVisible(selectors.adminDownloadPage.nextPager);
	clickElement("click", "button", selectors.adminDownloadPage.nextPager);
	pause(2000);
};

export const clickPreviousPagination = () => {
	clickElement("click", "button", selectors.adminDownloadPage.firstPager);
	pause(2000);
};

export default clickSecondPaginationOption;
