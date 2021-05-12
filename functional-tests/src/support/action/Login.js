import idamGlobalnavLogin from "./idamGlobalnavLogin";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const Login = (username, password) => {
	browser.refresh();
	browser.pause(2000);
	// waitForVisible("[data-qa-sel='open-commenting-panel']");
	// browser.click("[data-qa-sel='open-commenting-panel']");
	idamGlobalnavLogin(username, password);
	browser.pause(2000);
	browser.refresh();
	waitForVisible("[data-qa-sel='changeable-page-header']");
	browser.pause(2000);
};
