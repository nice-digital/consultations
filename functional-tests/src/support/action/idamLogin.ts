import acceptCookieBanner from "./acceptCookieBanner";
import {scroll} from "@nice-digital/wdio-cucumber-steps/lib/support/action/scroll";
import { waitForDisplayed } from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import { setInputField } from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import { clickElement } from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import { pause } from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";

export async function idamLogin(username: string, password: string): Promise<void> {
	await pause("2000");
	await acceptCookieBanner();
	await scroll("body [data-qa-sel='login-email']");
	await waitForDisplayed("body [data-qa-sel='login-email']", "false");
	await setInputField("set", process.env[username], "[data-qa-sel='login-email']");
	await setInputField("set", process.env[password], "[data-qa-sel='login-password']");
	await clickElement("click", "selector", "[data-qa-sel='login-button']");
	await pause("2000");
};

export default idamLogin;
