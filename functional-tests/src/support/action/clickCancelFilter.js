import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const clickCancelFilter = () => {
	clickElement("click", "button", selectors.adminDownloadPage.cancelFilter);
	pause(2000);
};

export default clickCancelFilter;
