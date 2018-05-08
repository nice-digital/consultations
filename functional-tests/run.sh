# Runs functional tests via Docker

# Avoid "Mount denied" errors for Chrome/Firefox containers on Windows
# See https://github.com/docker/for-win/issues/1829#issuecomment-376328022
export COMPOSE_CONVERT_WINDOWS_PATHS=1

docker-compose down --remove-orphans && docker-compose rm -vf
docker-compose build && docker-compose up -d
docker-compose run tests npm run test:teamcity -- --host hub -b http://comments:8081/consultations/ || exit $?
docker cp functionaltests_tests_run_1:/tests/errorShots ./errorShots_docker
# TODO: Copy serilog logs out of comments container:
#docker cp functionaltests_comments_run_1:/app/logs ./logs_docker
docker-compose down