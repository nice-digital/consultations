import globalnavLogin from "@nice-digital/wdio-cucumber-steps/lib/support/action/globalnavLogin";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const Login = (username, password) => {
	browser.refresh();
	globalnavLogin(username, password);
	browser.pause(2000);
	waitFor(".page-header");
	browser.pause(2000);
}

