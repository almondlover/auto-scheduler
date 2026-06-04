FROM node:lts-alpine as build
WORKDIR /app
COPY package*.json ./

RUN npm install

COPY . .

RUN npx shadcn-vue@latest add -a

RUN npm run build-only
