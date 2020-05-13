
FROM node:13.13-alpine3.10

WORKDIR /usr/app

COPY ./frontend/package*.json ./

RUN npm install

COPY ./frontend/ .
RUN npm run build


EXPOSE 5000

CMD ["npm", "start"]