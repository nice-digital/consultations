import login from "@nice-digital/wdio-cucumber-steps/lib/support/action/login";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

const globalnavLogin = (username, password) => { //todo: potentially - move this function to wdio cucumber steps.
	// If you are already logged in
	if (browser.getCookie("__nrpa_2.2")) {
		return;
	}

	var headerMenuShown = browser.isVisible("#header-menu-button");
	if (headerMenuShown) {
		browser.waitForExist("header[aria-label='Site header']");
		browser.click("#header-menu-button");
		browser.click("#header-menu a[href*='accounts.nice.org.uk/signin']");
	} else {
		browser.click("#header-menu-button+* a[href*='accounts.nice.org.uk/signin']");
	}	
	login(username, password);
};

export const Login = (username, password) => {
	browser.refresh();
	globalnavLogin(username, password);
	browser.pause(2000);
	waitFor(".page-header");
	browser.pause(2000);
}

