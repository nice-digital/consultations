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

Given(
	/^I comment on a Chapter$/,
	() => {
		browser.pause(1000);// And I pause for 1000ms
		browser.waitForVisible(".document-comment-container");// When I wait on element ".document-comment-container" to be visible
		browser.pause(1000);// And I pause for 1000ms
		browser.click(".chapter > .title [data-qa-sel='in-text-comment-button']");// When I click on the button ".chapter > .title [data-qa-sel='in-text-comment-button']"
		browser.pause(1000);// And I pause for 1000ms
		browser.waitForVisible("body [data-qa-sel='comment-box-title']");// Then I wait on element "body [data-qa-sel='comment-box-title']" to be visible
		// Then I expect that element "[data-qa-sel='comment-box-title']" contains the text "chapter"
	}
);

Given(
	/^I comment on a Section$/,
	() => {
		browser.pause(1000);// And I pause for 1000ms
		browser.click("[data-qa-sel='nav-list-item']:nth-of-type(4)");// When I click on the button "[data-qa-sel='nav-list-item']:nth-of-type(4)"
		browser.waitForVisible(".document-comment-container");// When I wait on element ".document-comment-container" to be visible
		browser.pause(1000);// And I pause for 1000ms
		browser.waitForVisible(".section:first-of-type > .title [data-qa-sel='in-text-comment-button']");// When I wait on element ".section:first-of-type > .title [data-qa-sel='in-text-comment-button']" to be visible
		browser.pause(1000);// And I pause for 1000ms
		browser.click(".section:first-of-type > .title [data-qa-sel='in-text-comment-button']");// When I click on the button ".section:first-of-type > .title [data-qa-sel='in-text-comment-button']"
		browser.pause(1000);// And I pause for 1000ms
		browser.waitForVisible("body [data-qa-sel='comment-box-title']");// Then I wait on element "body [data-qa-sel='comment-box-title']" to be visible
	}
);
