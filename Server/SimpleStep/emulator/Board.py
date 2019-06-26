# Python bytecode 2.3 (62011)
# Embedded file name: SimpleStep\emulator\Board.py
# Compiled at: 2004-08-27 07:38:35
# Decompiled by https://python-decompiler.com
import types, string, time

class EmulatorError(Exception):
    __module__ = __name__


class Board:
    """A fake SimpleStep board"""
    __module__ = __name__
    NUMBER_IN_PORTS = 6
    NUMBER_OUT_PORTS = 2
    READY_STATUS_CHAR = '>'
    HOMED_STATUS_CHAR = 'h'

    def __init__(self, address):
        """Constructor. The address is the board's address (eg. D1)"""
        self.address = address
        self.returnAddress = address.lower()
        self.numBits = 16
        self.maxSteps = 2 ** self.numBits - 2
        self.halfStepMode = False
        self.B = 38000
        self.E = 45000
        self.slope = 15
        self.position = 0
        self.homePosition = 0
        self.moving = 0
        self.movingEndTime = None
        self.previousStatus = Board.READY_STATUS_CHAR
        self.currentStatus = Board.READY_STATUS_CHAR
        self.ports = {}
        for port in range(Board.NUMBER_OUT_PORTS):
            self.ports[port] = 0

        return

    def __repr__(self):
        return 'Emulated SimpleStep board @ %s' % (self.address,)

    def isTarget(self, address):
        """These are not the droids you are looking for..."""
        return self.address == address

    def acceptCmd(self, cmd):
        """Take the command packet sent to the emulated serial port and
        process it (if we are not in the middle of a move or some other
        time-consuming action)
        """
        if self.moving and not self.moveTimeHasElapsed():
            return
        cmdCode, params = self.parseCmd(cmd)
        if self.isMoveCommand(cmdCode):
            status = self.startMove(cmdCode, params)
        else:
            status = self.processNonMoveCommand(cmdCode, params)
        if status != None:
            return '%s%s%s' % (self.returnAddress, self.currentStatus, status)
        else:
            return '%s%s>' % (self.returnAddress, self.currentStatus)
        return

    def moveTimeHasElapsed(self):
        """Returns true if the time taken to move the motor elapsed"""
        now = time.clock()
        status = False
        if self.movingEndTime == None:
            status = False
        else:
            if now > self.movingEndTime:
                self.stopMove()
                status = True
        return status
        return

    def stopMove(self):
        """Stop the emulated robot moving"""
        self.moving = False
        self.movingEndTime = None
        return

    def isMoveCommand(self, cmdCode):
        """Returns true if the command is to move the motor"""
        MOVE_CMDS = [
         'M', 'R', 'N', 'C']
        return cmdCode in MOVE_CMDS

    def startMove(self, cmdCode, params):
        """Start the motor moving"""
        moveMethod = {'R': self.startRelativeMove, 'M': self.startAbsoluteMove, 'N': self.nullMotorCommand}
        try:
            moveMethod[cmdCode](*(params,))
        except:
            raise EmulatorError('Move command %c is not yet supported' % cmdCode)

    def isValidPosition(self, position):
        """Is the position valid for this stepper's min & max travel"""
        if type(position) != int:
            raise EmulatorError('The position (%s) has to be a number!' % str(position))
        return position >= 0 and position <= self.maxSteps

    def moveTo(self, position):
        """Move the motor to a specific position (in steps)"""
        if not self.isValidPosition(position):
            raise EmulatorError('Whoops! Invalid position')
        else:
            distance = abs(self.position - position)
            self.position = position
            self.__moveRobot(distance)
            return None
        return

    def startRelativeMove(self, params):
        """Start a relative motor movement"""
        lookingForOpto = params[0] == 'Y'
        relativeSteps = int(params[1:])
        position = self.position + relativeSteps
        minPos = min(position, self.position)
        maxPos = max(position, self.position) + 1
        if lookingForOpto and self.homePosition in range(minPos, maxPos):
            self.moveTo(self.homePosition)
            self.currentStatus = Board.HOMED_STATUS_CHAR
        else:
            self.moveTo(position)
        return None
        return

    def startAbsoluteMove(self, params):
        """Start an absolute motor movement"""
        position = int(params)
        self.moveTo(position)

    def nullMotorCommand(self, params):
        """Zero/home the motor: For now, move to home position"""
        self.moveTo(self.homePosition)

    def __moveRobot(self, distance_steps):
        """Private method that determines the time it will take to move
        a certain distance (in steps) at the current velocity profile
        """
        now = time.clock()

    def processNonMoveCommand(self, cmdCode, params):
        """Process one of the commands that is not related to moving the stepper"""
        trCmds = {'*': 'STAR', '!': 'BANG'}
        if cmdCode in list(trCmds.keys()):
            cmdCode = trCmds[cmdCode]
        cmdMethodName = 'self.process_%s_cmd' % cmdCode
        try:
            cmdMethod = eval(cmdMethodName)
        except:
            raise EmulatorError('Command %s is not yet implemented' % cmdCode)

        try:
            i = len(params)
        except TypeError:
            params = [
             params]

        return cmdMethod(*(params,))

    def process__cmd(self, params):
        """Empty command, used to check for card's presence"""
        return ''

    def process_c_cmd(self, params):
        """Change the communications baudrate"""
        baudRateIndex = int(params[0])
        if baudRateIndex not in list(range(0, 5)):
            raise EmulatorError('%d is an invalid baudrate parameter' % baudRateIndex)
        return ''

    def process_STAR_cmd(self, params):
        """Abort motor movement with deceleration"""
        pass

    def process_BANG_cmd(self, params):
        """Stop all motor movements"""
        self.stopMove()

    def process_z_cmd(self, params):
        """Reset the current and previous status of the axes"""
        self.previousStatus = Board.READY_STATUS_CHAR
        self.currentStatus = Board.READY_STATUS_CHAR

    def process_A_cmd(self, params):
        """Set the current motor position. We also make this home"""
        self.position = int(params)
        self.homePosition = self.position

    def process_H_cmd(self, params):
        """Set the motor to half step mode"""
        self.halfStepMode = True

    def process_P_cmd(self, params):
        """Set the motor powerprofile"""
        pass

    def process_F_cmd(self, params):
        """Set the motor to full step mode"""
        self.halfStepMode = False

    def process_B_cmd(self, params):
        """Set the B value"""
        self.B = self.convertParam(params, 46000)

    def process_E_cmd(self, params):
        """Set the E value"""
        self.E = self.convertParam(params, 46250)

    def process_S_cmd(self, params):
        """Set the slope value"""
        self.slope = self.convertParam(params, 100)

    def process_b_cmd(self, params):
        """Return the current B value"""
        return self.B

    def process_e_cmd(self, params):
        """Return the current E value"""
        return self.E

    def process_s_cmd(self, params):
        """Return the current slope value"""
        return self.slope

    def process_m_cmd(self, params):
        """Return the current motor position"""
        return self.position

    def process_d_cmd(self, params):
        """Perform a delay"""
        pass

    def process_v_cmd(self, params):
        """Get board information"""
        infoParam = int(params)
        infoFields = {0: '00104', 1: '00255', 2: 'TBD', 3: 'TBD', 4: 'TBD', 5: 'TBD', 6: 'TBD'}
        return infoFields.get(infoParam, 'XXX')

    def process_I_cmd(self, params):
        """Return the status of the specified port"""
        if params[0] == None:
            if self.position == self.homePosition:
                bitmask = 1
            else:
                bitmask = 0
            return bitmask
        else:
            port = int(params[0])
            if port == 1 and self.position == self.homePosition:
                status = 1
            else:
                status = 0
            return status
        return

    def process_O_cmd(self, params):
        """Toggle a digital output port"""
        paramList = str.split(params, ',')
        port = int(paramList[0])
        state = int(paramList[1])
        self.ports[port] = state

    def parseCmd(self, cmd):
        """Parse a command out, stripping off the board address first
        Returns the command code and an optional string containing the
        various parameters that will require further parsing
        """
        cmdLen = len(cmd)
        if cmdLen < 2:
            raise EmulatorError('Zero-length command packet?')
        params = None
        if cmdLen == 2:
            cmdCode = ''
        else:
            cmdCode = cmd[2]
            if cmdLen > 3:
                params = cmd[3:]
        return (
         cmdCode, params)
        return

    def convertParam(self, param, limit):
        """Return the param as an int, checking that it is within the limit"""
        val = int(param)
        if val < 1 or val > limit:
            raise EmulatorError('Invalid value (%d)')
        return val


if __name__ == '__main__':
    pass
