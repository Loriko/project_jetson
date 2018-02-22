from threading import Thread
import random_based_camera_emulator
import traceback


# Run this file to start the emulating multiple camera systems sending people count data to the back end

# Number of camera system to emulate simultaneously
EMULATED_CAMERA_COUNT = 10
CAMERA_THREADS = []


def start_emulating_cameras():
    print "Booting emulator"
    for x in xrange(0, EMULATED_CAMERA_COUNT):
        # boot camera emulation
        print "Scheduling camera # %d" % x
        CAMERA_THREADS.append(Thread(target=random_based_camera_emulator.emulate_camera, args=(x,)))
    try:
        for thread in CAMERA_THREADS:
            thread.start()
        for thread in CAMERA_THREADS:
            thread.join()
    except:
        "Exception occurred in one of the threads"
        traceback.print_exc()
    # Following code will only execute once all the threads have executed
    print "Emulation completed"


start_emulating_cameras()
