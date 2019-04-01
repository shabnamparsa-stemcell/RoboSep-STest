# File: w (Python 2.3)

import time

def wait_msecs(period_msecs):
    '''Wait a certain number of milliseconds'''
    if period_msecs < 0:
        raise ValueError, 'We have to wait at least zero msecs!'
    
    delay = period_msecs / 1000.0
    time.sleep(delay)

if __name__ == '__main__':
    raise NotImplementedError, 'No application behaviour (yet)'

