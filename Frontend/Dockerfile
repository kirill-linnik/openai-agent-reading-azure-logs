FROM node:22-alpine AS build-stage
WORKDIR /app
COPY package.json /app/
RUN yarn
COPY ./ /app/ 
RUN yarn start:local

FROM nginx:1.27-alpine-slim
COPY --from=build-stage /app/build/ /usr/share/nginx/html
COPY build-configuration/nginx.local.conf /etc/nginx/nginx.conf
RUN rm /etc/nginx/conf.d/default.conf

EXPOSE 80