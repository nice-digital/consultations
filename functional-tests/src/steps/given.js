import { defineSupportCode } from "cucumber";

import openUrl from '../support/action/openUrl';

defineSupportCode(({ Given }) => {
    Given(
        /^I open the page "([^"]*)?"$/,
        openUrl
    );
});