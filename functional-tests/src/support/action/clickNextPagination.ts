import clickElement from "../action/clickElement";
import pause from "../action/pause";
import waitForDisplayed from "../action/waitForDisplayed";
import selectors from "../selectors";


export async function clickNextPagination(): Promise<void> {
	await waitForDisplayed(selectors.adminDownloadPage.nextPager, "");
	await clickElement("click", "selector", selectors.adminDownloadPage.nextPager);
	await pause("2000");
};

export default clickNextPagination;
