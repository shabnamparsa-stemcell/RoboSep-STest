# File: s (Python 2.3)

import re

def expandCSharpString(buffer, args = ()):
    '''Take a C# string that has expandable and variable parameters, similar
    to C\'s printf strings and convert them into fully expanded strings, 
    suitable for use in Python.
    buffer is the string with parameter replacement tags specified as {N},
    where N is a number between 0 and 9.
    args is a tuple of arguments
    Note that all the arguments are converted into strings, even if they don\'t 
    start that way.
    
    For example, turn:
\t"After {0} cycles, state = {1}", (42, \'IDLE\')
    into:
    \t"After 42 cycles, state = IDLE"

    And the arguments don\'t have to be in order, for example:
\t"My {3} name is {2} and my {1} name is {0}" , (\'Cross\', \'last\', \'Graeme\', \'first\')
    becomes:
\t"My first name is Graeme and my last name is Cross"
    '''
    searchString = '(\\{(\\d)\\})'
    pattern = re.compile(searchString)
    hits = pattern.findall(buffer)
    if not len(hits) == len(args):
        raise AssertionError
    if hits:
        for entry in hits:
            index = int(entry[1])
            arg = str(args[index])
            buffer = re.sub(searchString, arg, buffer, 1)
        
    
    return buffer

