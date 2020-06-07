
FROM node:13.13-alpine3.10

WORKDIR /usr/app
COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

CMD ["npm", "start"]