import { defineSupportCode } from "cucumber";

defineSupportCode(({ When }) => {
    When(
        /^I click on Fetch Data in the nav$/,
        () => {
            browser.element("a[href='/consultations/fetchdata']").click();
        }
    );
});