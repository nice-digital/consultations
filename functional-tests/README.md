# Consultations functional tests

> Webdriver.io powered browser-based automated functional tests for the Consultation Comments project.

Loosley based on the [WebdriverIO Cucumber Boilerplate](https://github.com/webdriverio/cucumber-boilerplate).

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
**Table of Contents**  *generated with [DocToc](https://github.com/thlorenz/doctoc)*

- [Required software](#required-software)
- [Commands](#commands)
  - [Excluding tests](#excluding-tests)
- [Docker](#docker)
  - [Docker Dev](#docker-dev)
    - [Running locally](#running-locally)
    - [Stopping the container](#stopping-the-container)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Required software

- Node >= 8.11.1 (latest LTS as of May 2018) and npm >= 5.6.0
- Chrome, any version
- Firefox, optional, any versoin

## Running locally

 When running locally, the tests will run in Chrome, via [wdio-selenium-standalone-service](http://webdriver.io/guide/services/selenium-standalone.html). You can run them in Firefox if you've got in installed - uncomment the Firefox capability from [wdio.conf.js](wdio.conf.js).

Once you've installed Node:

1) Open a terminal of your choice. Note: On Windows run in *cmd* and not *GitBash* otherwise the window just hangs.
2) `cd` into the *functional-tests* directory: `cd functional-tests`
3) install package dependencies from npm by running `npm i`. You only need to do this the first time you run the test: you can skip this step on subsequent sessions.

Then run the tests by running the following:

```sh
npm test
```

This automatically creates a selenium server and runs the tests (via [webdriver.io test runner](http://webdriver.io/guide/testrunner/gettingstarted.html)) against Chrome.

### Excluding tests

Exclude tests by using the `@pending` [cucumber tag](https://github.com/cucumber/cucumber/wiki/Tags).

### Run a single test

TODO

## Docker

Running tests on Docker is a good option as it means you don't need browsers installed on the host machine, and the Selenium grid is automatically created for you. This is useful on a TeamCity build agent where you can't rely on Chrome and Firefox being installed.

You can either run the tests via [Docker Compose](#docker-compose) which handles everything for you, or run the [webapp as a single container](#docker-single-container) if you're hacking locally.

Either way, first you need [publish the webapp](#publish) and set some [environment variables](#environment-variables).

### Publish

The docker images expect the dotnet core app to have already been published into the *published-app* folder. This is what happens on TeamCity. To run locally, first build and publish the dotnet core webapp by using the following command:

```sh
dotnet publish -c Release -o published-app /property:PublishWithAspNetCoreTargetManifest="false"
```

### Environment variables

When we deploy the webapp with Octo we use variable substitution to change settings in [appsettings.json](Comments/appsettings.json). When we're running via Docker, we don't have this luxury. So we have to use environment variables when we run the container(s) to pass in these settings. Run the following, with the correct values, from your terminal:

```sh
export DEFAULT_CONNECTION=""
export FEEDS_APIKEY=""
export FEEDS_BASEPATH=""
export FEEDS_CHAPTER=""
export FEEDS_DETAIL=""
export FEEDS_LIST=""
export GILLIAM_CLIENT_CERTIFICATE_BASE64=""
export GILLIAM_BASE_PATH=""
export GILLIAM_GET_CLAIMS_URL=""
export GILLIAM_REALM=""
export MSSQL_SA_PASSWORD=""
```

> Note: you can get the values from your local User Secrets in Visual Studio.

### Docker Compose

We use [Docker Compose](https://docs.docker.com/compose/) to create all of the containers needed to run the application and tests. It consists of 7 containers:

- *[Comment Collection web application](../Comments/Dockerfile)* - the dotnet core and React application
- SQL Server database
- Fake InDev feed backend [todo]
- *[The tests themselves](Dockerfile)*
- *Selenium Grid Hub* - See https://github.com/SeleniumHQ/docker-selenium#via-docker-compose
- *Chrome* - Grid Node with Chrome installed
- *Firefox* - Grid Node with Firefox installed

The following command create a network, builds images and runs all the containers, then runs the tests against Chrome and Firefox, via the selenium grid:

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

### Docker single container

#### Build the image

Make sure you've [published](#publish) the Comment Collection webapp and set the required [environment variables](#environment-variables) before building the image:

```sh
cd Comments
docker build . -t comments
```

#### Running the image

Run the image, passing in the environment variable names:

```sh
MSYS_NO_PATHCONV=1 docker run --name comments --rm -p 8081:80 \
    -e DEFAULT_CONNECTION \
    -e FEEDS_APIKEY \
    -e FEEDS_BASEPATH \
    -e FEEDS_CHAPTER \
    -e FEEDS_DETAIL \
    -e FEEDS_LIST \
    -e GILLIAM_CLIENT_CERTIFICATE_BASE64 \
    -e GILLIAM_BASE_PATH \
    -e GILLIAM_GET_CLAIMS_URL \
    -e GILLIAM_REALM \
    comments
```

Then browser to http://localhost:8081 on your host machine.

#### Stopping the container

```sh
docker stop comments
```