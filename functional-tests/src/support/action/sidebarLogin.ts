// import {globalnavLogin} from "@nice-digital/wdio-cucumber-steps/lib/support/action/globalnavLogin";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {clickElement} from "@nice-digital/wdio-cucumber-steps/lib/support/action/clickElement";
import {setInputField} from "@nice-digital/wdio-cucumber-steps/lib/support/action/setInputField";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const sidebarLogin = (username, password) => {
	refresh();
	clickElement("click", "selector", "[data-qa-sel='open-commenting-panel']");
	waitForDisplayed(".LoginBanner", "false");
	clickElement("click", "selector", "a[href*='/consultations/account/login?returnURL=']");
	waitForDisplayed("[data-qa-sel='login-email']", "false");
	setInputField("set", process.env[username], "[data-qa-sel='login-email']");
	setInputField("set", process.env[password], "[data-qa-sel='login-password']");
	clickElement("click", "selector", "[data-qa-sel='login-button']");
	pause("2000");
	waitForDisplayed("[data-qa-sel='open-commenting-panel']", "false");
	pause("2000");
}

