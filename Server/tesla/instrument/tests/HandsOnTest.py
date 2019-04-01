# 
# HandsOnTest.py
# Allow manual testing of Tesla components
# 
# Copyright (c) Invetech Operations Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Operations Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Operations Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

from tesla.instrument.Instrument import Instrument
from tesla.instrument.TeslaPlatform import TeslaPlatform
from tesla.instrument.TeslaPump import TeslaPump
from tesla.hardware.config import *
from tesla.types.tube import Tube
import tesla.hardware.AxisTracker
from tesla.config import COMPONENT_DB_PATH

# ---------------------------------------------------------------------------

def Setup():
    global i, pump, tp, car, rob, ts, c, z, t, p, cCard, zCard, tCard, pCard, gHardwareData
   
    #ReloadHardwareData('C:\Work\Tesla\Hardware.ini')
    print gHardwareData.Item ('Platform', 'carouselreferencepointoffset')
    print gHardwareData.Item ('Platform', 'robotthetareferencepointoffset')
    i = Instrument( "Instrument" )
    pump = i.getPump()
    tp = i.getPlatform()
    car = tp.getCarousel()
    rob = tp.getRobot()
    ts = tp.getTipStripper()
    c = car._ThetaAxis()
    z = rob.Z()
    t = rob.Theta()
    p = pump._TeslaPump__m_Pump
    cCard = c.m_Card
    zCard = z.m_Card
    tCard = t.m_Card
    pCard = p.m_Card
                          
    Initialise()
    SetupVials()
                     
def Initialise():
    global tp
    
    i.Initialise()

def TrackingReport():
    tesla.hardware.AxisTracker.Report(COMPONENT_DB_PATH)
    
def Check():
    global tp
    
    Initialise()
    tp.MoveTo (1, 'rear1ml3', 130)
    
def CheckHome(axis, overshoot, zeroPoint = 0):
    """ Use to estimate opto trigger point """
    axis.Home()
    i = 0
    while i < overshoot:
        i = i+1
        try:
            print '#%d: %d' % (i, axis.Step())
            axis.m_Card.processCmds(['P200,200,0', 'B1100', 'E3000', 'S5', 'RN+1','RY-1', 'P140,50,0'])
        except:
            print '...Bang!'
        else:
            i = overshoot

    axis.m_Card.processCmds(['A%d' % (zeroPoint)])
    
def Demo():
    print 'Initialising instrument.'
    Initialise()
    print 'Picking up 5ml tip from position 4, quadrant 4'
    tp.PickupTip(4, 4)
    print 'Moving to separation.'
    tp.MoveTo(4, 'separation', 115)
    print 'Stripping the tip'
    tp.StripTip()
    tp.PickupTip(4, 3)
    tp.StripTip()
    print 'Next...!'

def TestAspiration(volume, capacity = 1100):
    global pump
    pump.Aspirate (volume, capacity)
    pump.Dispense ()

def SetupVials (sector = 1):
    """Obtain references to all vials in a given sector."""
    global av, cv, bv, sv, mv, wv, lv, bulk
    av = i.ContainerAt (sector, Instrument.AntibodyLabel)
    cv = i.ContainerAt (sector, Instrument.CocktailLabel)
    bv = i.ContainerAt (sector, Instrument.BeadLabel)
    sv = i.ContainerAt (sector, Instrument.SampleLabel)
    mv = i.ContainerAt (sector, Instrument.SupernatentLabel)
    wv = i.ContainerAt (sector, Instrument.WasteLabel)
    lv = i.ContainerAt (sector, Instrument.LysisLabel)
    bulk = i.BulkBottle()
    print "vials in sector %d:" % (sector)
    print "av = Antibody"
    print "cv = Cocktail"
    print "bv = Bead"
    print "sv = Sample"
    print "mv = (magnetic) separation"
    print "wv = Waste"
    print "lv = Lysis"
    print "bulk = BCCM"
    
def SetVialVolume (vial, volume):
    """Set a given vial to have a given volume."""
    vial.removeVolume(vial.getVolume())
    vial.addVolume(volume)
    
def TestTransport (volume):
    srcVial = i.ContainerAt (0, Instrument.BCCMLabel)
    SetVialVolume (srcVial, 50000)
    dstVial = i.ContainerAt (4, Instrument.SupernatentLabel)
    i.Transport (dstVial, srcVial, volume, False)
    
def TestPumpWorkflows ():
    wasteVial = i.ContainerAt (4, Instrument.WasteLabel)
    mixVial = i.ContainerAt (4, Instrument.SampleLabel)
    mixVial.removeVolume(mixVial.getVolume())
    mixVial.addVolume(5000)
    print "Priming:"
    i.Prime (wasteVial)
    print "Mixing:"
    i.Mix (mixVial)
    print "Flushing:"
    i.Flush (wasteVial)

def TestPump ():
    pump.Initialise()
    
    pump.Aspirate(100,1100)
    pump.Dispense()
    
    pump.Aspirate(300,1100)
    pump.Dispense()
    
    pump.Aspirate(1000,5000)
    pump.Dispense()
    
    pump.Aspirate(4000,5000)
    pump.Dispense()

def TestTipStrip (nbrTimes = 1):
    tp.StripTip()
    for i in range (0, nbrTimes):
        tp.PickupTip(4, 3);tp.StripTip()
        tp.PickupTip(4, 4);tp.StripTip()
        
def MagicPump (ulPerSec):
    stepsPerSecond1 = ulPerSec / 1.33
    stepsPerSecond2 = ulPerSec / 0.1728
    b1 =  46250 - (1/stepsPerSecond1 - 0.000069789)/0.0000005425437
    b2 =  46250 - (1/stepsPerSecond2 - 0.000069789)/0.0000005425437
    return (b1, b2)

#------------------------- Test -----------------------
#
# EOF
