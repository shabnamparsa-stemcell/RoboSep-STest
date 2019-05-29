# File: c (Python 2.3)

__doc__ = '\nBasic currying support via a curry() function.\n\nExample of use:\n\ndef bar(x, y, z=42):\n    return x*3 + y*2 + z\n\nb = curry(bar, 1)\nprint b(2, z=3)\n'

curry = lambda f, *args, **kwargs: lambda : f(*args + cargs, **args + cargs)

