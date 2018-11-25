from jetson_stats_sender import PerSecondStats
from jetson_stats_sender import http_service
import datetime
import time
import random
import pytz

class CameraEmulator:
    def __init__(self, camera_key, api_key, destination_address, delay_between_stats_sec):
        self.http_service = http_service.HttpService(api_key, destination_address, camera_key)
        self.camera_key = camera_key
        self.delay_between_stats_sec = delay_between_stats_sec
        self.stats_sent = 0
        self.stats_acked = 0
        self.stopped = False

    def get_error_rate(self):
        return (self.stats_sent - self.stats_acked) / float(self.stats_sent) if self.stats_sent != 0 else 0.0

    def start(self):
        generated_random_per_second_stat = PerSecondStats(datetime.datetime.now(tz=pytz.utc), 0, self.camera_key)
        self.send_per_second_stat(generated_random_per_second_stat)
        while not self.stopped:
            time.sleep(self.delay_between_stats_sec)
            generated_random_per_second_stat = generate_random_per_second_stat(generated_random_per_second_stat)
            self.send_per_second_stat(generated_random_per_second_stat)

    def stop(self):
        self.stopped = True

    def send_per_second_stat(self, per_second_stat):
        self.stats_sent = self.stats_sent + 1
        if self.http_service.send_per_second_stat(per_second_stat):
            self.stats_acked = self.stats_acked + 1


# We take the previous per_second_stat and use it to calculate the next one.
# We do this by adding or substracting a random but reasonable amount from the previous people_count
def generate_random_per_second_stat(previous_per_second_stat):
    previous_people_count = previous_per_second_stat.num_tracked_people
    people_count_variation = random.choice((-4, -3, -2, -2, -2, -1, -1, -1, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 3, 4, 5))
    new_people_count = previous_people_count + people_count_variation
    if new_people_count < 0:
        new_people_count = 0
    return PerSecondStats(datetime.datetime.now(tz=pytz.utc), new_people_count, previous_per_second_stat.camera_key)
