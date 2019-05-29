# File: I (Python 2.3)


class InfoTable(object):
    '''A class for handling tables of information, where each line of the table
    file is code=message.'''
    
    def __init__(self, path):
        '''Construct with a path pointing to the info table file.'''
        self.path = path
        self._InfoTable__codes = { }
        self._InfoTable__readTable()

    
    def _InfoTable__readTable(self):
        '''Read in the information table and process for getCode() to use.'''
        
        try:
            f = open(self.path, 'r')
            data = f.readlines()
            for line in data:
                if line[0] == '#':
                    continue
                    continue
                if line.count('='):
                    (key, value) = line.split('=', 1)
                    key = key.strip()
                    if self._InfoTable__codes.has_key(key):
                        raise KeyError, "%s is a duplicate key in line '%s'" % (key, line)
                    
                    self._InfoTable__codes[key] = value.strip()
                    continue
            
            f.close()
        except:
            raise IOError, 'Unable to construct InfoTable from %s' % self.path


    
    def getMessage(self, code):
        '''Return the error message for the specified code.'''
        return self._InfoTable__codes[code]

    
    def getCode(self, msg):
        '''Return the code for a specific message.'''
        for (key, value) in self._InfoTable__codes.items():
            if msg == value:
                return key
                continue
        else:
            raise LookupError, 'Unable to find code for %s message' % msg

    
    def getCodes(self):
        '''Return a list of all the codes in this table.'''
        return self._InfoTable__codes.keys()


