import { defineSupportCode } from "cucumber";

import clickElement from "../support/action/clickElement";

defineSupportCode(({ When }) => {

    // E.g. When I click on text "Title here" in ".ancestor"
    When(
        /^I (click|doubleclick) on (text|element) "([^"]*)?"(?: in ?"([^"]*)?")?$/,
        clickElement
    );
});