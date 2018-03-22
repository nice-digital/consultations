import { Given } from "cucumber";

import openUrl from '../support/action/openUrl';

Given(
    /^I open the page "([^"]*)?"$/,
    openUrl
);