# 
# xmlrpc-test.py
#
# Test the Tesla XML-RPC gateway (instrument controller)
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

import xmlrpc.client 
import socket

# -----------------------------------------------------------------------------
# Constants that should not need to change too often (not soft-setups)

GATEWAY_HOST = 'localhost'                # Default gateway host
GATEWAY_PORT = 8000                        # Default gateway port

# -----------------------------------------------------------------------------

if __name__ == '__main__':
    host = "http://%s:%d" % (GATEWAY_HOST, GATEWAY_PORT)

    try:
        srv = xmlrpc.client.ServerProxy(host)

        print("\nAttempting to connect to %s\n" % (host))

        serverIsAlive = srv.ping()
        if serverIsAlive:

            serverInfo = srv.getServerInfo()
            print("  Gateway address:", serverInfo[0])
            print("   Server version:", serverInfo[1])
            print("    Server uptime:", serverInfo[2])

            hostInfo = srv.getHostConfiguration()
            for key in list(hostInfo.keys()):
                print("%17s: %s" % (key, hostInfo[key]))

            print()  
            print("Instrument status:", srv.getInstrumentStatus())
            print(" Instrument state:", srv.getInstrumentState())
            print("        Error log:", srv.getErrorLog())

            subscribers = srv.getSubscriberList()
            print("      Subscribers:", end=' ') 
            if len(subscribers):
                print(subscribers)
            else:
                print("None")

            try:
                protocolInfo = srv.getProtocols()
            except xmlrpc.client.Error as msg:
                print("ERROR -- UNABLE TO getProtocols()", msg)
                protocolInfo = ()

            print("        Protocols:")
            for protocol in protocolInfo:
                print("\t\t%s" % (protocol['label']))
                for key in protocol:
                    if key == 'label':
                        continue
                    else:
                        print("\t\t\t%15s: %s" % (key, protocol[key]))
                print()

            print("State:",srv.getInstrumentState())

            #etc = srv.startRun([])
            #print "Started run: ETC =", etc

            print("State:",srv.getInstrumentState())

            print("Halting...", srv.halt())

        else:
            print("Can not connect to %s" % (host))

    except socket.error as msg:
        print("Unable to connect to XML-RPC server (%s)" % (msg))

    except xmlrpc.client.Fault as msg:
        print("XML-RPC error (%s). Halting." % (msg))
        if srv.ping():
           srv.halt()

# eof

