import clickElement from "../action/clickElement";
import pause from "../action/pause";
import selectors from "../selectors";

export async function clickCancelFilter(): Promise<void> {
	await clickElement("click", "selector", selectors.adminDownloadPage.cancelFilter);
	await pause("2000");
};

export default clickCancelFilter;
