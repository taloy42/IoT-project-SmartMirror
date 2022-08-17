import asyncio
import websockets

async def handler(websocket):
    while 1:
        await websocket.send("I'm Docker")


start_server = websockets.serve(handler, "0.0.0.0", 8000)
asyncio.get_event_loop().run_until_complete(start_server)
print("server was started")
asyncio.get_event_loop().run_forever()
