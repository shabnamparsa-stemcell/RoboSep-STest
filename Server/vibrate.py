import re

from SimpleStep.mySimpleStep import mySimpleStep, mySimpleStepError
from ipl.utils.wait import wait_msecs
import time

if (__name__ == '__main__'):
    import sys


    zArm  = mySimpleStep('X0', port='COM1')
    thArm = mySimpleStep('Y0', port='COM1')
    Carou = mySimpleStep('Y!', port='COM1')
    
    zArm.sendAndCheck('P200,200,0')
    thArm.sendAndCheck('P200,200,0')
    Carou.sendAndCheck('P200,200,0')
    
    input('Press <Enter> to finish!!!')


