import unittest
import json
import random_based_camera_emulator
import camera_system_facade.classes.PerSecondStats
import datetime

CAMERA_INFO_LOCATION = "../camera_information.json"


class RandomBasedCameraEmulatorTests(unittest.TestCase):
    def test_get_camera_id(self):
        camera_number = 1
        with open(CAMERA_INFO_LOCATION, "rb") as camera_info_file:
            json_data = json.load(camera_info_file)
            in_bound_camera_number = len(json_data) - 1
            out_bound_camera_number = len(json_data) + 1
            self.assertEqual(json_data[in_bound_camera_number]["id"], random_based_camera_emulator.get_camera_id(in_bound_camera_number, camera_info_location=CAMERA_INFO_LOCATION))
            self.assertRaises(IndexError, random_based_camera_emulator.get_camera_id, out_bound_camera_number, CAMERA_INFO_LOCATION)

    def test_generate_random_per_second_stat(self):
        previous_people_count = 2000
        today_datetime = datetime.datetime.now()
        camera_id = 1
        seed_per_second_stat = camera_system_facade.classes.PerSecondStats(camera_id, today_datetime, previous_people_count, False)
        generated_per_second_stat = random_based_camera_emulator.generate_random_per_second_stat(camera_id, seed_per_second_stat)
        self.assertEqual(generated_per_second_stat.camera_id, seed_per_second_stat.camera_id)
        self.assertEqual(generated_per_second_stat.has_saved_image, seed_per_second_stat.has_saved_image)
        self.assertTrue((generated_per_second_stat.num_tracked_people < seed_per_second_stat.num_tracked_people + 5)
                        and (generated_per_second_stat.num_tracked_people > seed_per_second_stat.num_tracked_people - 5))

    def test_get_preset_camera_token(self):
        with open(CAMERA_INFO_LOCATION, "rb") as camera_info_file:
            json_data = json.load(camera_info_file)
            in_bound_camera_number = len(json_data) - 1
            out_bound_camera_number = len(json_data) + 1
            self.assertEqual(json_data[in_bound_camera_number]["token"], random_based_camera_emulator.get_preset_camera_token(in_bound_camera_number, camera_info_location=CAMERA_INFO_LOCATION))
            self.assertRaises(IndexError, random_based_camera_emulator.get_preset_camera_token, out_bound_camera_number, CAMERA_INFO_LOCATION)


if __name__ == '__main__':
    unittest.main()
