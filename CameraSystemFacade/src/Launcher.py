# Run this file to start the facade which emulates the camera system by sending hard coded camera data

import json

jsonImageData = [
    {
        'timeStamp': '2018-09-06 10:51:00 EST',
        'personCount': 3,
        'location': 'AMV',
        'cameraName': 'East Camera',
        'cameraId': '11111'
    },
    {
        'timeStamp': '2018-09-06 10:51:15 EST',
        'personCount': 4,
        'location': 'AMV',
        'cameraName': 'East Camera',
        'cameraId': '11111'
    },
    {
        'timeStamp': '2018-09-06 10:51:30 EST',
        'personCount': 5,
        'location': 'AMV',
        'cameraName': 'East Camera',
        'cameraId': '11111'
    },
]

print json.dumps(jsonImageData)
