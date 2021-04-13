FROM node:15

ENV NODE_ENV=production

RUN npm i --g typescript

WORKDIR /app

COPY ["package.json", "package-lock.json*", "./"]

RUN npm i --production=false

COPY . .

RUN npm run build

CMD [ "node", "dist/app.js" ]