import urx
from urx.robotiq_two_finger_gripper import Robotiq_Two_Finger_Gripper
import time
import sys
import numpy as np
from numpy import sin, cos
from scipy.optimize import least_squares

def ikEq(x, pos):
    a, b, c, d, e = x
    m = [[-9*(-sin(b)*sin(c)*cos(a) + cos(a)*cos(b)*cos(c))*sin(d) + 9*(sin(b)*cos(a)*cos(c) + sin(c)*cos(a)*cos(b))*cos(d) + 2*sin(a) - 21*sin(b)*sin(c)*cos(a) + 21*cos(a)*cos(b)*cos(c) + 25*cos(a)*cos(b)], [-9*(-sin(a)*sin(b)*sin(c) + sin(a)*cos(b)*cos(c))*sin(d) + 9*(sin(a)*sin(b)*cos(c) + sin(a)*sin(c)*cos(b))*cos(d) - 21*sin(a)*sin(b)*sin(c) + 21*sin(a)*cos(b)*cos(c) + 25*sin(a)*cos(b) - 2*cos(a)], [9*(-sin(b)*sin(c) + cos(b)*cos(c))*cos(d) - 9*(-sin(b)*cos(c) - sin(c)*cos(b))*sin(d) - 21*sin(b)*cos(c) - 25*sin(b) - 21*sin(c)*cos(b) + 12]]
    return (m[0][0] - pos[0], m[1][0] - pos[1], m[2][0] - pos[2])

class RobotController():
    def __init__(self, a, v):
        self.curDest = np.array([0, -np.pi/2, 0, -np.pi/2, 0]).astype(float)
        self.rob = None
        self.a = a
        self.v = v

    
        try: 
            #TODO: create robot controller
            print("Initializing Robot, A = {}, V = {}".format(self.a, self.v))
            self.rob = urx.Robot("169.254.121.1")
            self.rob.set_tcp((0, 0, 0.1, 0, 0, 0))
            self.rob.set_payload(2, (0, 0, 0.1))
            self.rob.movej((0, -np.pi/2, 0, -np.pi/2, 0, 0), 1, 1, wait=False)
            print("Moving")
            time.sleep(5)

            print("ready to run")
        except:
            print("Something went wrong")
            if(self.rob is not None):
                self.rob.close()
            quit()
        
    def isSafe(self, command):
        safe = True
        if(command[1] < -3.2 or command[1] > 0): safe = False
        if(command[2] < -2.5 or command[2] > 2.5): safe = False
        
        # if seg[4] rotated + or - 90 deg. cannot rotate seg[3] more than 90
        #TODO: add more safety tests (illegal position, hits itself [might not be able to do that], unsafe position [hitting robot console], etc)
        return safe

    def controlRobot(self, command):
        print(np.round(command, 4))
        if(self.isSafe(command)):
            if(np.sqrt((command - self.curDest)**2).sum() > 0.2):
                self.curDest = np.copy(command)
                self.rob.stopj()
                time.sleep(0.1)
                self.rob.movej((command[0], command[1], command[2], command[3], 0, 0), self.a, self.v, wait=False)
        else:
            print("Unsafe command, canceled")
        time.sleep(1)

    def findJointMovements(self):
        f = open("../Assets/Logs/log.txt", "r")
        line = f.readline().rstrip()
        f.close()
        pos = line.split(" ")
        if(len(pos) < 2): # error handling in case both read and write performed at same time
            return self.curDest
        else:
            for i in range(len(pos)):
                pos[i] = float(pos[i])
            
            pos = [-pos[2], pos[0], pos[1]] # swap to robot arm coordinate system
            pos = np.array(pos) # assumed in range -1.5 to 1.5
            pos = pos*60
            pos = np.clip(pos, -100, 100) # make sure position does not go outside valid range

            # Lower and upper bounds for the variables
            bounds = [
                (-1 * np.pi, -1 * np.pi, -1 * np.pi, -1 * np.pi, -1 * np.pi), 
                (1 * np.pi, 1 * np.pi, 1 * np.pi, 1 * np.pi, 1 * np.pi)
                ]
            return least_squares(ikEq, self.curDest, args=(pos,), bounds=bounds).x

    def mainLoop(self):
        try:
            command = self.findJointMovements() # run this more often than updating robot arm position
            self.controlRobot(command)
        except:
            print("Something went wrong")
            if(self.rob is not None):
                self.rob.stopj()
                time.sleep(0.1)
                self.rob.movej((0, -np.pi/2, 0, -np.pi/2, 0, 0), 1, 1, wait=False)
                self.rob.close()
            quit()

if __name__ == "__main__":
    a = 1
    v = 0.4
    rob = RobotController(a, v)
    while(True):
        rob.mainLoop()