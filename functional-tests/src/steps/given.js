import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from "cucumber";

import deleteComments from '../support/action/deleteComments';

Given(
    /^I delete all comments on the page$/,
    deleteComments
);

Given(
	/^I comment on a Document$/,
	() => {
		browser.pause(1000);// And I pause for 1000ms
		browser.click("[data-qa-sel='comment-on-consultation-document']")// When I click on the button "[data-qa-sel='comment-on-consultation-document']"
		browser.pause(1000);// And I pause for 1000ms
		browser.waitForVisible("body [data-qa-sel='comment-box-title']", 1000)// Then I wait on element "body [data-qa-sel='comment-box-title']" for 10000ms to be visible
	}
);
