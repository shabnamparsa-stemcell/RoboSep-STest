import serial
from ipl.utils.wait import wait_msecs
from threading import *

sem = Semaphore()
class SerialComms:
    __module__ = __name__
    __doc__ = 'Provide sufficient amount of serial comms to communicate with a \n    SimpleStep stepper motor controller board\n    '
    CHARACTER_WAIT_DELAY_MS = 5

    def __init__(self, port = 'COM1', baudrate = 57600):
        """Construct an instance with the serial port and baudrate"""
        self.baudrate = baudrate
        self.port = serial.Serial(port, baudrate)
        self.port.flushInput()
        self.port.flushOutput()
        self.portstr = self.port.portstr

    def open(self):
        self.port.open();  
    
    def close(self):
        self.port.close();

    def send(self, data):
        """Send a packet to the serial port handle"""
        packet = ('%s\r' % (data))
        self.port.write(packet)

    def sendwithout(self, data):
        """Send a packet to the serial port handle"""
        packet = ('%s' % (data))
        self.port.write(packet)



    def check(self, timeout = 0):
        """Return any waiting data, None otherwise"""
        data = ''
        numChars = 0
        wait = self.CHARACTER_WAIT_DELAY_MS
        if not self.baudrate == 57600:
            wait = 15
            
        while True:
            wait_msecs(wait)
            timeout -= wait
            newNumChars = self.port.inWaiting()
            
            if ((newNumChars == numChars) and (numChars > 0)):
                break
            else:
                numChars = newNumChars
            if (timeout <= 0):
                break

        data = self.port.read(numChars)
        if (data != ''):
            return data
        else:
            return None



    def sendAndCheck(self, data, pause = 100):
        """Send data and then check for a response"""
        sem.acquire()
        self.send(data)
        response = self.check(pause)
        sem.release()
        return response
    
    def sendAndCheckWithout(self, data, pause = 100):
        """Send data and then check for a response"""
        sem.acquire()
        self.sendwithout(data)
        response = self.check(pause)
        sem.release()
        return response

if (__name__ == '__main__'):
    ser = SerialComms()
    ser.send('D1')
    print ser.check()
    ser.send('U0')
    print ser.check()

