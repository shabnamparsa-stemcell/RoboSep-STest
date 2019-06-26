# 
# config.py
# tesla.config.py
#
# Fundamental configuration data for the Tesla control software
# NOTE: There is some executable code in here (where we want to adapt/change
#       configuration data to the environment)
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

import os, sys, socket
import logging

import time         # 2011-11-24 sp -- added for creating log filename prefix
from configparser import *          # 2012-01-30 sp -- configuration from ini file

# -----------------------------------------------------------------------------

# The version of the instrument control server software
# Changing this affects the version number for the installation package (via
# robosep.iss)   .... 338 represents a repair to lysis proportional volume
SOFTWARE_VERSION = '1.3.0.0'

# -----------------------------------------------------------------------------
# Configuration meta-data; don't touch anything in this section unless you
# really know what you are doing :)

PLATFORM = sys.platform 

# Configuration values for Windows
BASE_DIR32  = os.path.join('C:\\', 'Program Files', 'STI', 'RoboSep')
BASE_DIR64  = os.path.join('C:\\', 'Program Files (x86)', 'STI', 'RoboSep')
BASE_DIR    = BASE_DIR32

# -----------------------------------------------------------------------------
# Support functions

def getGatewayHost():
    # If the TESLA_FQDN environment variable is set, use it. Otherwise, try to 
    # resolve the FQDN (fully qualified domain name). If that fails, just fall back
    # to the DEFAULT_GATEWAY_HOST, which is "localhost" by default
    if 'TESLA_FQDN' in os.environ:
        host = os.environ['TESLA_FQDN']
    else:
        try:
            # Try to get a fully qualified domain name so that we can serve the
            # XML-RPC interface across the network (eg. for the service app)
            host = socket.getfqdn(socket.gethostname())
        except socket.error:
            # If that fails, resort to just using localhost
            host = DEFAULT_GATEWAY_HOST
    return host


# -----------------------------------------------------------------------------
# General instrument configuration data: this should not change (often)

# Name of the Tesla Python archive that will be imported
TESLA_ARCHIVE = 'robosep.zip'            

# XML-RPC server configuration
DEFAULT_GATEWAY_HOST = 'localhost'        # Default control gateway host
GATEWAY_HOST = getGatewayHost()         # Get the actual gateway host
GATEWAY_PORT = 8000                        # Default control gateway port

# Minimum disk space for safe operation, in bytes
MIN_SAFE_DISK_SPACE = 10 * 1024 * 1024        # Let's say 10 MB?

# What level do we want to log at?
LOG_LEVEL = logging.DEBUG

# -----------------------------------------------------------------------------
# Location of key directories and files
# 
# Note: * Constants for directory locations end in _PATH
#       * Constants for full file paths end in _PATH

# Location of configuration files
CONFIG_DIR = None

# Location of protocol definition files
PROTOCOL_DIR = None

# Location of the log files
LOG_DIR   = None
LOG_PATH  = None

# Location of the autocal log files
CAL_LOG_DIR   = None
CAL_LOG_PATH  = None

# Location of the command times definition file
CMD_TIME_PATH = None

# Location of the hardware configuration file and the hardware component 
# tracking database
HW_CFG_PATH = None
HW_MISC_PATH = None
CFG_BACKUP_DIR = None
HW_BACK_PATH = None

COMPONENT_DB_PATH = None

# Location of generated sample reports
SAMPLE_REPORT_DIR = None
SAMPLE_REPORT_NAME = 'report.log'

# Location of shared data files
STATUS_CODES_PATH = None
ERROR_CODES_PATH  = None
VIAL_CODES_PATH   = None

# Location of the instrument serial number file
SERIAL_PATH = None

def UpdatePathes():
    global CONFIG_DIR;
    CONFIG_DIR        = os.path.join(BASE_DIR, 'config')      
    global PROTOCOL_DIR;
    PROTOCOL_DIR      = os.path.join(BASE_DIR, 'protocols')    
    global LOG_DIR;
    LOG_DIR           = os.path.join(BASE_DIR, 'logs')
    global LOG_PATH;
    LOG_PATH          = os.path.join(LOG_DIR, 'robosep.log')    
    global CAL_LOG_DIR;
    CAL_LOG_DIR       = os.path.join(BASE_DIR, 'logs', 'calibration')    
    global CAL_LOG_PATH;
    CAL_LOG_PATH      = os.path.join(LOG_DIR, 'calibration.log')
    global SERVER_CONFIG_PATH;
    SERVER_CONFIG_PATH       = os.path.join(CONFIG_DIR, 'serverConfig.ini')
    global CMD_TIME_PATH;
    CMD_TIME_PATH     = os.path.join(CONFIG_DIR, 'cmd_times.ini')     
    global HW_CFG_PATH;
    HW_CFG_PATH       = os.path.join(CONFIG_DIR, 'hardware.ini')    
    global HW_MISC_PATH;
    HW_MISC_PATH       = os.path.join(CONFIG_DIR, 'hardware_misc.ini')
    global CFG_BACKUP_DIR;
    CFG_BACKUP_DIR    = os.path.join(CONFIG_DIR, 'hwbackup')
    global HW_BACK_PATH;
    HW_BACK_PATH      = os.path.join(CFG_BACKUP_DIR, 'hardware.ini')
    global COMPONENT_DB_PATH;
    COMPONENT_DB_PATH = os.path.join(BASE_DIR, 'component.dbm')    
    global SAMPLE_REPORT_DIR;
    SAMPLE_REPORT_DIR = os.path.join(BASE_DIR, 'reports')
    global STATUS_CODES_PATH;
    STATUS_CODES_PATH = os.path.join(CONFIG_DIR, 'statusConfig.txt')
    global ERROR_CODES_PATH;
    ERROR_CODES_PATH  = os.path.join(CONFIG_DIR, 'errorConfig.txt')
    global VIAL_CODES_PATH;
    VIAL_CODES_PATH   = os.path.join(CONFIG_DIR, 'protocolConfig.txt')
    global SERIAL_PATH;
    SERIAL_PATH       = os.path.join(CONFIG_DIR, 'serial.dat')

# 2012-01-30 sp -- move usage of environment variables into serveConfig.ini file
def GetConfigEnvironment():
    global SS_STEP;
    global SS_STEP_POPUP;
    global SS_STEP_BEEP;
    global SS_LOG_X0;
    global SS_LOG_X1;
    global SS_LOG_Y0;
    global SS_LOG_Y1;
    global SS_CMDLIST;
    global SS_DUMP;
    global SS_LOG_SILENT;
    global SS_DUMP_DISABLETIME;
    global SS_DISABLE_STRIP;
    global SS_LEGACY;
    global SS_DEBUG;
    global SS_TRACE;
    global SS_INFO;
    global SS_FORCE_EMULATION;
    global SS_MINNIE_DEV;
    global SS_ORCA;
    global SS_DRYRUN;
    global SS_CHATTER;
    global SS_ASPIRATEDOWN;
    global SS_DISPENSEDOWN;
    global SS_OLDPCB;
    global SS_DEBUGGER_LOG;
    global SS_EXT_LOGGER;
    global OVERWRITE_WITH_BARCODES;
    global SS_TIMEOUT;
    global SS_CUSTOM_HOME;      #CWJ Add 2015-08-06
    global SS_USE460_PUMPHOMING;#CWJ Add 2015-08-06
    global SS_BAD_H_TEST;       #CWJ Add 2015-08-06
    global SS_ENABLE_STRIP_ARM_POSITION_CHECK;
    global SS_XY_MICRO_LC;
    
    'Debugger parameters'
    global CarouselHomeSwitch
    global PowerOff
    global ZPowerOff
    
    global ZAxisPowerProfile
    global PumpPowerProfile
    global ThetaArmPowerProfile
    global CarouselPowerProfile
    
    # set default values
    SS_STEP               = 0;
    SS_STEP_POPUP         = 0;
    SS_STEP_BEEP          = 0;
    SS_LOG_X0             = 0;
    SS_LOG_X1             = 0;
    SS_LOG_Y0             = 0;
    SS_LOG_Y1             = 0;
    SS_CMDLIST            = 0;
    SS_DUMP               = 0;
    SS_LOG_SILENT         = 0;
    SS_DUMP_DISABLETIME   = 0;
    SS_DISABLE_STRIP      = 0;
    SS_LEGACY             = 0;
    SS_DEBUG              = 0;
    SS_TRACE              = 0;
    SS_INFO               = 0;
    SS_FORCE_EMULATION    = 0;
    SS_MINNIE_DEV         = 1;
    SS_ORCA               = 1;
    SS_DRYRUN             = 0;
    SS_CHATTER            = 0;
    SS_ASPIRATEDOWN       = 0;
    SS_DISPENSEDOWN       = 0;
    SS_OLDPCB             = 0;
    SS_DEBUGGER_LOG       = 0;
    SS_EXT_LOGGER           = 1;
    OVERWRITE_WITH_BARCODES = 1;
    SS_TIMEOUT              = 0;

    SS_CUSTOM_HOME          = 0; #CWJ Add 2015-08-06
    SS_USE460_PUMPHOMING    = 0; #CWJ Add 2015-08-06
    SS_BAD_H_TEST           = 0; #CWJ Add 2015-08-06
    
    SS_ENABLE_STRIP_ARM_POSITION_CHECK = 1;
    SS_XY_MICRO_LC = 0;

    CarouselHomeSwitch = 0

    PowerOff = 'P1,0,0'
    ZPowerOff = 'P20,20,0'

    ZAxisPowerProfile    = 'homingpowerprofile'
    PumpPowerProfile     = 'homingpowerprofile'
    ThetaArmPowerProfile = 'homingpowerprofile'
    CarouselPowerProfile = 'homingpowerprofile'

    cfg = ConfigParser()

    global configEnvironmentMsg;
    global configEnvironmentSettingMsg;
    # if configuration file exists, extract the settings

    configEnvironmentMsg = ''
    configEnvironmentSettingMsg =''

    if( os.path.exists( SERVER_CONFIG_PATH)):
        cfg.read( SERVER_CONFIG_PATH )
        try:
            # get settings from configuration file
            SS_STEP               = int( cfg.get( 'environment', 'SS_STEP' ) )
            SS_STEP_POPUP         = int( cfg.get( 'environment', 'SS_STEP_POPUP' ) )
            SS_STEP_BEEP          = int( cfg.get( 'environment', 'SS_STEP_BEEP' ) )
            SS_LOG_X0             = int( cfg.get( 'environment', 'SS_LOG_X0' ) )
            SS_LOG_X1             = int( cfg.get( 'environment', 'SS_LOG_X1' ) )
            SS_LOG_Y0             = int( cfg.get( 'environment', 'SS_LOG_Y0' ) )
            SS_LOG_Y1             = int( cfg.get( 'environment', 'SS_LOG_Y1' ) )
            SS_CMDLIST            = int( cfg.get( 'environment', 'SS_CMDLIST' ) )
            SS_DUMP               = int( cfg.get( 'environment', 'SS_DUMP' ) )
            SS_STEP_BEEP          = int( cfg.get( 'environment', 'SS_STEP_BEEP' ) )
            SS_LOG_SILENT         = int( cfg.get( 'environment', 'SS_LOG_SILENT' ) )
            SS_DUMP_DISABLETIME   = int( cfg.get( 'environment', 'SS_DUMP_DISABLETIME' ) )
            SS_DISABLE_STRIP      = int( cfg.get( 'environment', 'SS_DISABLE_STRIP' ) )
            SS_LEGACY             = int( cfg.get( 'environment', 'SS_LEGACY' ) )
            SS_DEBUG              = int( cfg.get( 'environment', 'SS_DEBUG' ) )
            SS_TRACE              = int( cfg.get( 'environment', 'SS_TRACE' ) )
            SS_INFO               = int( cfg.get( 'environment', 'SS_INFO' ) )
            SS_FORCE_EMULATION    = int( cfg.get( 'environment', 'SS_FORCE_EMULATION' ) )
            SS_MINNIE_DEV         = int( cfg.get( 'environment', 'SS_MINNIE_DEV' ) )
            SS_ORCA               = int( cfg.get( 'environment', 'SS_ORCA' ) )
            SS_DRYRUN             = int( cfg.get( 'environment', 'SS_DRYRUN' ) )
            SS_CHATTER            = int( cfg.get( 'environment', 'SS_CHATTER' ) )
            SS_ASPIRATEDOWN       = int( cfg.get( 'environment', 'SS_ASPIRATEDOWN' ) )
            SS_DISPENSEDOWN       = int( cfg.get( 'environment', 'SS_DISPENSEDOWN' ) )
            # SS_OLDPCB             = int( cfg.get( 'environment', 'SS_OLDPCB' ) )
            SS_DEBUGGER_LOG       = int( cfg.get( 'environment', 'SS_DEBUGGER_LOG' ) )
            SS_EXT_LOGGER         = int( cfg.get( 'environment', 'SS_EXT_LOGGER' ) )
            OVERWRITE_WITH_BARCODES = int( cfg.get( 'environment', 'OVERWRITE_WITH_BARCODES' ) )
            SS_TIMEOUT            = int(cfg.get( 'environment', 'SS_TIMEOUT' ) )
            SS_CUSTOM_HOME        = int(cfg.get( 'environment', 'SS_CUSTOM_HOME' ) )
            SS_USE460_PUMPHOMING  = int(cfg.get( 'environment', 'SS_USE460_PUMPHOMING' ) )
            SS_BAD_H_TEST         = int(cfg.get( 'environment', 'SS_BAD_H_TEST' ) )
            SS_ENABLE_STRIP_ARM_POSITION_CHECK = int( cfg.get( 'environment', 'SS_ENABLE_STRIP_ARM_POSITION_CHECK' ) )
            #SS_XY_MICRO_LC = int( cfg.get( 'environment', 'SS_XY_MICRO_LC' ) )
            
            CarouselHomeSwitch    = int(cfg.get( 'debugger', 'CarouselHomeSwitch'))
            PowerOff              = str(cfg.get( 'debugger', 'PowerOff'))
            ZPowerOff             = str(cfg.get( 'debugger', 'ZPowerOff'))

            ZAxisPowerProfile    = str(cfg.get( 'debugger', 'ZAxisPowerProfile'))
            PumpPowerProfile     = str(cfg.get( 'debugger', 'PumpPowerProfile'))
            ThetaArmPowerProfile = str(cfg.get( 'debugger', 'ThetaArmPowerProfile'))
            CarouselPowerProfile = str(cfg.get( 'debugger', 'CarouselPowerProfile'))

            # generate log message of success, report file and version number
            configEnvironmentMsg = 'Loaded environment settings from configuration file.'
            configEnvironmentSettingMsg = 'Settings| SS_STEP=%d' % SS_STEP
            configEnvironmentSettingMsg += '| SS_STEP_POPUP=%d' % SS_STEP_POPUP
            configEnvironmentSettingMsg += '| SS_STEP_BEEP=%d' % SS_STEP_BEEP
            configEnvironmentSettingMsg += '| SS_LOG_X0=%d' % SS_LOG_X0
            configEnvironmentSettingMsg += '| SS_LOG_X1=%d' % SS_LOG_X1
            configEnvironmentSettingMsg += '| SS_LOG_Y0=%d' % SS_LOG_Y0
            configEnvironmentSettingMsg += '| SS_LOG_Y1=%d' % SS_LOG_Y1
            configEnvironmentSettingMsg += '| SS_CMDLIST=%d' % SS_CMDLIST
            configEnvironmentSettingMsg += '| SS_DUMP=%d' % SS_DUMP
            configEnvironmentSettingMsg += '| SS_STEP_BEEP=%d' % SS_STEP_BEEP
            configEnvironmentSettingMsg += '| SS_LOG_SILENT=%d' % SS_LOG_SILENT
            configEnvironmentSettingMsg += '| SS_DUMP_DISABLETIME=%d' % SS_DUMP_DISABLETIME
            configEnvironmentSettingMsg += '| SS_DISABLE_STRIP=%d' % SS_DISABLE_STRIP
            configEnvironmentSettingMsg += '| SS_LEGACY=%d' % SS_LEGACY
            configEnvironmentSettingMsg += '| SS_DEBUG=%d' % SS_DEBUG
            configEnvironmentSettingMsg += '| SS_TRACE=%d' % SS_TRACE
            configEnvironmentSettingMsg += '| SS_INFO=%d' % SS_INFO
            configEnvironmentSettingMsg += '| SS_FORCE_EMULATION=%d' % SS_FORCE_EMULATION
            configEnvironmentSettingMsg += '| SS_MINNIE_DEV=%d' % SS_MINNIE_DEV
            configEnvironmentSettingMsg += '| SS_ORCA=%d' % SS_ORCA
            configEnvironmentSettingMsg += '| SS_DRYRUN=%d' % SS_DRYRUN
            configEnvironmentSettingMsg += '| SS_CHATTER=%d' % SS_CHATTER
            configEnvironmentSettingMsg += '| SS_ASPIRATEDOWN=%d' % SS_ASPIRATEDOWN
            configEnvironmentSettingMsg += '| SS_DISPENSEDOWN=%d' % SS_DISPENSEDOWN
#            configEnvironmentSettingMsg += '| SS_OLDPCB=%d' % SS_OLDPCB 
            configEnvironmentSettingMsg += '| SS_DEBUGGER_LOG=%d' % SS_DEBUGGER_LOG
            configEnvironmentSettingMsg += '| SS_EXT_LOGGER=%d' % SS_EXT_LOGGER
            configEnvironmentSettingMsg += '| OVERWRITE_WITH_BARCODES=%d' % OVERWRITE_WITH_BARCODES
            configEnvironmentSettingMsg += '| SS_TIMEOUT=%d' % SS_TIMEOUT
            configEnvironmentSettingMsg += '| SS_CUSTOM_HOME=%d' % SS_CUSTOM_HOME
            configEnvironmentSettingMsg += '| SS_USE460_PUMPHOMING=%d' % SS_USE460_PUMPHOMING
            configEnvironmentSettingMsg += '| SS_BAD_H_TEST=%d' % SS_BAD_H_TEST
            configEnvironmentSettingMsg += '| SS_ENABLE_STRIP_ARM_POSITION_CHECK=%d' % SS_ENABLE_STRIP_ARM_POSITION_CHECK
#            configEnvironmentSettingMsg += '| SS_XY_MICRO_LC=%d' % SS_XY_MICRO_LC
            
#        except Exception, msg:
        except Exception:
            configEnvironmentMsg = 'Error reading from configuration file [%s]...Default settings used: ' % (SERVER_CONFIG_PATH)
    else:
        configEnvironmentMsg = 'Error reading from configuration file [%s]...Default settings used: ' % (SERVER_CONFIG_PATH)
    
# -----------------------------------------------------------------------------
# Various protocol constants that should not change

NUM_QUADRANTS = 4                    # How many quadrants on the carousel?

POSITIVE_PROTOCOL    = 'Positive'   # Unique ID for positive separation protocols
NEGATIVE_PROTOCOL    = 'Negative'   # Unique ID for negative separation protocols
HUMANPOSITIVE_PROTOCOL    = 'HumanPositive'   # Unique ID for HumanPositive separation protocols#CR
HUMANNEGATIVE_PROTOCOL    = 'HumanNegative'   # Unique ID for HumanNegative separation protocols#CR
MOUSEPOSITIVE_PROTOCOL    = 'MousePositive'   # Unique ID for MousePositive separation protocols#CR
MOUSENEGATIVE_PROTOCOL    = 'MouseNegative'   # Unique ID for MouseNegative separation protocols#CR
WHOLEBLOODPOSITIVE_PROTOCOL    = 'WholeBloodPositive'   # Unique ID for WholeBloodPositive separation protocols#CR
WHOLEBLOODNEGATIVE_PROTOCOL    = 'WholeBloodNegative'   # Unique ID for WholeBloodPositive separation protocols
MAINTENANCE_PROTOCOL = 'Maintenance'        # Unique ID for maintenance protocols
SHUTDOWN_PROTOCOL    = 'Shutdown'   # Unique ID for shutdown protocols
TEST_PROTOCOL        = 'Testing'    # Unique ID for testing protocols

# Volume constraints
DEFAULT_WORKING_VOLUME_uL = 10000   # The default working volume (in uL)

# -----------------------------------------------------------------------------
# Error handling

WARNING_LEVEL, ERROR_LEVEL = (1,2)  # Our warning and error level values

# -----------------------------------------------------------------------------
# Shutdown control
#
# If the TESLA_FULL_SHUTDOWN environment variable is defined and set to zero,
# we will not do a full shutdown of the ICS and PC, otherwise we do a full
# shutdown
FULL_SHUTDOWN = (os.environ.get('TESLA_FULL_SHUTDOWN', '1') != '0')
FULL_SHUTDOWN_IN_PROGRESS = False;

# -----------------------------------------------------------------------------
# Debugging or testing flags

# Enable XML-RPC debugging (both ways)
XMLRPC_DEBUG = False or 'TESLA_XMLRPC_DEBUG' in os.environ

# Enable debugging of dispatch events
DISPATCH_DEBUG = False or 'TESLA_DISPATCH_DEBUG' in os.environ

# Enable debugging of the scheduling subsystem
SCHEDULER_DEBUG = False or 'TESLA_SCHEDULER_DEBUG' in os.environ

# Enable instrument/hardware layer debugging
HW_DEBUG = False or 'TESLA_HW_DEBUG' in os.environ

TIME_START = None;

# eof

