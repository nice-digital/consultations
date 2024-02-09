import clickElement from "../action/clickElement.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function clickCancelFilter(): Promise<void> {
	await clickElement("click", "selector", selectors.adminDownloadPage.cancelFilter);
	await pause("2000");
};

export default clickCancelFilter;
