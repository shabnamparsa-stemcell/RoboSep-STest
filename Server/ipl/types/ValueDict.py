# File: V (Python 2.3)

__all__ = [
    'ValueDict']

class ValueDict(dict):
    
    def getFirstKey(self, value):
        """Return the first key that has the specified value, returns None
\tif the value can't be found."""
        keys = self.getKeys(value)
        if len(keys) > 0:
            return keys[0]
        else:
            return None

    
    def getKeys(self, value):
        """Return a sorted list of keys that have the specified value. Returns an
\tempty list if the value isn't in the dictionary."""
        keys = []
        for (key, keyedValue) in self.items():
            if keyedValue == value:
                keys.append(key)
                continue
        
        keys.sort()
        return keys


