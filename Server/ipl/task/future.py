# File: f (Python 2.3)

from threading import *
import copy

class Future:
    """From the Python Cookbook discussion:
    The Future class sometimes allows you to hide the fact that you're 
    using threading while still taking advantage of threading's potential 
    performance advantages.

    Although Python's thread syntax is nicer than the syntax in many 
    languages, it can still be a pain if all you want to do is run a 
    time-consuming function in a separate thread while allowing the main 
    thread to continue uninterrupted. A Future object provides a legible 
    and intuitive way to achieve such an end. 

    To run a function in a separate thread, simply put it in a Future
    object: 

    >>> A=Future(longRunningFunction, arg1, arg2 ...)

    Both the calling thread and the execution of the function will 
    continue on their merry ways until the caller needs the function's 
    result. When it does, the caller can read the result by calling Future 
    like a function. For example: 

    >>> print A(  )

    If the Future object has completed executing, the call returns 
    immediately. If it is still running, the call (and the calling thread in 
    it) blocks until the function completes. The result of the function is 
    stored in an attribute of the Future instance, so subsequent calls to it 
    return immediately. 

    Since you wouldn't expect to be able to change the result of a function, 
    Future objects are not meant to be mutable. This is enforced by requiring 
    Future to be called, rather than directly reading _ _result. If desired, 
    stronger enforcement of this rule can be achieved by playing with 
    __getattr__ and __setattr__ or, in Python 2.2, by using property. 

    Future runs its function only once, no matter how many times you read it. 
    Thus, you will have to recreate Future if you want to rerun your function 
    (e.g., if the function is sensitive to the time of day). 

    For example, suppose you have a function named muchComputation that can 
    take a rather long time (tens of seconds or more) to compute its results, 
    because it churns along in your CPU or it must read data from the network 
    or from a slow database. You are writing a GUI, and a button on that GUI 
    needs to start a call to muchComputation with suitable arguments, 
    displaying the results somewhere on the GUI when done. You can't afford to
    run the function itself as the command associated with the button, since 
    if you did, the whole GUI would appear to freeze until the computation is 
    finished, and that is unacceptable. Future offers one easy approach to 
    handling this situation. First, you need to add a list of pending Future 
    instances that are initially empty to your application object called, for 
    example, app.futures. When the button is clicked, execute something like
    this: 

    app.futures.append(Future(muchComputation, with, its, args, here))

    and then return, so the GUI keeps being serviced (Future is now running the
    function, but in another thread). Finally, in some periodically executed 
    poll in your main thread, do something like this: 

    for future in app.futures[:]:    # Copy list and alter it in loop
\tif future.isDone( ):
\t    appropriately_display_result(future( ))
            app.futures.remove(future)
    """
    
    def __init__(self, func, *param):
        '''Constructor: pass in the function and optional parameters for it.'''
        self._Future__done = False
        self._Future__result = None
        self._Future__status = 'working'
        self._Future__caughtException = False
        self._Future__exception = RuntimeError()
        self._Future__C = Condition()
        self._Future__T = Thread(target = self._Future__wrapper, args = (func, param))
        self._Future__T.setName('FutureThread')
        self._Future__T.start()

    
    def isDone(self):
        '''Returns True if the function is done, False otherwise.'''
        return self._Future__done

    
    def hasCaughtException(self):
        '''Returns True if the function caught an exception during execution.'''
        return self._Future__caughtException

    
    def __repr__(self):
        '''Returns a string representation of the future.'''
        return '<Future at ' + hex(id(self)) + ':' + self._Future__status + '>'

    
    def __call__(self):
        '''Method executed when the object is called.'''
        self._Future__C.acquire()
        while not (self._Future__done):
            self._Future__C.wait()
        self._Future__C.release()
        if self._Future__caughtException:
            raise self._Future__exception
        else:
            result = copy.deepcopy(self._Future__result)
            return result

    
    def _Future__wrapper(self, func, param):
        '''Private method: Run the actual function and housekeep around it.'''
        self._Future__C.acquire()
        self._Future__caughtException = False
        
        try:
            self._Future__result = func(*param)
        except Exception:
            inst = None
            self._Future__caughtException = True
            self._Future__exception = inst

        self._Future__done = True
        self._Future__status = repr(self._Future__result)
        self._Future__C.notify()
        self._Future__C.release()


if __name__ == '__main__':
    import time
    
    def waitAwhile(delay = 42):
        time.sleep(delay)
        return delay

    delay = 2
    A = Future(waitAwhile, delay)
    print('Is our %d delay finished? %s' % (delay, A.isDone()))
    print("Now let's wait until we know it is definitely finished...")
    time.sleep(delay * 2)
    print('Is our %d delay finished? %s' % (delay, A.isDone()))
    print('And the result is:', A())
    print("Now let's try that again for a 30 second delay...")
    delay = 30
    aBigNumber = 123456789
    B = Future(waitAwhile, delay)
    print('We are off sleeping in a thread for %d seconds' % delay)
    print('We could spend this time doing all sorts of other things.')
    print("Let's loop from 1 to %d" % aBigNumber)
    j = 0
    for i in range(0, aBigNumber):
        j += i
        if B.isDone():
            print('We counted up to i = %d before our thread completed' % i)
            break
            continue
    
    print('And the result is:', B())

