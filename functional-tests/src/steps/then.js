import { defineSupportCode } from "cucumber";

import checkContainsText from '../support/check/checkContainsText';

defineSupportCode(({ Then }) => {
    Then(
        /^I expect that (button|element) "([^"]*)?"( not)* matches the text "([^"]*)?"$/,
        checkContainsText
    );
});