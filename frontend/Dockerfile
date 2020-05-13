
FROM node:13.13-alpine3.10

WORKDIR /usr/app

COPY ./frontend/package*.json ./

RUN npm install

COPY . .
RUN npm run build
COPY ./frontend/.env ./

EXPOSE 5000

CMD ["npm", "start"]