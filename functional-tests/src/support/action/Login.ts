import idamGlobalnavLogin from "./idamGlobalnavLogin";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function Login(username: string, password: string): Promise<void> {
	// await refresh();
	await pause("2000");
	await $("[data-qa-sel='open-commenting-panel']").waitForDisplayed();
	await $("[data-qa-sel='open-commenting-panel']").click();
	await idamGlobalnavLogin(username, password);
	// await pause("2000");
	// await refresh();
	// await waitForDisplayed("[data-qa-sel='changeable-page-header']", "false");
	// await pause("2000");
};
