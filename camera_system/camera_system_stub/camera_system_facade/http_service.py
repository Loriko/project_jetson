import urllib
import urllib2
import json

ENDPOINT_HOST = "http://localhost"
ENDPOINT_PORT = "5000"
ENDPOINT_PATH = "/api/datareceival/datamessage"


# Real implementation of the function that will send the passed in per_second_stat objects to the back-end
# We haven't decided yet if we'll send multiple per_second_stat objects at a time or just one,
# but this implementation supports multiple stats
def send_per_second_stat(camera_id, token, per_second_stats):
    url = ENDPOINT_HOST + ":" + ENDPOINT_PORT + ENDPOINT_PATH
    headers = {'Content-type': 'application/json'}
    json_data = {
        # "CameraId": camera_id,
        # "Token": token,
        "RealTimeStats": per_second_stats
    }
    json_encoded_data = json.dumps(json_data)
    try:
        req = urllib2.Request(url=url, data=json_encoded_data, headers=headers)
        response = urllib2.urlopen(req)
        print "Server response to sending stats for camera %s: " % camera_id + response.read()
    except:
        print "send_per_second_stat with camera_id: %s failed" % camera_id
