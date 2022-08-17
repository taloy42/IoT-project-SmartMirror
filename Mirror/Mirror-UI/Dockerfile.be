FROM node:18.7-alpine

WORKDIR /backend
ENV PATH /backend/node_modules/.bin:$PATH

COPY ./App .
RUN npm install

EXPOSE 3001

CMD ["/usr/local/bin/npm", "start"]
