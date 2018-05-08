# Runs functional tests via Docker

# Avoid "Mount denied" errors for Chrome/Firefox containers on Windows
# See https://github.com/docker/for-win/issues/1829#issuecomment-376328022
export COMPOSE_CONVERT_WINDOWS_PATHS=1

docker-compose down --remove-orphans && docker-compose rm -vf
docker-compose build && docker-compose up -d
docker-compose run tests npm run test:teamcity -- --host hub -b http://comments/consultations/ || exit $?
docker cp functionaltests_tests_run_1:/tests/errorShots ./errorShots_docker
docker-compose down