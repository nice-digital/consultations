import idamGlobalnavLoginAdmin from "./idamGlobalnavLoginAdmin";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function LoginAdmin(username: string, password: string): Promise<void> {
	await pause("2000");
	await idamGlobalnavLoginAdmin(username, password);
	await pause("2000");
	// await refresh();
	await waitForDisplayed("[data-qa-sel='changeable-page-header']", "");
	await pause("2000");
};
