FROM node:14.18-buster-slim

WORKDIR /app

COPY package*.json ./

RUN npm ci

COPY . .

# Patch config at build time
RUN sed -i 's|require.resolve("https-browserify")|"https-browserify"|g' config-overrides.js

RUN npm run build

EXPOSE 5000

CMD ["npm", "run", "start:ssr"]