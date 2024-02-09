import waitForDisplayed from "../action/waitForDisplayed.js";
import clickElement from "../action/clickElement.js";
import setInputField from "../action/setInputField.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function sidebarLogin(username: string, password: string): Promise<void> {
	await browser.refresh();
	await clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
	await waitForDisplayed(".LoginBanner", "");
	await clickElement("click", "selector", "a[href*='/consultations/account/login?returnURL=']");
	await waitForDisplayed("[data-qa-sel='login-email']", "");
	await setInputField("set", process.env[username]!, "[data-qa-sel='login-email']");
	await setInputField("set", process.env[password]!, "[data-qa-sel='login-password']");
	await clickElement("click", "selector", "[data-qa-sel='login-button']");
	await pause("2000");
	await waitForDisplayed("[data-qa-sel='open-commenting-panel']", "");
	await pause("2000");
}

