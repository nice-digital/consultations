# Runs functional tests via Docker

# Avoid "Mount denied" errors for Chrome/Firefox containers on Windows
# See https://github.com/docker/for-win/issues/1829#issuecomment-376328022
export COMPOSE_CONVERT_WINDOWS_PATHS=1

# Clean up before starting containers
docker-compose down --remove-orphans && docker-compose rm -vf
docker-compose build && docker-compose up -d

# Wait for comments webapp before running the tests
docker-compose run tests ./wait-for-it.sh -t 120 --strict niceorg:8080 -- npm run test:teamcity -- --host hub -b http://niceorg:8080/consultations/

# Copy error shots and logs to use as a TeamCity artifact for debugging purposes
mkdir -p docker-output
docker cp functional-tests_tests_run_1:/tests/errorShots ./docker-output/errorShots
docker-compose logs > ./docker-output/logs.txt

# Clean up
docker-compose down