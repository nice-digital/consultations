import clickElement from "../action/clickElement.js";
import pause from "../action/pause.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import selectors from "../selectors.js";


export async function clickNextPagination(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.nextPager, "");
	await clickElement("click", "selector", selectors.adminDownloadPage.nextPager);
	await pause("2000");
};

export default clickNextPagination;
