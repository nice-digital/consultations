import { defineSupportCode } from "cucumber";

defineSupportCode(({ Given }) => {
    Given(
        /^I am on the homepage$/,
        () => {
            browser.url("");
        }
    );
});