import idamGlobalnavLoginAdmin from "./idamGlobalnavLoginAdmin";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const LoginAdmin = (username, password) => {
	browser.pause(2000);
	idamGlobalnavLoginAdmin(username, password);
	browser.pause(2000);
	browser.refresh();
	waitForVisible("[data-qa-sel='changeable-page-header']");
	browser.pause(2000);
};
