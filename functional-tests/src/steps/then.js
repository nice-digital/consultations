import { defineSupportCode } from "cucumber";

defineSupportCode(({ Then }) => {
    Then(
        /^I should see weather forecast$/,
        () => {
            $("h1='Weather forecast'").should.not.be.undefined;
        }
    );
});