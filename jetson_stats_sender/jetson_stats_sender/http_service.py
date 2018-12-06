import urllib2
import json
from threading import Timer
from threading import Lock
import cv2
import base64
import logging

ENDPOINT_PATH = "/api/datareceival/datamessage"
global_http_service = None


def initialize_global_stats_sender(api_key, destination_address, camera_key=None):
    global global_http_service
    global_http_service = HttpService(api_key, destination_address, camera_key)
    logging.info("jetson_stats_sender initialized with api_key: '%s', destination_address '%s', camera_key '%s'" %
                 (api_key, destination_address, camera_key,))


def initialize_global_camera_key(camera_key):
    global global_http_service
    global_http_service.set_camera_key(camera_key)
    logging.info("Set camera_key to '%s'" % (camera_key,))


def send_per_second_stats_with_global_service(per_second_stats):
    global global_http_service
    global_http_service.send_per_second_stats(per_second_stats)


def send_per_second_stat_with_global_service(per_second_stat):
    global global_http_service
    global_http_service.send_per_second_stat(per_second_stat)


def queue_per_second_stat_for_sending_with_global_service(per_second_stat):
    if per_second_stat.num_tracked_people is not None:
        global global_http_service
        global_http_service.queue_per_second_stat_for_sending(per_second_stat)
        logging.debug("Queued following stat for sending '%s'" % (per_second_stat.to_json(),))


def get_global_http_service():
    global global_http_service
    return global_http_service


class HttpService:
    def __init__(self, api_key, destination_address, camera_key=None,
                 reset_max_stat_delay=3600, send_batch_delay_sec=10):
        self.api_key = api_key
        self.destination_address = destination_address
        self.camera_key = camera_key
        self.active_alerts_lock = Lock()
        self.batch_lock = Lock()
        self.active_alerts = []
        self.per_second_stats_batch = []
        self.send_batch_delay_sec = send_batch_delay_sec
        self._initialize_scheduler()

    def send_batch(self):
        if len(self.per_second_stats_batch) > 0:
            with self.batch_lock:
                for stat in self.per_second_stats_batch:
                    if stat.has_saved_image == 1:
                        self.encode_frm(stat)
                self.send_per_second_stats(self.per_second_stats_batch)
                self.per_second_stats_batch = []
        Timer(self.send_batch_delay_sec, self.send_batch, ()).start()

    def encode_frm(self, per_second_stat):
        try:
            retval, buf = cv2.imencode('.jpg', per_second_stat.frm)
            if retval:
                per_second_stat.frm_jpg = base64.b64encode(buf)
            else:
                logging.error("Couldn't convert video frame to image for stat: %s" % per_second_stat.to_json())
        except:
            logging.error("Couldn't convert video frame to image for stat: %s" % per_second_stat.to_json())

    def _initialize_scheduler(self):
        Timer(self.send_batch_delay_sec, self.send_batch, ()).start()

    def set_camera_key(self, camera_key):
        self.camera_key = camera_key

    def queue_per_second_stat_for_sending(self, per_second_stat):
        with self.batch_lock:
            if self.does_stat_trigger_alerts(per_second_stat):
                # Indicate that stat's frame should be converted to an image
                logging.info("Stat: %s triggers alert, thus associating image with stat" % per_second_stat.to_json())
                per_second_stat.has_saved_image = 1
            self.per_second_stats_batch.append(per_second_stat)

    def does_stat_trigger_alerts(self, per_second_stat):
        stat_triggers_alert = False
        with self.active_alerts_lock:
            for alert in self.active_alerts:
                if alert["needsImage"]:
                    if alert["isMoreThanTrigger"]:
                        if per_second_stat.num_tracked_people > alert["triggerNumber"]:
                            alert["needsImage"] = False
                            stat_triggers_alert = True
                    else:
                        if per_second_stat.num_tracked_people < alert["triggerNumber"]:
                            alert["needsImage"] = False
                            stat_triggers_alert = True
        return stat_triggers_alert

    # Real implementation of the function that will send the passed in per_second_stat objects to the back-end
    # Sends multiple stats
    def send_per_second_stats(self, per_second_stats):
        url = self.get_url_for_per_second_stats_post()
        headers = self.get_headers_for_per_second_stats_post()
        json_encoded_data = self.get_json_encoded_data_for_per_second_stats(per_second_stats)
        logging.info("Sending %d stats to '%s'" % (len(per_second_stats), url,))
        try:
            req = urllib2.Request(url=url, data=json_encoded_data, headers=headers)
            response = urllib2.urlopen(req)
            contents = response.read()
            json_response = json.loads(contents)["value"]
            if (200 <= response.getcode() < 300) and json_response["numberOfReceivedStats"] == len(per_second_stats):
                if json_response["activeAlertsForCamera"] is not None:
                    logging.info("Updating active alert list with %d alerts" % len(json_response["activeAlertsForCamera"]))
                    with self.active_alerts_lock:
                        self.active_alerts = json_response["activeAlertsForCamera"]
                return True
            else:
                return False
        except:
            return False

    def send_per_second_stat(self, per_second_stat):
        return self.send_per_second_stats([per_second_stat, ])

    def get_url_for_per_second_stats_post(self):
        return self.destination_address + ENDPOINT_PATH

    def get_json_encoded_data_for_per_second_stats(self, per_second_stats):
        json_stats = []
        for stat in per_second_stats:
            if stat.camera_key is None or stat.camera_key == "":
                stat.camera_key = self.camera_key
            json_stats.append(stat.to_json())
        json_data = {
            "api_key": self.api_key,
            "RealTimeStats": json_stats
        }
        json_encoded_data = json.dumps(json_data)
        return json_encoded_data

    @staticmethod
    def get_headers_for_per_second_stats_post():
        return {'Content-type': 'application/json'}
