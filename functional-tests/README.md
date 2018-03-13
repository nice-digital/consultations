# Consultations functional tests

> Webdriver.io powered browser-based automated functional tests for the Consultation Comments project.

Loosley based on the [WebdriverIO Cucumber Boilerplate](https://github.com/webdriverio/cucumber-boilerplate). When running locally, the tests will run in Chrome and Firefox, via [wdio-selenium-standalone-service](http://webdriver.io/guide/services/selenium-standalone.html).

## Required software

- Node
- Chrome
- Firefox

## Commands

Run:

```sh
npm i
npm test
```

This automatically creates a selenium server and runs the tests (via [webdriver.io test runner](http://webdriver.io/guide/testrunner/gettingstarted.html)) against both Chrome and Firefox.

> Note: On Windows run in *cmd* and not *GitBash* otherwise the window just hangs.

### Excluding tests

Exclude tests by using the `@pending` [cucumber tag](https://github.com/cucumber/cucumber/wiki/Tags).
