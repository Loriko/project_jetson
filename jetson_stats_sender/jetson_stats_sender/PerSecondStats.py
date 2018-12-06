class PerSecondStats:
    def __init__(self, stat_time, num_tracked_people, camera_key=None, frm=None):
        self.camera_key = camera_key
        self.stat_time = stat_time
        self.num_tracked_people = num_tracked_people
        self.has_saved_image = 0
        self.frm = frm
        self.frm_jpg = None

    def to_json(self):
        if self.frm_jpg is not None:
            self.has_saved_image = 1
        return {
            "CameraKey": self.camera_key,
            "DateTime": self.stat_time.strftime("%Y-%m-%d %H:%M:%S %z"),
            "NumTrackedPeople": self.num_tracked_people,
            "HasSavedImage": self.has_saved_image,
            "FrameAsJpg": self.frm_jpg
        }
