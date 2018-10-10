import urllib2
import json

ENDPOINT_HOST = "http://localhost"
ENDPOINT_PORT = "5001"
ENDPOINT_PATH = "/api/datareceival/datamessage"


# Real implementation of the function that will send the passed in per_second_stat objects to the back-end
# We haven't decided yet if we'll send multiple per_second_stat objects at a time or just one,
# but this implementation supports multiple stats
def send_per_second_stats(per_second_stats):
    url = get_url_for_per_second_stats_post()
    headers = get_headers_for_per_second_stats_post()
    json_encoded_data = get_json_encoded_data_for_per_second_stats(get_api_key(), per_second_stats)
    try:
        req = urllib2.Request(url=url, data=json_encoded_data, headers=headers)
        response = urllib2.urlopen(req)
        print "Got server response after sending stat: " % response.read()
    except:
        print "send_per_second_stat failed"


def get_url_for_per_second_stats_post():
    return ENDPOINT_HOST + ":" + ENDPOINT_PORT + ENDPOINT_PATH


def get_headers_for_per_second_stats_post():
    return {'Content-type': 'application/json'}


def get_api_key():
    return "AFRJNILIJHRU"


def get_json_encoded_data_for_per_second_stats(api_key, per_second_stats):
    json_data = {
        "api_key": api_key,
        "RealTimeStats": per_second_stats
    }
    json_encoded_data = json.dumps(json_data)
    return json_encoded_data
