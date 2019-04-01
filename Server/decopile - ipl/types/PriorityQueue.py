# File: P (Python 2.3)

import bisect

class PriorityQueue:
    '''A queue container that is sorted by priority. Each element in the queue
    is a tuple of priority and data.'''
    
    def __init__(self):
        '''Constructor; creates an empty queue.'''
        self.queue = []

    
    def __len__(self):
        '''Return the length of the queue.'''
        return len(self.queue)

    
    def insert(self, data, priority):
        '''Insert a new element in the queue according to its priority.'''
        bisect.insort(self.queue, (priority, data))

    
    def pop(self):
        '''Pop the highest-priority element of the queue.'''
        return self.queue.pop()[1]


