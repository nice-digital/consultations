# Runs the Comment Collection webapp.
# Assumes you've published the app to ./published-app

FROM microsoft/aspnetcore:2.0

ENV ASPNETCORE_URLS http://+:8081

# Install node and npm
# Latest Nodejs LTS as of 03/05/2018
ENV NODE_VERSION 8.11.1 
ENV NODE_DOWNLOAD_SHA 0e20787e2eda4cc31336d8327556ebc7417e8ee0a6ba0de96a09b0ec2b841f60
ENV NODE_DOWNLOAD_URL https://nodejs.org/dist/v$NODE_VERSION/node-v$NODE_VERSION-linux-x64.tar.gz

RUN curl -SL "$NODE_DOWNLOAD_URL" --output nodejs.tar.gz \
  && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
  && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
  && rm nodejs.tar.gz \
  && ln -s /usr/local/bin/node /usr/local/bin/nodejs
RUN node -v
RUN npm -v

# Install jq for JSON file replacement
RUN apt-get update && apt-get install -y jq \
  && rm -rf /var/lib/apt/lists/*

# Install frontend packages with node
COPY ./published-app/ClientApp/package.json ./app/ClientApp/
#COPY ./published-app/ClientApp/package-lock.json ./app/ClientApp/
WORKDIR /app/ClientApp
RUN npm i --production

# Copy .net app to a location on container and run application 
WORKDIR /app
COPY ./run.sh ./
COPY ./published-app ./

EXPOSE 8081

# Run .net app
ENTRYPOINT ["./run.sh"]