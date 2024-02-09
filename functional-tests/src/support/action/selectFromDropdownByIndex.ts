import selectors from "../selectors.js";

export async function selectValueFromDropdown(index: string): Promise<void> {
	const optionIndex = parseInt(index, 10);
	await $(selectors.adminDownloadPage.numberResultsOnPage).selectByIndex(optionIndex);
	// await selectOptionByIndex(
	// 	index,
	// 	obsolete,
	// 	selectors.adminDownloadPage.numberResultsOnPage
	// );
};

export default selectValueFromDropdown;
