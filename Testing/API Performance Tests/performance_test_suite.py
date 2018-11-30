import unittest
from performance_tests_support import run_cameras_in_parallel
from pyunitreport import HTMLTestRunner

ACCEPTED_ERROR_RATE_THRESHOLD = 0.03


class RandomBasedCameraEmulatorTests(unittest.TestCase):
    def test_1_camera_100ms_delay(self):
        cameras = run_cameras_in_parallel(1, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_5_cameras_in_parallel_100ms_delay(self):
        cameras = run_cameras_in_parallel(5, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_10_cameras_in_parallel_100ms_delay(self):
        cameras = run_cameras_in_parallel(10, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_15_cameras_in_parallel_100ms_delay(self):
        cameras = run_cameras_in_parallel(15, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_20_cameras_in_parallel_100ms_delay(self):
        cameras = run_cameras_in_parallel(20, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_25_cameras_in_parallel_100ms_delay(self):
        cameras = run_cameras_in_parallel(25, 0.1)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_1_camera_10ms_delay(self):
        cameras = run_cameras_in_parallel(1, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_5_cameras_in_parallel_10ms_delay(self):
        cameras = run_cameras_in_parallel(5, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_10_cameras_in_parallel_10ms_delay(self):
        cameras = run_cameras_in_parallel(10, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_15_cameras_in_parallel_10ms_delay(self):
        cameras = run_cameras_in_parallel(15, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_20_cameras_in_parallel_10ms_delay(self):
        cameras = run_cameras_in_parallel(20, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

    def test_25_cameras_in_parallel_10ms_delay(self):
        cameras = run_cameras_in_parallel(25, 0.01)
        for camera in cameras:
            self.assertLessEqual(camera.get_error_rate(), ACCEPTED_ERROR_RATE_THRESHOLD)

if __name__ == '__main__':
    unittest.main(testRunner=HTMLTestRunner(output=""))
