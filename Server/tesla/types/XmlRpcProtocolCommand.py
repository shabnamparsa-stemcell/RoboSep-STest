# 
# XmlRpcProtocolCommand.py
# tesla.types.XmlRpcProtocolCommand

from tesla.exception import TeslaException

class XmlRpcProtocolCommand:
    
    def __init__(self, cmd = "", time = 0, description = ""):
        self.cmd           = cmd
        self.time     = time
        self.description     = description

    def __repr__(self):
        '''String representation of the protocol consumables'''
        return "XmlRpcProtocolCommand %s %d %s" % (self.cmd, self.time, self.description)

# eof

