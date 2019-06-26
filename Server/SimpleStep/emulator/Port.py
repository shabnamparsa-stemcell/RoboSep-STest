# Python bytecode 2.3 (62011)
# Embedded file name: SimpleStep\emulator\Port.py
# Compiled at: 2004-09-07 02:14:15
# Decompiled by https://python-decompiler.com
from ipl.utils.wait import wait_msecs
from .Board import Board, EmulatorError

class Port:
    """A fake serial port that looks like it has a SimpleStep card attached"""
    __module__ = __name__

    def __init__(self, port='COM1'):
        """Constructor. The port setting is ignored (it is just there for 
        compatibility with the 'real' SimpleStep class)
        """
        self.returnData = None
        self.portstr = port
        self.boards = []
        self.registerBoards()
        return

    def __repr__(self):
        return 'Fake serial port on %s' % (self.portstr,)

    def send(self, data):
        """Send a packet to the (supposed) serial port handle"""
        targetBoard = data[0:2]
        board = None
        for board in self.boards:
            if board.isTarget(targetBoard):
                self.returnData = board.acceptCmd(data)
                return

        return

    def check(self, pause=200):
        """Return any waiting data, None otherwise"""
        wait_msecs(pause)
        if self.returnData != None:
            data = self.returnData + '\r'
        else:
            data = None
        self.returnData = None
        return data
        return

    def sendAndCheck(self, data, pause=100):
        """Send data and then check for a response"""
        self.send(data)
        return self.check(pause)

    def registerBoards(self):
        for address in ['X0', 'Y0', 'X1', 'Y1', 'D2']:
            board = Board(address)
            self.boards.append(board)


if __name__ == '__main__':
    fakeCOM1 = Port('COM1')
    print(fakeCOM1.boards)
    print(fakeCOM1.sendAndCheck('D1'))
    print(fakeCOM1.sendAndCheck('D1v0'))