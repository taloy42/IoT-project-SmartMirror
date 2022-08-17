import asyncio
import websockets
import json
from time import sleep
import cv2
import requests


face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
detected_user = "nobody"

async def get_user_from_img(websocket, mirror_id: str, img):
    print("here1")
    # img=cv2.rotate(img,cv2.ROTATE_180)
    cv2.imwrite('./tempsaved.jpg', img)
    formatted_image = open('./tempsaved.jpg', 'rb')
    # print(type(newImg))
    global detected_user
    url = "https://tryincdecazurefunction20220420002219.azurewebsites.net/api/TryToMatchDetails?personGroupId=" + mirror_id
    response = requests.post(url, files={"form_field_name": formatted_image})
    if response.ok:
        print("response is: " + response.text)
    else:
        print("response was not ok")
    try:
        detected_user = json.loads(response.text)['name']
    except Exception as e:
        print(e)
        return -1
    await websocket.send(f'{detected_user}')
    print("just after sending ",detected_user)
    return response.text if response.ok else None



















async def hello(websocket):
    print("began hello function")
    # await websocket.send('yesterday')
    name = await websocket.recv()
    sleep(1)
    mirror_id = "trypersongroupid"
    global detected_user
    cap = cv2.VideoCapture(0)
    while True:
        _, img = cap.read()
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, 1.1, 4)
        # print("faces is now: ", faces)
        if len(faces):
            if detected_user == "nobody":
                await get_user_from_img(websocket, mirror_id, img)
        else:
            # print("nobody is here")
            if not detected_user == "nobody":
                flag = False
                for i in range (4):
                    _, img = cap.read()
                    sleep(0.5)
                    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
                    faces = face_cascade.detectMultiScale(gray, 1.2, 4)
                    if len(faces):
                        flag = True
                        break
                if flag:
                    continue
                else:
                    await websocket.send('nobody')
                    print("just after sending nobody")
                    detected_user = "nobody"
        for (x, y, w, h) in faces:
            cv2.rectangle(img, (x, y), (x + w, y + h), (255, 0, 0), 2)
        cv2.imshow('img', img)


        print("the detected user is: ", detected_user)
        k = cv2.waitKey(30) & 0Xff
        if k == 27:
            break
    cap.release()


async def try1(websocket):
    while 1:
        x=input("enter user:")
        await websocket.send(f'{x}')










    # name = await websocket.recv()
    # print(f"<<< {name}")

    # greeting = f"Hello {name}!"
    #
    # await websocket.send(greeting)
    # print(f">>> {greeting}")

start_server = websockets.serve(hello, "localhost", 8000)
asyncio.get_event_loop().run_until_complete(start_server)
print("server has started")
asyncio.get_event_loop().run_forever()











































# import asyncio
# import websockets
# import json
#
# # async def handler(websocket):
# #     while 1:
# #         x=""
# #         x = input("Enter a name: ")
# #         # x = '{ "username":',x,'}'
# #         # x = json.loads(x)
# #
# #
# #         await websocket.send(f'{x}')
# #
# #
# # start_server = websockets.serve(handler, "localhost", 8000)
# # # asyncio.get_event_loop().run_until_complete(start_server)
# # print("server was started")
# # # asyncio.get_event_loop().run_forever()
# # asyncio.run(handler())
# # print("the end")
#
#
# async def echo(websocket):
#     async for message in websocket:
#         await websocket.send(message)
#
# async def main():
#     async with websockets.serve(echo, "localhost", 8765):
#         await asyncio.Future()  # run forever
#
# asyncio.run(main())
#
#
#
# print("done")
