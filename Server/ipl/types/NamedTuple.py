# File: N (Python 2.3)


class NamedTupleMetaclass(type):
    """Metaclass for a tuple with elements named and indexed.
    
    NamedTupleMetaclass instances must set the 'names' class attribute
    with a list of strings of valid identifiers, being the names for the
    elements. The elements can then be obtained by looking up the name or the index.
    """
    
    def __init__(cls, classname, bases, classdict):
        if not (tuple in bases):
            raise ValueError, "'%s' must derive from tuple type." % classname
        
        type.__init__(cls, classname, bases, classdict)
        
        def instance_setattr(self, name, value):
            raise TypeError, "'%s' object has only read-only attributes (assign to .%s)" % (self.__class__.__name__, name)

        cls.__setattr__ = instance_setattr
        
        def cls_new(cls, seq_or_dict):
            if isinstance(seq_or_dict, dict):
                seq = []
                for name in cls.names:
                    
                    try:
                        seq.append(seq_or_dict[name])
                    continue
                    except KeyError:
                        raise KeyError, "'%s' element of '%s' not given" % (name, cls.__name__)
                        continue
                    

                
            else:
                seq = seq_or_dict
            instance = tuple.__new__(cls, seq)
            for (i, name) in enumerate(cls.names):
                tuple.__setattr__(instance, name, seq[i])
            
            return instance

        cls.__new__ = staticmethod(cls_new)



def NamedTuple(*namelist):
    '''Class factory function for creating named tuples.
    A common usage of tuples is to represent aggregations of heterogenous data,
    as a kind of anonymous data structure. The built-in tuple type only allows
    the elements to be accessed by their indices; this leads to a loss of 
    clarity: seeing x[3] in code is less clear than x.middlename, for example.

    Classes using NamedTupleMetaclass are tuples with named elements. These 
    elements can then be accessed by index or by name, as convenient. See the 
    code for some example usage.

    Because NamedTuple is a subclass of tuple, all the standard tuple methods 
    will work on it as usual. This provides the convenience of tuples with the 
    clarity of named elements.

    An example:

        PersonTuple = NamedTuple("name", "age", "height")
        person1 = PersonTuple(["James", "25", "185"])
        print person1.name, person1.age, person1.height
    '''
    
    class _NamedTuple(tuple):
        __metaclass__ = NamedTupleMetaclass
        names = list(namelist)

    return _NamedTuple

