#!/usr/local/bin/python


import OSC
import time

counter = 0

while(1):
    c = OSC.OSCClient()
    c.connect(('127.0.0.1', 5556))  # connect to Unity
    oscmsg = OSC.OSCMessage()
    oscmsg.setAddress("/toUnity")
    oscmsg.append('Hi Unity')

    print counter , "sending HELLO to Unity"
    counter = counter + 1
    c.send(oscmsg)

    time.sleep(1)

