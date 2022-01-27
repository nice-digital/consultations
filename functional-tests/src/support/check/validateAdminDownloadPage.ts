import {checkContainsText} from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function validateDownloadPageResultCount(countText): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.pageResultCount, "");
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		"false",
		countText
	);
	await pause("5000");
};

export async function validateDownloadPageAllResults(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.paginationSection, "true");
	await pause("1000");
};

export async function validateFirstLinkInPagination(linkText): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.firstPager, "");
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.firstPager,
		"false",
		linkText
	);
	await pause("1000");
};

export default validateDownloadPageResultCount;
