FROM python:3.8-slim-buster

WORKDIR /ws

COPY ./Python .
RUN pip3 install websockets

EXPOSE 8000

CMD [ "python", "server.py" ]