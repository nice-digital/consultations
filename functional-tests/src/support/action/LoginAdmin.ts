import idamGlobalnavLoginAdmin from "./idamGlobalnavLoginAdmin.js";
import pause from "../action/pause.js";
import selectors from "../selectors.js";

export async function LoginAdmin(username: string, password: string): Promise<void> {
	await pause("2000");
	await idamGlobalnavLoginAdmin(username, password);
	await pause("3000");
	// await refresh();
	await $("[data-qa-sel='changeable-page-header']").waitForDisplayed({timeout: 30000});
	await pause("2000");
};
