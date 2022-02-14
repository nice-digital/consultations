import {tophatLogin} from "@nice-digital/wdio-cucumber-steps/lib/support/action/tophatLogIn";
import {waitFor} from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import {pause} from "@nice-digital/wdio-cucumber-steps/lib/support/action/pause";
import {refresh} from "@nice-digital/wdio-cucumber-steps/lib/support/action/refresh";
import selectors from "../selectors";

export const Login = (username, password) => {
	refresh();
	tophatLogin(username, password);
	pause("2000");
	waitFor(".page-header", "3000", "false", "exist");
	pause("2000");
};
