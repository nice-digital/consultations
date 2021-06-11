import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";

export const enterEmailaddress = (emailaddress) => {
	browser.pause(2000);
	browser.waitForVisible("input#emailAddress", 40000);
	browser.setValue("input#emailAddress", process.env[emailaddress]);
	browser.pause(2000);
};
