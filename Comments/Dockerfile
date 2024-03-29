# Runs the Comment Collection webapp.
# Assumes you've published the app to ./published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0

ENV ASPNETCORE_URLS http://+:8080

# Install Git
RUN apt-get update && apt-get install git -y
RUN git --version

# Install curl
RUN apt-get update \
  && apt-get install -y curl


# Install node and npm
# Latest Nodejs LTS as of 03/05/2018
ENV NODE_VERSION 18.17.0
ENV NODE_DOWNLOAD_SHA 5c4a7fd9262c0c47bafab3442de6c3fed1602be3d243cb8cf11309a201955e75
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
  && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
  && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
  && rm nodejs.tar.gz \
  && ln -s /usr/local/bin/node /usr/local/bin/nodejs
RUN node -v
RUN npm -v

# Install replace globally for index.html replacement
RUN npm i -g replace

# Install jq for JSON file replacement and nc for checking sql port is available
RUN apt-get update && apt-get install jq -y \
  && rm -rf /var/lib/apt/lists/*

# Download wait-for-it as binary waitforit, see  https://github.com/vishnubob/wait-for-it
ENV WAITFORIT_VERSION db04971
ENV WAITFORIT_DOWNLOAD_SHA dd7511b88c5eaf3f8953167ab498f66589143649142a9a2f49a8f33e08584aaf
ENV WAITFORIT_DOWNLOAD_URL https://github.com/vishnubob/wait-for-it/archive/$WAITFORIT_VERSION.tar.gz
RUN curl -SL "$WAITFORIT_DOWNLOAD_URL" --output waitforit.tar.gz \
  && echo "$WAITFORIT_DOWNLOAD_SHA waitforit.tar.gz" | sha256sum -c - \
  && mkdir -p /usr/local/waitforit \
  && tar -xzf "waitforit.tar.gz" -C /usr/local/waitforit --strip-components=1 \
  && rm waitforit.tar.gz \
  && mv /usr/local/waitforit/wait-for-it.sh /usr/local/bin/waitforit \
  && chmod +x /usr/local/bin/waitforit \
  && rm -rf /usr/local/waitforit

# Install frontend packages with node
COPY ./published-app/ClientApp/package.json ./app/ClientApp/
# COPY ./wwwroot/Media/Default/html/HtmlWidget/Footer.html ./app/wwwroot/Media/Default/html/HtmlWidget/
#COPY ./published-app/ClientApp/package-lock.json ./app/ClientApp/
WORKDIR /app/ClientApp
RUN npm i --production

# Copy .net app to a location on container and run application
WORKDIR /app

# Script for running the dotnet app and setting appsettings.json values
COPY ./run.sh ./
RUN chmod +x ./run.sh

# Assume dotner core app's already been published...
COPY ./published-app ./

EXPOSE 8080

# Wait for SQL Server to be available before running the dotnet app
# ENTRYPOINT ["--", "./run.sh"]
ENTRYPOINT ["waitforit", "-t", "60", "--strict", "database:1433", "--", "./run.sh"]
