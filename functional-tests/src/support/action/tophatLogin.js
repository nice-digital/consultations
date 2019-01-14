import tophatLogin from "@nice-digital/wdio-cucumber-steps/lib/support/action/tophatLogIn";
import waitFor from "@nice-digital/wdio-cucumber-steps/lib/support/action/waitFor";
import selectors from "../selectors";

export const tophatLogin = (username, password) => {
	browser.refresh();
	tophatLogin(username, password);// When I log in to Accounts via TopHat with username "ACCOUNTS_EMAIL2" and password "ACCOUNTS_PASSWORD"
	waitFor(.page-header); // When I wait on element ".page-header" to exist
	browser.pause(2000); // And I pause for 1000ms
}

