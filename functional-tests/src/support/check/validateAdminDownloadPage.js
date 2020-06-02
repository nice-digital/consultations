import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateDownloadPageResultCount = (countText) => {
	checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		countText
	);
	pause(1000);
};

export const validateDownloadPageAllResults = () => {
	waitForVisible(selectors.adminDownloadPage.paginationSection, "true");
	pause(1000);
};

export const validateFirstLinkInPagination = (linkText) => {
	waitForVisible(selectors.adminDownloadPage.firstPager);
	checkContainsText(
		"selector",
		selectors.adminDownloadPage.firstPager,
		linkText
	);
	pause(1000);
};

export default validateDownloadPageResultCount;
