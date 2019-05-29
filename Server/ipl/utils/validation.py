# File: v (Python 2.3)


def swap(a, b):
    '''Swap two values, returning them as a tuple'''
    return (b, a)


def validateNumber(number, lower, upper):
    '''Returns True if the number is within the lower and upper boundaries.
    False otherwise'''
    if upper < lower:
        (lower, upper) = swap(lower, upper)
    
    if number >= lower:
        pass
    return number <= upper

