# File: _ (Python 2.3)


def simpleHash(x):
    """Create a simple hash that is guaranteed to be a positive number.
    Note that this (in theory) could cause a clash, but that's okay for the
    sorts of applications we are using it for."""
    return abs(hash(x))


def hexHash(x):
    '''Return a hex version of simpleHash().'''
    return hex(simpleHash(x))


def isIterable(item):
    '''Returns True if the item can be iterated over. Only works in 
    Python 2.2 and above.'''
    
    try:
        iter(item)
    except:
        return False

    return True


def isStringLike(item):
    '''Returns True if this is a string-like object.'''
    
    try:
        pass
    except TypeError:
        return False

    return True


def isScalar(item):
    '''Returns True if item is a scalar.'''
    if not isStringLike(item):
        pass
    return not isIterable(item)


def flattenSequence(sequence, result = None):
    '''Flatten a nested sequence; this is adapted from section 1.12 of the 
    Python Cookbook.'''
    if result is None:
        result = []
    
    if isScalar(sequence):
        result = [
            sequence]
    else:
        for item in sequence:
            if isScalar(item):
                result.append(item)
                continue
            flattenSequence(item, result)
        
    return result

