import checkContainsText from "../check/checkContainsText";
import waitForDisplayed from "../action/waitForDisplayed";
import pause from "../action/pause";
import selectors from "../selectors";

export async function validateDownloadPageResultCount(countText): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.pageResultCount, "");
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		null,
		countText
	);
	await pause("5000");
};

export async function validateDownloadPageAllResults(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.paginationSection, "true");
	await pause("1000");
};

export async function validateFirstLinkInPagination(linkText): Promise<void> {
	await pause("2000")
	await waitForDisplayed(selectors.adminDownloadPage.firstPager, "");
	await pause("2000")
	await checkContainsText(
		"element",
		selectors.adminDownloadPage.firstPager,
		null,
		linkText
	);
	await pause("1000");
};

export default validateDownloadPageResultCount;
