# Consultations functional tests

> Webdriver.io powered browser-based automated functional tests for the Consultation Comments project.

Loosley based on the [WebdriverIO Cucumber Boilerplate](https://github.com/webdriverio/cucumber-boilerplate).

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
**Table of Contents**  *generated with [DocToc](https://github.com/thlorenz/doctoc)*

- [Required software](#required-software)
- [Running locally](#running-locally)
  - [Excluding tests](#excluding-tests)
  - [Run a single test](#run-a-single-test)
- [Docker](#docker)
  - [Publish](#publish)
  - [Environment variables](#environment-variables)
  - [Docker Compose](#docker-compose)
  - [Docker single container](#docker-single-container)
    - [Build the image](#build-the-image)
    - [Running the image](#running-the-image)
    - [Stopping the container](#stopping-the-container)
  - [Troubleshooting and gotchas](#troubleshooting-and-gotchas)
    - [Error starting userland proxy](#error-starting-userland-proxy)
    - [bad interpreter: No such file or directory](#bad-interpreter-no-such-file-or-directory)
    - [Unknown locator strategy id](#unknown-locator-strategy-id)

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
export LOGGING_LOG_FILE_PATH=""
export APPSETTINGS_ENVIRONMENT_NAME=""
export APPSETTINGS_ENVIRONMENT_SECURESITE=""
export APPSETTINGS_ENVIRONMENT_REALM=""
export INDEV_APIKEY=""
export INDEV_BASEPATH=""
export INDEV_CHAPTER=""
export INDEV_DETAIL=""
export INDEV_LIST=""
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
./docker-run.sh
```
### Developing in Docker

We have set up a way of creating and running up the containers in docker so that we can develop and run the tests within a running docker environment. This also mounts a volume on the tests container so that any changes made locally in the test project is instantly available within the running container.

First, the following script will set up the docker containers needed for testing, mount a volume on the tests container and leave the user in the container ready to run the tests:

```sh
./docker-dev.sh
```
You will now notice you have started up the docker environment and you appear on the command line within the tests container. To run the tests run the following:

```sh
npm run test:teamcity -- --host hub -b http://niceorg:8080/consultations/
```
Now, if you make a change to the tests locally, you have a way of testing the change within docker giving you more confidence that it will run in the teamcity build agent when you push it up. 

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
MSYS_NO_PATHCONV=1 docker run --name comments --rm -p 8080:8080 \
    -e DEFAULT_CONNECTION \
    -e LOGGING_LOG_FILE_PATH \
    -e APPSETTINGS_ENVIRONMENT_NAME \
    -e APPSETTINGS_ENVIRONMENT_SECURESITE \
    -e APPSETTINGS_ENVIRONMENT_REALM \
    -e INDEV_APIKEY \
    -e INDEV_BASEPATH \
    -e INDEV_CHAPTER \
    -e INDEV_DETAIL \
    -e INDEV_LIST \
    -e GILLIAM_CLIENT_CERTIFICATE_BASE64 \
    -e GILLIAM_BASE_PATH \
    -e GILLIAM_GET_CLAIMS_URL \
    -e GILLIAM_REALM \
    comments
```

Then browser to http://localhost:8080 on your host machine.

#### Stopping the container

```sh
docker stop comments
```

### Troubleshooting and gotchas

Solutions to common gotchas.

#### Error starting userland proxy

If you get errors like "Cannot start service...Error starting userland proxy: mkdir", try disabling Experimental features in Docker for Windows. See https://github.com/docker/for-win/issues/573#issuecomment-288940904

#### bad interpreter: No such file or directory

If you get errors like "/bin/bash^M: bad interpreter: No such file or directory" or "exited with code 126" then check that your .sh files use LF (and not CRLF) line endings

#### Unknown locator strategy id

If you get errors like "Unknown locator strategy id" in Firefox, using ID's as selectors, see https://github.com/aerokube/selenoid/issues/261. Firefox (Gecko driver) doesn't support IDs and requires CSS selectors, so you can get round it with "body #id".

#### Drive has not been shared

If you get the following error:

ERROR: for tests  Cannot create container for service tests: b'Drive has not been shared'

Navigate to docker settings and in the "Shared Drives" section tick the C drive option and click Apply
