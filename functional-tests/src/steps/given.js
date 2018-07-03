import "@nice-digital/wdio-cucumber-steps/lib/given";
import { Given } from "cucumber";

import deleteComments from '../support/action/deleteComments';

Given(
    /^I delete all comments on the page$/,
    deleteComments
);
