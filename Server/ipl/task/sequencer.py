
import threading
import time
import sys

#DISPATCHER_TIMEOUT = 0.5 # in seconds added by shabnam

def myPrint(*args):
    """A simple helper function, provided in case the optional handler 
    parameter is not used in the Sequencer constructor."""
    print args


class SequencerException(StandardError):
    __module__ = __name__
    __doc__ = 'Exceptions related to the sequencer'

class Sequencer:
    __module__ = __name__
    __doc__ = 'A basic event sequencer.'

    def __init__(self, schedule, handler = myPrint, exitFunc = None, preExecuteHandler = None, onPausedHandler = None, doesActionRequirePushbackIfLate = None, timeFactor = 1, debug = False):
        """Construct the sequence engine. Use run() to start it up. The
\tschedule is a list of pairs of (start time, event); actually, 
\tit can be any object that supports a sort() method that will 
\tsort the pairs into time-ascending order, a pop(index = -1) 
\tmethod that will return a pair and slicing (for the copy). 
\tNote that we copy the schedule so that the original "list" is not
\tmodified.
\tThe optional handler is passed the event and is expected to 
\tprocess it appropriately. The optional exitFunc is a handler that can
\tbe called when the dispatcher has completed the schedule (which is
\thandy if you want a mechanism for reporting that the Sequencer has
\tcompleted processing the schedule).
\tThe optional timeFactor parameter allows you to accelerate the rate
\tat which the dispatcher works; allowing you to speed up or slow down 
\ttime!
        Setting debug to True enables the internal debug mode. This can also be
        set with the enableDebugging() method.
\t"""
        self.handler = handler
        self.exitFunc = exitFunc
        self.preExecuteHandler = preExecuteHandler
        self.onPausedHandler = onPausedHandler
        self.doesActionRequirePushbackIfLate = doesActionRequirePushbackIfLate
        self._Sequencer__timeFactor = float(timeFactor)
        self._Sequencer__debug = debug
        self.thrd = threading.Thread(target=self.dispatcher, args=())
        self.thrd.setDaemon(True)
        self.schedule = schedule[:]
        self.now = 0
        self._Sequencer__isRunning = threading.Event()
        self._Sequencer__isRunning.clear()
        self._Sequencer__halt = threading.Event()
        self._Sequencer__halt.clear()
        self._Sequencer__resume = threading.Event()
        self._Sequencer__resume.set()
        self._Sequencer__break = threading.Event()
        self._Sequencer__break.clear()



    def enableDebugging(self, debugState = True):
        """Enable or disable internal debugging; default is to enable."""
        self._Sequencer__debug = debugState



    def getDebuggingState(self):
        """Returns the boolean state of the debugging flag"""
        return self._Sequencer__debug



    def getTime(self):
        """Return the time now (which may be accelerated by a time factor)."""
        return (time.time() * self._Sequencer__timeFactor)



    def dispatcher(self):
        """The thread function which dispatches the events in the schedule.
\tThe schedule is a list of pairs of (start time, event)."""
        self._Sequencer__isRunning.set()

        #print "sequencer... sleep"
        #time.sleep(1)  
        #time.sleep(1)
        #time.sleep(1)
        #time.sleep(1)
        #time.sleep(DISPATCHER_TIMEOUT) #added by shabnam
        print "sequencer... dispatch now"


        
        t0 = self.getTime()
        self.schedule.sort()
        while (len(self.schedule) and self.isNotHalted()):
            (start, event,) = self.schedule.pop(0)
            while self.isNotHalted():
                if self.preExecuteHandler:
                    apply(self.preExecuteHandler, (None,))
                if (self.isPaused() and self.onPausedHandler):
                    apply(self.onPausedHandler, (None,))
                if self._Sequencer__debug:
                    print 'self.__resume.wait()'
                self._Sequencer__resume.wait()
                if (not self.isNotHalted()):
                    break
                now = (self.getTime() - t0)
                if self._Sequencer__debug:
                    print 'now, start =',
                    print (now,
                     start)
                if (now >= start):
                    try:
                        lateness = (now - start)
                        if ((lateness > 0.01) and ((self.doesActionRequirePushbackIfLate != None) and apply(self.doesActionRequirePushbackIfLate, (event,)))):
                            if self._Sequencer__debug:
                                print 'Pushing back schedule by',
                                print lateness,
                                print 'seconds'
                            self.pushBackSchedule(lateness)
                        elif self._Sequencer__debug:
                            print 'No need to push back schedule'
                        apply(self.handler, (event,))
                    except:
                        self._Sequencer__halt.set()
                        self._Sequencer__break.set()
                        (exceptionType, exceptionMsg, tb,) = sys.exc_info()
                        if self._Sequencer__debug:
                            print ('Caught %s: %s' % (exceptionType,
                             exceptionMsg))
                        raise SequencerException, ('%s: %s' % (exceptionType,
                         exceptionMsg))
                    break
                else:
                    delay = ((start - now) / self._Sequencer__timeFactor)
                    if (delay > 1.0):
                        delay = 1.0
                    self._Sequencer__break.wait(delay)


        if (self.exitFunc != None):
            apply(self.exitFunc, ())
        self._Sequencer__isRunning.clear()



    def run(self):
        """Start the sequencer dispatcher running."""
        self._Sequencer__break.clear()
        self._Sequencer__halt.clear()
        self._Sequencer__resume.set()
        self.thrd.start()


    def abandonDispatching(self):
        """Halt the dispatcher from inside the dispatcher thread. If you want
\tto terminate the dispatching from outside that thread, call halt()
\tinstead."""
        if self._Sequencer__debug:
            print 'Dispatching abandoned'
        self._Sequencer__halt.set()
        self._Sequencer__resume.set()
        self._Sequencer__break.set()



    def isRunning(self):
        """Returns True if the dispatcher is running."""
        return (self.thrd.isAlive() and self._Sequencer__isRunning.isSet())



    def isNotHalted(self):
        """Returns True if the halt flag is NOT set."""
        return (not self._Sequencer__halt.isSet())



    def isPaused(self):
        """Returns True if the sequencer is paused (but not halted)"""
        return (self.isRunning() and (not self._Sequencer__resume.isSet()))



    def pause(self):
        """Pause the dispatcher (allowing it to be resumed at a later time)."""
        self._Sequencer__resume.clear()
        self._Sequencer__break.set()


    def resume(self):
        """Resume a paused dispatcher."""
        self._Sequencer__break.clear()
        self._Sequencer__resume.set()



    def halt(self, timeout = None):
        """Halt the dispatcher (with no resumption of event dispatching)."""
        print '\n >>> Abort Set! \n'
        if self.thrd.isAlive():
            self._Sequencer__halt.set()
            self._Sequencer__resume.set()
            self._Sequencer__break.set()
            threading.Thread.join(self.thrd, timeout)



    def pushBackSchedule(self, seconds):
        """Pushes back the remaining blocks in the schedule by the given
        number of seconds"""
        newSchedule = []
        for action in self.schedule:
            (start, event,) = action
            newAction = ((start + seconds),
             event)
            newSchedule.append(newAction)

        self.schedule = newSchedule




