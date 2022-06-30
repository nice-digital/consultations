import {selectOptionByIndex} from "@nice-digital/wdio-cucumber-steps/lib/support/action/selectOptionByIndex";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function selectValueFromDropdown(index: string): Promise<void> {
	await selectOptionByIndex(
		index,
		"obsolete",
		selectors.adminDownloadPage.numberResultsOnPage
	);
};

export default selectValueFromDropdown;
