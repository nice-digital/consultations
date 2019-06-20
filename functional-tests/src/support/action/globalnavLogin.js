import globalnavLogin from "./globalnavLogin_Local";
//import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
//import scroll from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const Login = (username, password) => {
	browser.refresh();
	globalnavLogin(username, password);
	browser.pause(2000);
	waitFor(".page-header");
	browser.pause(2000);
}

