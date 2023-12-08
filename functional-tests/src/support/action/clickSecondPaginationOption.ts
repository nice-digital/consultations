import clickElement from "../action/clickElement";
import pause from "../action/pause";
import selectors from "../selectors";

export async function clickSecondPaginationOption(): Promise<void> {
	await clickElement("click", "selector", selectors.adminDownloadPage.secondPager);
	await pause("2000");
};

export default clickSecondPaginationOption;
