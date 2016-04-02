#!/usr/local/bin/python
from OSC import OSCServer
import sys
from time import sleep

server = OSCServer( ("192.168.253.200", 5555) )
server.timeout = 0
run = True

# this method of reporting timeouts only works by convention
# that before calling handle_request() field .timed_out is 
# set to False
def handle_timeout(self):
    self.timed_out = True

# funny python's way to add a method to an instance of a class
import types
server.handle_timeout = types.MethodType(handle_timeout, server)

def user_callback(path, tags, args, source):
    print "from" , path , "we got", args

def quit_callback(path, tags, args, source):
    # don't do this at home (or it'll quit blender)
    global run
    run = False

def camera_callback(path, tags, args, source):
    print "CAMERACALLBACK: from" , path , "we got", args

server.addMsgHandler( "/SWG/camera/4/positionorientation", camera_callback )
server.addMsgHandler( "/test", user_callback )
server.addMsgHandler( "/test/alive", user_callback )
server.addMsgHandler( "/quit", quit_callback )

# user script that's called by the game engine every frame
def each_frame():
    # clear timed_out flag
    server.timed_out = False
    # handle all pending requests then return
    while not server.timed_out:
        server.handle_request()

# simulate a "game engine"
while run:
    # do the game stuff:
    sleep(1)
    # call user script
    each_frame()

server.close()

