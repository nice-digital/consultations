// import {globalnavLogin} from "@nice-digital/wdio-cucumber-steps/lib/support/action/globalnavLogin";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export async function sidebarLogin(username: string, password: string): Promise<void> {
	await refresh();
	await clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
	await waitForDisplayed(".LoginBanner", "");
	await clickElement("click", "selector", "a[href*='/consultations/account/login?returnURL=']");
	await waitForDisplayed("[data-qa-sel='login-email']", "");
	await setInputField("set", process.env[username], "[data-qa-sel='login-email']");
	await setInputField("set", process.env[password], "[data-qa-sel='login-password']");
	await clickElement("click", "selector", "[data-qa-sel='login-button']");
	await pause("2000");
	await waitForDisplayed("[data-qa-sel='open-commenting-panel']", "");
	await pause("2000");
}

