import idamLogin from "./idamLogin";

export const idamGlobalNavLoginAdmin = (username, password) => {
	// If you are already logged in
	if (browser.getCookie("__nrpa_2.2")) {
		return;
	}

	var headerMenuExists = browser.waitForExist("body .gn_qKxI2", 2000);
	if (headerMenuExists !== true) {
		browser.refresh();
	} else {
		var headerMenuShown = browser.isVisible("body #header-menu-button");
		if (headerMenuShown) {
			browser.click("body #header-menu-button");
			browser.click("body .gn_qKxI2");
		} else {
			browser.click("body .gn_qKxI2");
		}
	}
	idamLogin(username, password);
};

export default idamGlobalNavLoginAdmin;
