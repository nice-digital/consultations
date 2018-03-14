import { Then } from "cucumber";

import checkContainsText from '../support/check/checkContainsText';

Then(
    /^I expect that (button|element) "([^"]*)?"( not)* matches the text "([^"]*)?"$/,
    checkContainsText
);