import idamGlobalnavLogin from "./idamGlobalnavLogin";
import pause from "../action/pause";
import selectors from "../selectors";

export async function Login(username: string, password: string): Promise<void> {
	await browser.refresh();
	await pause("2000");
	await $("[data-qa-sel='open-commenting-panel']").waitForDisplayed();
	await $("[data-qa-sel='open-commenting-panel']").click();
	await idamGlobalnavLogin(username, password);
};
