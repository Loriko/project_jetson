import camera_emulator
import time
from threading import Thread
import traceback
import json


CONFIG_LOCATION = "config.json"


def get_web_server_location(config_location=CONFIG_LOCATION):
    with open(config_location, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if "web_server_location" in json_data:
            return json_data["web_server_location"]
        else:
            raise IndexError("Json config file doesn't have a value for the 'web_server_location' key")


def get_api_key(config_location=CONFIG_LOCATION):
    with open(config_location, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if "api_key" in json_data:
            return json_data["api_key"]
        else:
            raise IndexError("Json config file doesn't have a value for the 'api_key' key")


def get_delay_between_stats_sec(config_location=CONFIG_LOCATION):
    with open(config_location, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if "delay_between_stats_sec" in json_data:
            return json_data["delay_between_stats_sec"]
        else:
            raise IndexError("Json config file doesn't have a value for the 'delay_between_stats_sec' key")


def get_camera_keys(config_location=CONFIG_LOCATION):
    with open(config_location, "rb") as camera_info_file:
        json_data = json.load(camera_info_file)
        if "camera_keys" in json_data:
            return json_data["camera_keys"]
        else:
            raise IndexError("Json config file doesn't have a value for the 'camera_keys' key")


def get_x_emulated_cameras(x, api_key, web_server_location, delay_between_stats_sec):
    emulated_cameras = []
    camera_keys = get_camera_keys()
    for i in range(x):
        emulated_cameras.append(camera_emulator.CameraEmulator(camera_keys[i], api_key,
                                                               web_server_location, delay_between_stats_sec))
    return emulated_cameras


def run_cameras_in_parallel(parallel_cameras_num, delay_between_stats_sec=get_delay_between_stats_sec(),
                            test_length_sec=30):
    api_key = get_api_key()
    web_server_location = get_web_server_location()

    emulated_cameras = get_x_emulated_cameras(parallel_cameras_num, api_key, web_server_location,
                                              delay_between_stats_sec)

    emulated_camera_threads = []
    for emulated_camera in emulated_cameras:
        emulated_camera_threads.append(Thread(target=emulated_camera.start))

    try:
        for thread in emulated_camera_threads:
            thread.start()

        time.sleep(test_length_sec)

        for emulated_camera in emulated_cameras:
            emulated_camera.stop()

        for thread in emulated_camera_threads:
            thread.join()
    except:
        # Exception occurred in one of the threads
        traceback.print_exc()

    # Wait for all emulators to have completed their last loop
    time.sleep(5)
    return emulated_cameras
