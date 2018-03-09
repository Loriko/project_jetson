class PerSecondStats:
    def __init__(self, camera_id, year, month, day, hour, minute, second, num_tracked_people, has_saved_image):
        self.camera_id = camera_id
        self.year = year
        self.month = month
        self.day = day
        self.hour = hour
        self.minute = minute
        self.second = second
        self.num_tracked_people = num_tracked_people
        self.has_saved_image = has_saved_image

    def to_json(self):
        return {
            "CameraId": self.camera_id,
            "Year": self.year,
            "Month": self.month,
            "Day": self.day,
            "Hour": self.hour,
            "Minute": self.minute,
            "Second": self.second,
            "NumTrackedPeople": self.num_tracked_people,
            "HasSavedImage": self.has_saved_image
        }
