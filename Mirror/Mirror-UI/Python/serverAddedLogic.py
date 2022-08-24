import asyncio
import websockets
import json
from time import sleep
import cv2
import requests


face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
# current_user = "nobody"
unidentified_frames = 0
# current_status = "nobody"
nobody_id = "00000000-0000-0000-0000-000000000000"
detected_user = nobody_id
detected_username = 'nobody'


async def get_user_from_img(websocket, mirror_id: str, img):
    cv2.imwrite('./tempsaved.jpg', img)
    formatted_image = open('./tempsaved.jpg', 'rb')
    global detected_user
    global unidentified_frames
    global detected_username
    url = "https://tryincdecazurefunction20220420002219.azurewebsites.net/api/TryToMatchDetails?personGroupId=" + mirror_id
    response = requests.post(url, files={"form_field_name": formatted_image})
    if response.ok:
        print("response is: " + response.text)

    else:
        print("response was not ok")
        print(response)
    try:
        detected_user = json.loads(response.text)['personId']
        print("just after setting detected user as: ",detected_user)
        

        detected_username = json.loads(response.text)['userData']['firstName']
    except Exception as e:
        print(e)
        return -1
    if detected_user == nobody_id:   # make sure that this is what azure sen#ds when it doesnt recognize anyone
        unidentified_frames += 1
        print("unidtentified_frames is now: ",unidentified_frames)
    else:
        unidentified_frames = 0
        x = json.dumps({'data': detected_user, 'name': detected_username})
        await websocket.send(f'{x}')
    return response.text if response.ok else None



async def hello(websocket):
    print("began hello function")
    await websocket.recv()

    mirror_id = "trypersongroupid"
    global detected_user
    global detected_username
    global unidentified_frames
    cap = cv2.VideoCapture(0)
    while True:
        if websocket is None:
            print("WEBSOCKET WAS NONE!!!!!!!!")
            return
        sleep(0.6)
        _, img = cap.read()
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, 1.1, 4)
        if len(faces) or unidentified_frames > 0:
            if detected_user == nobody_id:
                if unidentified_frames < 2:
                    x = json.dumps({'data': 'face_detected', 'name': ''})
                    await websocket.send(f'{x}')
                    print("just after sending:", x)

                    print("just sent face_detected")
                    await get_user_from_img(websocket, mirror_id, img)
                else:
                    print("The person is unidentified... telling the frontend to display 'failed to recoginze. please register'")
                    x = json.dumps({'data': 'unrecognized', 'name': ''})
                    print(x)
                    await websocket.send(f'{x}')


                    unidentified_frames = 0
                    sleep(2)
        else:
            # print("nobody is here")
            if not detected_user == nobody_id:
                flag = False
                for i in range (8):
                    _, img = cap.read()
                    sleep(0.5)
                    gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
                    faces = face_cascade.detectMultiScale(gray, 1.2, 4)
                    if len(faces):
                        flag = True
                        break
                if flag:
                    await get_user_from_img(websocket, mirror_id, img)
                    continue
                else:
                    x = json.dumps({'data': nobody_id, 'name': ''})
            
                    await websocket.send(f'{x}')





                    print("just after sending nobody's id")
                    detected_user = nobody_id
                    detected_username="nobody"

        print("the detected user is: ", detected_user)
        k = cv2.waitKey(30) & 0Xff
        if k == 27:
            break
    cap.release()


async def main():
    async with websockets.serve(hello, "localhost", 8000):
        print("after serve function")
        await asyncio.Future()



if __name__ == "__main__":
    asyncio.run(main())

