# Consultations functional tests

> Webdriver.io powered browser-based automated functional tests for the Consultation Comments project.

Loosley based on the [WebdriverIO Cucumber Boilerplate](https://github.com/webdriverio/cucumber-boilerplate).

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

**Table of Contents** _generated with [DocToc](https://github.com/thlorenz/doctoc)_

- [Required software](#required-software)
- [Running locally](#running-locally)
  - [Excluding tests](#excluding-tests)
  - [Run a single test](#run-a-single-test)
- [Docker](#docker)
  - [Publish](#publish)
  - [Environment variables](#environment-variables)
  - [Docker Compose](#docker-compose)
  - [Developing in Docker](#developing-in-docker)
  - [Docker single container](#docker-single-container)
    - [Build the image](#build-the-image)
    - [Running the image](#running-the-image)
    - [Stopping the container](#stopping-the-container)
  - [Troubleshooting and gotchas](#troubleshooting-and-gotchas)
    - [Error starting userland proxy](#error-starting-userland-proxy)
    - [bad interpreter: No such file or directory](#bad-interpreter-no-such-file-or-directory)
    - [Unknown locator strategy id](#unknown-locator-strategy-id)
    - [Drive has not been shared](#drive-has-not-been-shared)
    - [error while creating mount source path](#error-while-creating-mount-source-path)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Required software

- Node >= 18.19.0 and npm >= 10.2.3
- Chrome
- Firefox, optional
- Edge, optional
- Docker for windows (latest)

## Running locally

When running locally, the tests will run in Chrome, Firefox and Edge (You will need all these browser installed on you machine). However, if you want to just run on one browser you can comment out the browser capabilities for the other browsers in the [wdio.local.conf.js](wdio.local.conf.js).

Chromedriver is no longer needed as of Wdio V8. This is because WebdriverIO now handles this by managing the [Driver Binaries](https://webdriver.io/docs/driverbinaries/#:~:text=With%20WebdriverIO%20v8.,WebdriverIO%20will%20do%20the%20rest.).

Once you've installed Node:

1. Open a terminal of your choice.
2. You need [publish the webapp](#publish) and set some [environment variables](#environment-variables).
3. `cd` into the _functional-tests_ directory: `cd functional-tests`
4. install package dependencies from npm by running `npm i`. You only need to do this the first time you run the test: you can skip this step on subsequent sessions.
5. run the app in docker but running `./docker-dev.sh`. This will exec out on the test folder once everything is set up.
6. run ctrl + d and you will appear back on the terminal but the application is now running on your machine ready to test

Then run the tests by running the following:

```sh
npm run test-local
```

This automatically initiates your framework (e.g. Mocha, Jasmine or Cucumber) within worker process and runs all your test files within your Node.js environment (via [webdriver.io local runner](https://webdriver.io/docs/runner/#local-runner)) against Chrome, firefox and Microsoft Edge.
This means you can see the tests running on your local machine and watch them run on the browsers as they are run

### Excluding/Including tests

You can add and remove tests from the spec section of the capabilities of each browser in the [wdio.local.conf.js](wdio.local.conf.js) file. Chrome is set up to run all as default while firefox and Edge have a reduced number of tests. If you see the top level specs section you will find a definitive list of all individual tests commented out. You can copy these and place them into the relevent spec section of the browser you would like them to run on.

## Docker

Running tests on Docker is a good option as it means you don't need browsers installed on the host machine, and the Selenium grid is automatically created for you. This is useful on a TeamCity build agent where you can't rely on Chrome and Firefox being installed.

We recommend running the tests via [Docker Compose](#docker-compose) which handles everything for you.

First you need [publish the webapp](#publish) and set some [environment variables](#environment-variables).

### Publish

The docker images expect the dotnet core app to have already been published into the _published-app_ folder. This is what happens on TeamCity. To run locally, first build and publish the dotnet core webapp by using the following command:

```sh
dotnet publish -c Release -o published-app /property:PublishWithAspNetCoreTargetManifest="false"
```

### Environment variables

When we deploy the webapp with Octo we use variable substitution to change settings in [appsettings.json](Comments/appsettings.json). When we're running via Docker, we don't have this luxury. So we have to use environment variables when we run the container(s) to pass in these settings. Obtain all the current environment variables from another team member, then add them to your terminal by using either export for bash, or set for cmd, e.g.

```sh
export DEFAULT_CONNECTION="Beta"
```

or

```sh
set ACCOUNTS_ENVIRONMENT=Beta
```

> Note: you can get the values from your local User Secrets in Visual Studio.

### Docker Compose

We use [Docker Compose](https://docs.docker.com/compose/) to create all of the containers needed to run the application and tests. It consists of 10 containers:

- _[Comment Collection web application](../Comments/Dockerfile)_ - the dotnet core and React application
- _database_ SQL Server database
- _api_ Fake InDev feed backend
- _[The tests themselves](Dockerfile)_
- _SeleniumHub_ - See https://github.com/SeleniumHQ/docker-selenium#via-docker-compose
- _chrome_ - Grid Node with Chrome installed
- _firefox_ - Grid Node with Firefox installed
- _edge_ - Grid Node with Microsoft Edge installed
- Nginx container as a reverse proxy to handle the SSL encryption/decryption
- _redis_ REdis container

The following command create a network, builds images and runs all the containers, then runs the tests against Chrome, Firefox and Edge, via the selenium grid:

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
npm run wait-then-test
```

Now, if you make a change to the tests locally, you have a way of testing the change within docker giving you more confidence that it will run in the teamcity build agent when you push it up.

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

ERROR: for tests Cannot create container for service tests: b'Drive has not been shared'

Navigate to docker settings and in the "Shared Drives" section tick the C drive option and click Apply

#### error while creating mount source path

If when spinning up the containers in dev mode you get the following error:

ERROR: for functional-tests_tests_1 Cannot start service tests: b"error while creating mount source path '/host_mnt/c/src/consultations/functional-tests': mkdir /host_mnt/c: file exists"

This means that the mounted volume has gone stale and you need to reset it. This can be done with a few quick commands.

First remove the offending container(in this case the testing container):

```sh
docker rm -vf functional-tests_tests_1
```

Make sure that the volumes are no longer hanging around:

```sh
docker volume ls
```

Then run the following command to remove any offending volume:

```sh
docker volume rm -f <volume>
```

At this point you will need to restart docker. Once Docker is restarted then run the "./docker-dev.sh" script and you should no longer see the error and be able to continue.

#### Debug dotnet app

docker cp functional-tests_comments_1:/app/logs ./docker-output

#### Connect to SQL server in docker

when the docker containers are up and running you can connect to the db using sql server management studio
