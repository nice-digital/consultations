# Runs functional tests via Docker

export COMPOSE_CONVERT_WINDOWS_PATHS=1

  # Clean up before we start
  rm -rf docker-output && rm -rf allure-results && rm -rf allure-report
  docker exec functional-tests_database_1 kill 1 || :
  docker-compose down --remove-orphans && docker-compose rm -vf

  docker-compose build --no-cache && docker-compose up -d

  # Wait for the web app to be up before running the tests
  docker-compose run -T tests npm run wait-then-accessibility-test

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
