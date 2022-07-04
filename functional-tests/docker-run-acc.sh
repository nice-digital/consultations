#!/bin/bash

# Runs functional tests via Docker

# Avoid "Mount denied" errors for Chrome/Firefox containers on Windows
# See https://github.com/docker/for-win/issues/1829#issuecomment-376328022
export COMPOSE_CONVERT_WINDOWS_PATHS=1

  # Clean up before we start
  rm -rf docker-output && rm -rf allure-results && rm -rf allure-report
  docker exec functional-tests_database_1 kill 1 || :
  docker-compose down --remove-orphans && docker-compose rm -vf

  docker-compose build --no-cache && docker-compose up -d

  # Wait for the web app to be up before running the tests
  docker-compose run -T tests npm run wait-then-accessibility-test
  # Or for dev mode, uncomment:
  # winpty docker-compose exec tests bash

  # Generate an Allure test report
  docker-compose run -T tests allure generate --clean

  mkdir -p docker-output
  docker cp functional-tests_tests_1:/tests/errorShots ./docker-output/errorShots
  docker cp functional-tests_comments_1:/app/logs ./docker-output
  docker cp functional-tests_tests_1:/tests/allure-report ./docker-output
  docker-compose logs --no-color > ./docker-output/logs.txt

  # Stop in the background so the script finishes quicker - we don't need to wait
  nohup docker-compose down --remove-orphans --volumes > /dev/null 2>&1 &
  docker volume ls
  docker-compose down -v
  docker volume ls


# function exitWithCode()
#   {
#     echo "exit code is: $1"
#     if [ "$1" -gt 0 ]
#     then
#       exit 1
#     else
#       exit 0
#     fi
#   }

# error=0
# trap 'catch' ERR
# catch() {
#   error=1
# }

# cleanupBeforeStart
# runTests
# processTestOutput
# cleanup
# exitWithCode $error
# # Runs functional tests via Docker

# # Avoid "Mount denied" errors for Chrome/Firefox containers on Windows
# # See https://github.com/docker/for-win/issues/1829#issuecomment-376328022
# export COMPOSE_CONVERT_WINDOWS_PATHS=1

# # Clean up before we start
# rm -rf docker-output && rm -rf allure-results && rm -rf allure-report

# # Clean up before starting containers
# docker exec functional-tests_database_1 kill 1 || :
# docker-compose down --remove-orphans && docker-compose rm -vf
# docker-compose build && docker-compose up -d

# # Wait for comments webapp before running the tests
# docker-compose run tests waitforit -t 120 --strict comments:8080 -- npm run test:teamcity -- --host hub -b https://niceorg/consultations/

# # Generate an Allure test report
# docker-compose run -T tests allure generate --clean

# # Copy error shots and logs to use as a TeamCity artifact for debugging purposes
# mkdir -p docker-output
# docker cp functional-tests_tests_1:/tests/errorShots ./docker-output/errorShots
# docker cp functional-tests_comments_1:/app/logs ./docker-output
# docker cp functional-tests_tests_1:/tests/allure-report ./docker-output
# docker-compose logs --no-color > ./docker-output/logs.txt

# # Clean up
# docker volume ls
# docker-compose down -v
# docker volume ls
# #docker volume prune -f
