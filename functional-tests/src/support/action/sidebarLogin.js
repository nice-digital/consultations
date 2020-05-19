import globalnavLogin from "@nice-digital/wdio-cucumber-steps/lib/support/action/globalnavLogin";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible"
import clickElement from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement"
import setInputField from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField"
import selectors from "../selectors";

export const sidebarLogin = (username, password) => {
	browser.refresh();
	clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
	waitForVisible(".LoginBanner");
	clickElement("click", "selector", "a[href*='/consultations/account/login?returnURL=']");
	waitForVisible("[data-qa-sel='login-email']");
	browser.setValue("[data-qa-sel='login-email']", process.env[username]);
	browser.setValue("[data-qa-sel='login-password']", process.env[password]);
	clickElement("click", "selector", "[data-qa-sel='login-button']");
	browser.pause(2000);
	waitForVisible("[data-qa-sel='open-commenting-panel']");
	browser.pause(2000);
}

