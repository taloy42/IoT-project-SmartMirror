# IoT-project-SmartMirror

## Build local image
Run this from mirror-ui directory.

For WS:

`docker build -f .\Dockerfile.ws . -t local:ws `

For APP:

`docker build -f .\Dockerfile.app . -t local:app `

## Testing

To test the app we use docker-compose.

change the *docker-compose.yml* ***image*** property for each envoriment

Example for ws:

For local use (Not dev of main branch):

`#image: local:ws #Local TEST`

For dev use:

`image: isarn/mirror-iot-ws-dev:latest #DEV`

For prod use:

`#image: isarn/mirror-iot-ws-main:latest #PROD`

Example for app:

For local use (Not dev of main branch):

`#image: local:app #Local TEST`

For dev use:

`image: isarn/mirror-iot-app-dev:latest #DEV`

For prod use:

`#image: isarn/mirror-iot-app-main:latest #PROD`

finnaly run 
`docker compose up`
