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
    zArm.process('RN+1024',False)
    raw_input('Press <Enter> to Start!!!')
    
    zArm.process('N-1',False)

    overstep = 0
    chk = int((zArm.sendAndCheck('I',False))[0])
    print chk   

    while 1 :
          zArm.process('RN-1',False)
          overstep = overstep + 1
          chk = int((zArm.sendAndCheck('I',False))[0])
          print 'Steps: %d, check: %d'%(overstep,chk)
          if chk == 1020 :
             break
             
    raw_input('Press <Enter> to finish!!!')


