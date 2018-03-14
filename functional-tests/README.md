# Consultations functional tests

> Webdriver.io powered browser-based automated functional tests for the Consultation Comments project.

Loosley based on the [WebdriverIO Cucumber Boilerplate](https://github.com/webdriverio/cucumber-boilerplate).

## Required software

- Node
- Chrome
- Firefox

## Commands

 When running locally, the tests will run in Chrome and Firefox (if installed), via [wdio-selenium-standalone-service](http://webdriver.io/guide/services/selenium-standalone.html).

Run:

```sh
npm i
npm test
```

This automatically creates a selenium server and runs the tests (via [webdriver.io test runner](http://webdriver.io/guide/testrunner/gettingstarted.html)) against both Chrome and Firefox.

> Note: On Windows run in *cmd* and not *GitBash* otherwise the window just hangs.

### Excluding tests

Exclude tests by using the `@pending` [cucumber tag](https://github.com/cucumber/cucumber/wiki/Tags).

## Docker

Running tests on Docker is a good option as it means you don't need browsers installed on the host machine, and the Selenium grid is automatically created for you. This is useful on a TeamCity build agent where you can't rely on Chrome and Firefox being installed.

In bash:

```sh
./run.sh
```

Or in CMD on Windows:

```sh
run
```

Or in PowerShell:

```sh
cmd /c "run"
```

