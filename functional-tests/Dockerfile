FROM node:8.11.1

RUN mkdir -p /tests

WORKDIR /tests

# Script for waiting for web app, see https://github.com/vishnubob/wait-for-it
COPY ./wait-for-it.sh ./
RUN chmod +x ./wait-for-it.sh

COPY package.json /tests
COPY package-lock.json /tests

RUN npm i

COPY . /tests