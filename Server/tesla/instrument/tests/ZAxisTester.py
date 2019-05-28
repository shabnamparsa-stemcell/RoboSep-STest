# 
# ZAxisTester.py
# Simple functional test for the Z Axis
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

import sys
from tesla.instrument.TeslaPlatform import TeslaPlatform
from tesla.instrument.Instrument import Instrument

NUM_TESTS = 100

# -----------------------------------------------------------------------------

def moveAndConfirm(axisMoveFunc, axisGetPosFunc, power, profile, position):
    '''Move the axis to the specified position & confirm that it got there.'''
    print "      Moving to position = %d" % (position)
    apply(axisMoveFunc, (position,))
    robotPosition = apply(axisGetPosFunc)
    print "Moved. Robot at position = %d" % (robotPosition)
    if robotPosition != position:
        print "Aborting run"
        sys.exit(1)
    
# -----------------------------------------------------------------------------

if __name__ == '__main__':
    # Set up the platform and initialise it
    instrument = Instrument()
    instrument.Initialise()
    platform = instrument.getPlatform()
    robot = platform.robot
    z = robot.Z()

    # Our functions for moving the robot and getting it's position (according
    # to the stepper card)
    zMoveFunc = robot.SetZPosition
    zGetPosFunc = robot.ZPosition

    # Then home the Z axis to be sure
    robot.HomeZ()

    tipData = TeslaPlatform.tipReferenceMap[1][1]
    pickupPosition = tipData.m_PickupPosition
    stripPosition = tipData.m_StripPosition
    strippedPosition = tipData.m_StrippedPosition
    travelPosition = float(platform._m_Settings[TeslaPlatform.TravelPositionLabel])
    zeroPosition = 0

    # The various settings we will use for testing
    powerSettings = (platform._m_Settings[TeslaPlatform.PickupTipPowerProfileLabel],
                     platform._m_Settings[TeslaPlatform.StripTipPowerProfileLabel],
                     z.m_Settings[z.MotorPowerLabel],
                     z.m_Settings[z.MotorHomingPowerLabel],
                     z.m_Settings[z.MotorIdlePowerLabel],
                     )
    velocityProfiles = (platform._m_Settings[TeslaPlatform.PickupTipVelocityProfileLabel],
                        platform._m_Settings[TeslaPlatform.StripTipVelocityProfileLabel],
                        instrument._m_Settings[Instrument.WickingExtractVelocityProfileLabel],
                        z.m_Settings[z.MotorStepVelocityLabel],
                        z.m_Settings[z.MotorHomingStepVelocityLabel],
                        )
    positions = (pickupPosition, stripPosition, strippedPosition, travelPosition, zeroPosition)

    # Now loop through the following scenario:
    # 1. Move down to tip pick up position, confirm position
    # 2. Move to tip strip position, confirm position
    # 3. Move to tip stripped position, confirm position
    # 4. Move to travel position, confirm position
    # 5. Move to zero position, confirm position
    for i in range(NUM_TESTS):
        print "\nZ axis test #%d" % (i + 1)

        for power in powerSettings:
            print "\nPower = %s" % (str(power))
            for profile in velocityProfiles:
                print "Velocity profile = %s" % (str(profile))
                for position in positions:
                    moveAndConfirm(zMoveFunc, zGetPosFunc, power, profile, position)

# eof
