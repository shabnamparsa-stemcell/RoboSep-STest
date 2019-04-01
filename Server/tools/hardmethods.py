# 
# hardmethods.py
#
# Dump out all the methods for each device and subsystem in the Tesla 
# hardware layer
# 
# Copyright (c) Invetech Pty Ltd, 2004
# 495 Blackburn Rd
# Mt Waverley, Vic, Australia.
# Phone (+61 3) 9211 7700
# Fax   (+61 3) 9211 7701
# 
# The copyright to the computer program(s) herein is the
# property of Invetech Pty Ltd, Australia.
# The program(s) may be used and/or copied only with
# the written permission of Invetech Pty Ltd
# or in accordance with the terms and conditions
# stipulated in the agreement/contract under which
# the program(s) have been supplied.
# 

import inspect

from tesla.instrument.Instrument import Instrument
from tesla.hardware.Device import Device

def printMethods(out, hwObject):
    out.write("\n\t<h3>%s</h3>\n" % (str(hwObject).split(': ')[1]))
    methodList = inspect.getmembers(hwObject, inspect.ismethod)
    out.write("\t<ul>\n")
    for methodName, methodInstance in methodList:
        if methodName[0] == '_':
            continue
        args = str(inspect.getargspec(methodInstance)[0]).replace("'", '').replace('[', '').replace(']', '')
        out.write("\t\t<li>%s(%s)\n" % (methodName, args))
    out.write("\t</ul>\n")


if __name__ == '__main__':
    inst = Instrument('Instrument')

    f = open('tesla_hw_methods.html', 'w')

    f.write("<html><head><title>Tesla hardware methods</title></head>\n")
    f.write("<body><h1 align='center'>Tesla hardware methods</h1>\n")

    f.write("<h2>Subsystems</h2>\n")
    for subsystem in inst.instanceList:
        printMethods(f, subsystem)
        
    f.write("<hr>\n")
    f.write("<h2>Devices</h2>\n")
    for device in Device.deviceList:
        printMethods(f, device)
    
    f.write("</body></html>\n")

    f.close()
    
