#!/usr/local/bin/python


import OSC
import time

counter = 0

while(1):
    c = OSC.OSCClient()
    c.connect(('127.0.0.1', 5556))  # connect to a Unity camera (OSC Server)
    oscmsg = OSC.OSCMessage()

    oscMessage = '(-23.16295, 1.55000, ' + str((counter/100.0) - 5.0) + ')/(' + str((counter) * 10 % 360) + ', 261.26000, ' + str((counter) * 2 % 180) + ')'
    oscAddress = "/SWG/camera/OTHERCAMERA/positionorientation"
    oscmsg.setAddress(oscAddress)
    oscmsg.append(oscMessage)
    print counter , "sending", oscMessage, " to Unity to ", oscAddress
    c.send(oscmsg)


    counter = counter + 1
    time.sleep(0.01)

