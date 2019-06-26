# File: s (Python 2.3)

import threading

class SimpleThread(threading.Thread):
    '''A simple thread class.'''
    
    def __init__(self, task, threadName = 'SimpleThread'):
        '''The basic constructor: task is a function that takes two parameters,
\tas described in run() and an optional thread identifier name.'''
        self._SimpleThread__task = task
        self._SimpleThread__stopEvent = threading.Event()
        self._SimpleThread__sleepPeriod = 1.0
        threading.Thread.__init__(self, name = threadName)

    
    def setSleepPeriod(self, newPeriod):
        '''Set a new sleep period (in seconds).'''
        self._SimpleThread__sleepPeriod = newPeriod

    
    def run(self):
        '''Overloaded threading.thread.run(). We call self.task, which
\ttakes two parameters, an event object and a sleep period.'''
        self._SimpleThread__task(self._SimpleThread__stopEvent, self._SimpleThread__sleepPeriod)

    
    def join(self, timeout = None):
        '''Stop the thread.'''
        self._SimpleThread__stopEvent.set()
        threading.Thread.join(self, timeout)


if __name__ == '__main__':
    import time
    
    def myTask(eventObj, sleepPeriod):
        count = 0
        while not eventObj.isSet():
            count += 1
            print('Count = %d' % (count,))
            eventObj.wait(sleepPeriod)

    testThread = SimpleThread(myTask)
    print('Thread is called:', testThread.getName())
    testThread.start()
    time.sleep(10.0)
    testThread.join()

