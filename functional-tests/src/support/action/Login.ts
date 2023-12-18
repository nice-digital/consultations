import idamGlobalnavLogin from "./idamGlobalnavLogin.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function Login(username: string, password: string): Promise<void> {
	await browser.refresh();
	await pause("2000");
	await $("[data-qa-sel='open-commenting-panel']").waitForDisplayed();
	await $("[data-qa-sel='open-commenting-panel']").click();
	await idamGlobalnavLogin(username, password);
};
