import re

from SimpleStep.mySimpleStep import mySimpleStep, mySimpleStepError
from ipl.utils.wait import wait_msecs
import time

if (__name__ == '__main__'):
    import sys


    zArm = mySimpleStep('X0', port='COM1')
    zArm.getBoardInfo(0)    
    zArm.sendAndCheck('H3')

    zArm.sendAndCheck('P200,200,0')
    zArm.sendAndCheck('B750')
    zArm.sendAndCheck('E750')
    zArm.sendAndCheck('S1')
    zArm.process('N-1',False)
    zArm.process('RN-350',False)    
    raw_input('Press <Enter> to Start!!!')
    zArm.sendAndCheck('P100,100,0')
    zArm.sendAndCheck('B3000')
    zArm.sendAndCheck('E15000')
    zArm.sendAndCheck('S5')
    zArm.process('RY-400',False)    
    zArm.process('N-1',False)        
    raw_input('Press <Enter> to finish!!!')
