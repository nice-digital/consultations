FROM redis:alpine3.15

COPY ./redis.conf /usr/local/etc/redis/redis.conf

WORKDIR /data

CMD ["redis-server", "/usr/local/etc/redis/redis.conf"]
