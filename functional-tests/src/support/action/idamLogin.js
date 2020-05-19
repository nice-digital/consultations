export const idamLogin = (username, password) => {
	browser.waitForVisible("body [data-qa-sel='login-email']", 40000);
	browser.setValue("[data-qa-sel='login-email']", process.env[username]);
	browser.setValue("[data-qa-sel='login-password']", process.env[password]);
	browser.click("[data-qa-sel='login-button']");
	browser.pause(2000);
};

export default idamLogin;
