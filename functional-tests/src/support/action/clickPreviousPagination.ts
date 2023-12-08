import clickElement from "../action/clickElement";
import pause from "../action/pause";
import selectors from "../selectors";


export async function clickPreviousPagination(): Promise<void> {
	await clickElement("click", "selector", selectors.adminDownloadPage.firstPager);
	await pause("2000");
}
