import acceptCookieBanner from "./acceptCookieBanner";
import scroll from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";

export const idamLogin = (username, password) => {
	browser.pause(2000);
	acceptCookieBanner();
	scroll("body [data-qa-sel='login-email']");
	browser.waitForVisible("body [data-qa-sel='login-email']", 40000);
	browser.setValue("[data-qa-sel='login-email']", process.env[username]);
	browser.setValue("[data-qa-sel='login-password']", process.env[password]);
	browser.click("[data-qa-sel='login-button']");
	browser.pause(2000);
};

export default idamLogin;
