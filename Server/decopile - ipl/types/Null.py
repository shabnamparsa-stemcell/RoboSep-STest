# File: N (Python 2.3)


class Null:
    ''' Null objects always and reliably "do nothing."

    An instance of the Null class can replace the primitive value None. Using 
    this class, you can avoid many conditional statements in your code and can 
    often express algorithms with little or no checking for special values. 
    This recipe is a sample implementation of the Null Object design pattern 
    (see "The Null Object Pattern", B. Woolf, Pattern Languages of Programming, 
    PLoP 96, September 1996). 

    This recipe\'s Null class ignores all parameters passed when constructing or 
    calling instances and any attempt to set or delete attributes. Any call or 
    attempt to access an attribute (or a method, since Python does not 
    distinguish between the two and calls __getattr__ either way) returns the 
    same Null instance (i.e., self, since there\'s no reason to create a new 
    one). For example, if you have a computation such as: 

    def compute(x, y):
        try: "lots of computation here to return some appropriate object"
        except SomeError: return None

    and you use it like this:

    for x in xs:
        for y in ys:
            obj = compute(x, y)
            if obj is not None:
                obj.somemethod(y, x)

    you can usefully change the computation to:

    def compute(x, y):
        try: "lots of computation here to return some appropriate object"
        except SomeError: return Null(  )

    and thus simplify it as:

    for x in xs:
        for y in ys:
            compute(x, y).somemethod(y, x)

    Thus, you don\'t need to check whether compute has returned a real result or
    an instance of Null. Even in the latter case, you can safely and 
    innocuously call on it whatever method you want. 

    Python calls __getattr__ for special methods as well. This means that you 
    may have to take some care and customize Null to your application\'s needs, 
    either directly in the class\'s sources or by subclassing it appropriately. 
    For example, with this recipe\'s Null, any comparison between Null instances, 
    even a==a, returns a Null instance and evaluates as false. (Note that we\'ve 
    had to explicitly define __nonzero__ for this purpose, since __nonzero__ 
    must return an int.) If this is a problem for your purposes, you must define 
    __eq__ (in Null itself or in an appropriate subclass) and implement it 
    appropriately. Similar delicate considerations apply to several other special 
    methods. 

    The goal of Null objects is to provide an intelligent replacement for the 
    often-used primitive value None in Python (Null or null pointers in other 
    languages). These "nobody lives here" markers are used for many purposes, 
    including the important case in which one member of a group of otherwise 
    similar elements is special. Usually, this usage results in conditional 
    statements to distinguish between ordinary elements and the primitive null 
    (e.g., None) value, but Null objects help you avoid that. 

    Among the advantages of using Null objects are the following: 

    * Superfluous conditional statements can be avoided by providing a 
      first-class object alternative for the primitive value None, thereby 
      improving code readability. 

    * They can act as a placeholder for objects whose behavior is not yet 
      implemented. 

    * They can be used polymorphically with instances of any other class.

    * They are very predictable.

    To cope with the disadvantage of creating large numbers of passive objects 
    that do nothing but occupy memory space, the Null Object pattern is often 
    combined with the Singleton pattern (see Recipe 5.22), but this recipe does
    not explore that combination.
    '''
    
    def __init__(self, *args, **kwargs):
        pass

    
    def __call__(self, *args, **kwargs):
        return self

    
    def __repr__(self):
        return 'Null()'

    
    def __nonzero__(self):
        return 0

    
    def __getattr__(self, name):
        return self

    
    def __setattr__(self, name, value):
        return self

    
    def __delattr__(self, name):
        return self


