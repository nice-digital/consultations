import idamGlobalnavLoginAdmin from "./idamGlobalnavLoginAdmin";
import {waitForDisplayed} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitForDisplayed";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import selectors from "../selectors";

export const LoginAdmin = (username, password) => {
	pause("2000");
	idamGlobalnavLoginAdmin(username, password);
	pause("2000");
	refresh();
	waitForDisplayed("[data-qa-sel='changeable-page-header']", "false");
	pause("2000");
};
