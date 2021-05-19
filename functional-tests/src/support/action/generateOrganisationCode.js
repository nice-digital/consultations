import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import pause from	"@nice-digital/wdio-cucumber-steps/lib/support/action/pause";

export const generateOrganisationCode = () => {

	browser.click("[data-qa-sel='share-with-org-button']");
	browser.click("[data-qa-sel='generate-code-button']");
	waitForVisible("[data-qa-sel='copy-code-button']");
	browser.click("[data-qa-sel='copy-code-button']");
	browser.keys(['CTRL', 'C'])
	pause(2000);
	browser.click(".gn_2hlYN");
	browser.click(".gn_2nlnx");
	browser.click(".gn_2nlnx a[href='/consultations/account/logout']", 2000);
};
