import re

from SimpleStep.mySimpleStep import mySimpleStep, mySimpleStepError
from ipl.utils.wait import wait_msecs
import time
import datetime

if (__name__ == '__main__'):
    import sys


    wheel = mySimpleStep('Y1', port='COM1')
    wheel.sendAndCheck('H4')
    wheel.sendAndCheck('P200,200,0')
    wheel.sendAndCheck('B1500')
    wheel.sendAndCheck('E1500')
    wheel.sendAndCheck('S1')

    raw_input('Press <Enter> to Start!!!')

    wheel.sendAndCheckDebug('N-1')
    
    try:
      wheel.sendAndCheckDebug('RN+1024')
    except:  
      endtimestamp = str(datetime.datetime.now())
      print '\n # Error Occurs on %s\n'%endtimestamp
      wheel.SendAbort()
      raw_input('Press <Enter> to Quit!!!')
    

