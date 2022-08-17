FROM balenalib/raspberry-pi-alpine-node

LABEL Name="Test"
LABEL version="1"

WORKDIR /app
ENV PATH /app/node_modules/.bin:$PATH

COPY ./App .
RUN npm install
WORKDIR /app/frontend
RUN npm install 
WORKDIR /app

COPY entrypoint.sh .
RUN dos2unix entrypoint.sh

EXPOSE 3000

CMD ["/bin/sh", "./entrypoint.sh"]
