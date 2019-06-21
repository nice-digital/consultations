import login from "./login";
import selectors from "../selectors";
import waitForVisible from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForVisible";
import scroll from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";

module.exports = (username, password) => {
	// If you are already logged in
	if (browser.getCookie("__nrpa_2.2")) {
		return;
	}

	browser.waitForExist("header[aria-label='Site header']");

	if (browser.isVisible("#cookie-banner-description")){
		browser.click("#cookie-banner-description + button"); //dismiss cookie notification, if shown.
	}	

	// var runInBrowser = function(argument) { //this hacky function is here to click things in the browser that selenium can't.
	// 	argument.click();
	// };

	browser.setViewportSize({
        width: 1366,
        height: 768
    });

	var headerMenuShown = browser.isVisible("#header-menu-button"); //the global nav shows a "menu" button for small screens, or a "sign in" button for big screens.
	if (headerMenuShown) { //if a small screen
		browser.click("#header-menu-button"); //click the menu button
		scroll(selectors.documentPage.globalNavSiginMenuSignInButton); //scroll to the signin button in the menu
		waitForVisible(selectors.documentPage.globalNavSiginMenuSignInButton); 

		var signinButtonInMenu = browser.$(selectors.documentPage.globalNavSiginMenuSignInButton);
		console.log("signinButtonInMenu:")
		console.log(signinButtonInMenu);
		//browser.execute(runInBrowser, signinButtonInMenu);

		browser.click(selectors.documentPage.globalNavSiginMenuSignInButton);
	} else {
		scroll(selectors.documentPage.globalNavSigin);
		waitForVisible(selectors.documentPage.globalNavSigin);

		var signinButton = browser.$(selectors.documentPage.globalNavSigin);
		console.log("signinButton:")
		console.log(signinButton);

		//browser.execute(runInBrowser, signinButton);

		browser.click(selectors.documentPage.globalNavSigin);
	}
	login(username, password);
};
