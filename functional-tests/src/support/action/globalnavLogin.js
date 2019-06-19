import globalnavLogin from "@nice-digital/wdio-cucumber-steps/lib/support/action/globalnavLogin";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import scroll from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const Login = (username, password) => {
	browser.refresh();
	scroll(selectors.documentPage.globalNavSigin);
	waitForVisible(selectors.documentPage.globalNavSigin);
	globalnavLogin(username, password);
	browser.pause(2000);
	waitFor(".page-header");
	browser.pause(2000);
}

