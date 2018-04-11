import urllib
import urllib2
import json

ENDPOINT_HOST = "http://localhost"
ENDPOINT_PORT = "5000"
ENDPOINT_PATH = "/api/datareceival/datamessage"


# Real implementation of the function that will send the passed in per_second_stat objects to the back-end
# We haven't decided yet if we'll send multiple per_second_stat objects at a time or just one,
# but this implementation supports multiple stats
def send_per_second_stats(camera_id, token, per_second_stats):
    url = get_url_for_per_second_stats_post()
    headers = get_headers_for_per_second_stats_post()
    json_encoded_data = get_json_encoded_data_for_per_second_stats(per_second_stats)
    try:
        req = urllib2.Request(url=url, data=json_encoded_data, headers=headers)
        response = urllib2.urlopen(req)
        print "Server response to sending stats for camera %s: " % camera_id + response.read()
    except:
        print "send_per_second_stat with camera_id: %s failed" % camera_id


def get_url_for_per_second_stats_post():
    return ENDPOINT_HOST + ":" + ENDPOINT_PORT + ENDPOINT_PATH


def get_headers_for_per_second_stats_post():
    return {'Content-type': 'application/json'}


def get_json_encoded_data_for_per_second_stats(per_second_stats):
    json_data = {
        # "CameraId": camera_id,
        # "Token": token,
        "RealTimeStats": per_second_stats
    }
    json_encoded_data = json.dumps(json_data)
    return json_encoded_data
