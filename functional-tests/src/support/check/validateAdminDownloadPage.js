import checkContainsText from "@nice-digital/wdio-cucumber-steps/lib/support/check/checkContainsText";
import pause from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const validateDownloadPageResultCount = (countText) => {
	checkContainsText(
		"element",
		selectors.adminDownloadPage.pageResultCount,
		countText
	);
	pause(1000);
};

export default validateDownloadPageResultCount;
