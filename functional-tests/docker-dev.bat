@echo off

export "COMPOSE_CONVERT_WINDOWS_PATHS=1"
docker-compose "down" "--remove-orphans" && docker-compose "rm" "-vf"
docker-compose "build" && docker-compose "up" "-d" "--force-recreate"
winpty "docker-compose" "exec" "tests" "bash"
