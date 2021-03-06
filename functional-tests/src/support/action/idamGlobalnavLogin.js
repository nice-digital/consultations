import idamLogin from "./idamLogin";

export const idamGlobalNavLogin = (username, password) => {
	// If you are already logged in
	if (browser.getCookie("__nrpa_2.2")) {
		return;
	}

	var headerMenuExists = browser.waitForExist(
		"body #header-menu a[href*='/consultations/account/login?returnURL=']",
		2000
	);
	if (headerMenuExists !== true) {
		browser.refresh();
	} else {
		var headerMenuShown = browser.isVisible("body #header-menu-button");
		if (headerMenuShown) {
			browser.click("body #header-menu-button");
			browser.click(
				"body #header-menu a[href*='/consultations/account/login?returnURL=']"
			);
		} else {
			browser.click(
				"body #header-menu-button+* a[href*='/consultations/account/login?returnURL=']"
			);
		}
	}
	idamLogin(username, password);
};

export default idamGlobalNavLogin;
