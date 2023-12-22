import acceptCookieBanner from "./acceptCookieBanner.js";
//import scroll from "../action/scroll.js";
import waitForDisplayed from "../action/waitForDisplayed.js";
import setInputField from "../action/setInputField.js";
import clickElement from "../action/clickElement.js";
import pause from "../action/pause.js";

export async function idamLogin(username: string, password: string): Promise<void> {
	await pause("2000");
	await acceptCookieBanner();
	//await scroll("body [data-qa-sel='login-email']");
	await waitForDisplayed("body [data-qa-sel='login-email']", "");
	await setInputField("set", process.env[username]!, "[data-qa-sel='login-email']");
	await browser.pause(2000);
	await waitForDisplayed("body [data-qa-sel='login-password']", "");
	await setInputField("set", process.env[password]!, "[data-qa-sel='login-password']");
	await browser.pause(2000);
	await clickElement("click", "selector", "[data-qa-sel='login-button']");
	await pause("2000");
};

export default idamLogin;
