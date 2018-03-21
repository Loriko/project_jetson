from camera_system_facade.classes.PerSecondStats import PerSecondStats
import json
import time
import datetime
import random
from camera_system_facade import http_service


CAMERA_INFO_LOCATION = "camera_information.json"
DELAY_BETWEEN_MEASUREMENTS_IN_SECONDS = 2


def emulate_camera(camera_number):
    # First we need to get the camera's camera_id from our json file
    camera_id = get_camera_id(camera_number)
    print "Emulating camera with camera_id: %s" % camera_id
    # First we need to authenticate the camera to the back-end to get a token
    token = authenticate_camera(camera_id, camera_number)
    # Now we use this token to endlessly send made up data to the back-end
    serve_made_up_stats_to_server(camera_id, token)


def get_camera_id(camera_number):
    with open(CAMERA_INFO_LOCATION, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if camera_number < len(json_data):
            return json_data[camera_number]["id"]
        else:
            raise IndexError("Json file doesn't have information for camera # %d" % camera_number)


def serve_made_up_stats_to_server(camera_id, token):
    print "Now sending people count information from camera with id: %s" % camera_id
    today_datetime = datetime.datetime.now()
    generated_random_per_second_stat = PerSecondStats(camera_id, today_datetime.year, today_datetime.month, today_datetime.day, today_datetime.hour, today_datetime.minute, today_datetime.second, 0, False)
    send_one_per_second_stat(camera_id, token, generated_random_per_second_stat)
    while True:
        time.sleep(DELAY_BETWEEN_MEASUREMENTS_IN_SECONDS)
        generated_random_per_second_stat = generate_random_per_second_stat(camera_id, generated_random_per_second_stat)
        send_one_per_second_stat(camera_id, token, generated_random_per_second_stat)


def send_one_per_second_stat(camera_id, token, per_second_stat):
    http_service.send_per_second_stat(camera_id, token, [per_second_stat.to_json(), ])


# We take the previous per_second_stat and use it to calculate the next one.
# We do this by adding or substracting a random but reasonable amount from the previous people_count
def generate_random_per_second_stat(camera_id, previous_per_second_stat):
    previous_people_count = previous_per_second_stat.num_tracked_people
    people_count_variation = random.choice((-4, -3, -2, -2, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 3, 4,))
    new_people_count = previous_people_count + people_count_variation
    if new_people_count < 0:
        new_people_count = 0
    today_datetime = datetime.datetime.now()
    generated_per_second_stat = PerSecondStats(camera_id, today_datetime.year, today_datetime.month, today_datetime.day, today_datetime.hour, today_datetime.minute, today_datetime.second, new_people_count, False)
    return generated_per_second_stat


def authenticate_camera(camera_id, camera_number):
    # get_preset_camera_token is a placeholder method which grabs the token from a json file
    # TODO: replace call to get_preset_camera with actual authentication mechanism. Requires work on the back-end
    return get_preset_camera_token(camera_number)


def get_preset_camera_token(camera_number):
    with open(CAMERA_INFO_LOCATION, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if camera_number < len(json_data):
            return json_data[camera_number]["token"]
        else:
            raise IndexError("Json file doesn't have information for camera # %d" % camera_number)
