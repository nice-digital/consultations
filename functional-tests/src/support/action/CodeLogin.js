import acceptCookieBanner from "./acceptCookieBanner";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const CodeLogin = (Organisationcode) => {
	browser.pause(2000);
	acceptCookieBanner();
	browser.pause(2000);
	browser.waitForVisible("input#orgCode-document", 40000);
	browser.setValue("input#orgCode-document", process.env[Organisationcode]);
	browser.waitForVisible("//BUTTON[@class='btn btn--cta'][text()='Confirm']", 40000);
	browser.click("//BUTTON[@class='btn btn--cta'][text()='Confirm']");
	browser.pause(2000);
};
