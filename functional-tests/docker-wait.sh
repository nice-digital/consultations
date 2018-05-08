#!/bin/bash

# Run this script inside a Docker container to:
# - wait for the Comments webapp to exist
# - run the tests

i=1
# Wait for webapp to be available
# https://stackoverflow.com/a/21189440/486434
until $(curl --output /dev/null --silent --head --fail http://comments:8081/consultations/); do
    printf "Waiting for Comment Collection (%ss) web app…\n" "$i"
    sleep 1
    i=$((i + 1))
done

# Run the tests against the Selenium grid against the web app
npm run test:teamcity -- --host hub -b http://comments:8081/consultations/