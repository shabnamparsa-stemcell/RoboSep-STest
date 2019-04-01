# File: B (Python 2.3)


class Bunch:
    '''
    Dictionaries are fine for collecting a small bunch of stuff, each item 
    with a name; however, when names are constants and to be used just like
    variables, the dictionary-access syntax ("if bunch[\'squared\'] > threshold",
    etc) is not maximally clear; it takes VERY little effort to build a 
    little class, as in the \'Bunch\' example above, that will both ease the 
    initialization task _and_ provide elegant attribute-access syntax 
    ("if bunch.squared > threshold", etc).

    It would not be hard to add __getitem__, __setitem__ and __delitem__ 
    methods to allow attributes to *also* be accessed as bunch[\'squared\'] etc
    -- they would just have to delegate to self.__dict__, e.g. via the 
    handy functions getitem, setitem, delitem in module operator. This would 
    mimic javascript use, where attributes and items are regularly confused; 
    such unPythonic idioms, however, seem to be completely useless in Python. 
    For occasional access to an attribute whose name is held in a variable 
    (or otherwise runtime-computed), builtin functions getattr, setattr and 
    delattr are perfectly adequate, and definitely preferable to complicating 
    the delightfully-simple Bunch class.

    Example:

        # Now, you can create a Bunch whenever you want to group a few variables:
        point = Bunch(datum=y, squared=y*y, coord=x)

        # and of course you can read/write the named
        # attributes you just created, add others, del
        # some of them, etc, etc:
        if point.squared > threshold:
            point.isok = 1
    '''
    
    def __init__(self, **kwds):
        self.__dict__.update(kwds)


