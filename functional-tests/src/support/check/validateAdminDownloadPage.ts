import checkContainsText from "../check/checkContainsText.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function validateDownloadPageResultCount(countText: string): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.pageResultCount, "");
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		"",
		countText
	);
	await pause("5000");
};

export async function validateDownloadPageAllResults(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.paginationSection, "true");
	await pause("1000");
};

export async function validateFirstLinkInPagination(linkText: string): Promise<void> {
	await pause("2000")
	await waitForDisplayed(selectors.adminDownloadPage.firstPager, "");
	await pause("2000")
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.firstPager,
		"",
		linkText
	);
	await pause("1000");
};

export default validateDownloadPageResultCount;
