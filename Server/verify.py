import re

from SimpleStep.mySimpleStep import mySimpleStep, mySimpleStepError
from ipl.utils.wait import wait_msecs
import time

if (__name__ == '__main__'):
    import sys

    print('\n--------------------------------------')
    initpoint = int(input('Initial Steps: '))
    pwr       = int(input('Initial Power Profile: '))
    begin     = int(input('Initial Begin Velocity: '))  
    end       = int(input('Initial End Velocity: '))  
    slope     = int(input('Initial Slope Velocity: '))  
    targetpoint     = int(input('Evaluation Steps: '))
    targetpwr       = int(input('Evaluation Power Profile: '))
    targetbegin     = int(input('Evaluation Begin Velocity: '))  
    targetend       = int(input('Evaluation End Velocity: '))  
    targetslope     = int(input('Evaluation Slope Velocity: '))      
    print('--------------------------------------\n\n')
    
        
    zArm = mySimpleStep('X0', port='COM1')
    zArm.getBoardInfo(0)    
    zArm.sendAndCheck('H3')

    for _ in range(0, 10):     
        zArm.sendAndCheck('P%d,%d,0'%(pwr,pwr))
        zArm.sendAndCheck('B%d'%begin)
        zArm.sendAndCheck('E%d'%end)
        zArm.sendAndCheck('S%d'%slope)
        zArm.process('N-1',False)
        zArm.process(('RN+%d'%initpoint),False)
        zArm.process('N-1',False)
        
        zArm.sendAndCheck('P%d,%d,0'%(targetpwr,targetpwr))
        zArm.sendAndCheck('B%d'%targetbegin)
        zArm.sendAndCheck('E%d'%targetend)
        zArm.sendAndCheck('S%d'%targetslope)

        zArm.process(('RN+%d'%targetpoint),False)  
        zArm.process('N-0H',False)
        zArm.port.sendAndCheck(zArm.prefix+'TY1,1', zArm.POLL_PAUSE )
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            zArm.ExtLogger.SetLog( ('#[%sTY1,1] Sent '%zArm.prefix), zArm.prefix  )
        zArm.sendAndCheck('C-',False)
        rtn = zArm.sendAndCheck('m',False)
        zArm.process('!',False)
        zArm.process('TN',False)
        zArm.process('N-1',False)        
        if zArm.ver < 109 :
          if ( int(rtn[0]) == 0 ):
              LossCount = 0
          else:
              LossCount = int(rtn[0]) - 65536

        else:
          LossCount = int(rtn[0])
        
        if (LossCount == 0):
            print('\n#### LossCount %d ####\n'%LossCount)
            if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
                zArm.ExtLogger.CheckSystemAvail()
                zArm.ExtLogger.DumpHistory()
        else:
            print('\n#### LossCount %d ####\n'%LossCount)

        time.sleep(1)    

    input('Press <Enter> to finish!!!')
