import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateDownloadPageResultCount = (countText) => {
	waitForDisplayed(selectors.adminDownloadPage.pageResultCount, "false");
	checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		"false",
		countText
	);
	pause("5000");
};

export const validateDownloadPageAllResults = () => {
	waitForDisplayed(selectors.adminDownloadPage.paginationSection, "true");
	pause("1000");
};

export const validateFirstLinkInPagination = (linkText) => {
	waitForDisplayed(selectors.adminDownloadPage.firstPager, "false");
	checkContainsText(
		"element",
		selectors.adminDownloadPage.firstPager,
		"false",
		linkText
	);
	pause("1000");
};

export default validateDownloadPageResultCount;
