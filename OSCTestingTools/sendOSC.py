#!/usr/local/bin/python


import OSC
import time

counter = 0

while(1):
    c = OSC.OSCClient()
    c.connect(('127.0.0.1', 5555))   # connect to SuperCollider
    oscmsg = OSC.OSCMessage()
    oscmsg.setAddress("/test")
    oscmsg.append('HELLO')

    print counter , "sending HELLO"
    counter = counter + 1
    c.send(oscmsg)

    time.sleep(1)

