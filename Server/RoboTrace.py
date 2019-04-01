from wxPython.wx import *
from re import *
import threading
import sys, os
import glob
import time
import datetime
import string
import shutil

import tesla.config

import SimpleStep.SimpleStep  #CWJ Add
import os, sys;
import win32gui, win32con, win32api, win32clipboard

from tesla.PgmLog import PgmLog     # 2012-01-04 sp -- programming logging
from ConfigParser import *          # 2012-01-04 sp -- configuration from ini file

import ctypes;

ID_FRAME      = wxNewId()
ID_BUTTON     = wxNewId()
ID_PANEL      = wxNewId()
ID_TIMER      = wxNewId()
ID_PWD        = wxNewId()
ID_TITLE      = wxNewId()
ID_CAPT_AXIS  = wxNewId()
ID_AXIS_X0    = wxNewId()
ID_AXIS_X1    = wxNewId()
ID_AXIS_Y0    = wxNewId()
ID_AXIS_Y1    = wxNewId()
ID_CMDTYPE    = wxNewId()
ID_DUMP       = wxNewId()
ID_DUMPNOTIME = wxNewId()
ID_DUMPNODISP = wxNewId()
ID_STEP       = wxNewId()
ID_STEPBEEP   = wxNewId()
ID_STEPPOPUP  = wxNewId()
ID_DUMPBUTTON = wxNewId()
ID_BMP        = wxNewId()

ORIG_X_FRAME      = 200
ORIG_Y_FRAME      = 100
ORIG_X_BUTTON     = 360
ORIG_Y_BUTTON     = 242
ORIG_X_PWD        = 10
ORIG_Y_PWD        = 242
ORIG_X_TITLE      = 20
ORIG_Y_TITLE      = 80
ORIG_X_CAPTAXIS   = 10
ORIG_Y_CAPTAXIS   = 10
ORIG_X_CHKX0      = 80
ORIG_Y_CHKX0      = 8
ORIG_X_CHKX1      = 130
ORIG_Y_CHKX1      = 8
ORIG_X_CHKY0      = 180
ORIG_Y_CHKY0      = 8
ORIG_X_CHKY1      = 230
ORIG_Y_CHKY1      = 8
ORIG_X_CMBCMD     = 10
ORIG_Y_CMBCMD     = 30
ORIG_X_DUMP       = 10
ORIG_Y_DUMP       = 70
ORIG_X_DUMPNOTIME = 140
ORIG_Y_DUMPNOTIME = 70
ORIG_X_DUMPNODISP = 280
ORIG_Y_DUMPNODISP = 70
ORIG_X_STEP       = 10
ORIG_Y_STEP       = 90
ORIG_X_STEPBEEP   = 140
ORIG_Y_STEPBEEP   = 90
ORIG_X_STEPPOPUP  = 280
ORIG_Y_STEPPOPUP  = 90
ORIG_X_DUMPBUTTON = 240
ORIG_Y_DUMPBUTTON = 40
ORIG_X_WALLPAPER  = 10
ORIG_Y_WALLPAPER  = 0

SIZE_X_FRAME      = 430
SIZE_Y_FRAME      = 300
SIZE_X_PANEL      = SIZE_X_FRAME
SIZE_Y_PANEL      = SIZE_Y_FRAME
SIZE_X_BUTTON     = 55
SIZE_Y_BUTTON     = 25
SIZE_X_PWD        = 340
SIZE_Y_PWD        = 25
SIZE_X_AXIS       = 40
SIZE_Y_AXIS       = 20
SIZE_X_CMD        = 200
SIZE_Y_CMD        = 40
SIZE_X_DUMP       = 120
SIZE_Y_DUMP       = 20
SIZE_X_DUMPNOTIME = 140
SIZE_Y_DUMPNOTIME = 20
SIZE_X_DUMPNODISP = 140
SIZE_Y_DUMPNODISP = 20
SIZE_X_STEP       = 120
SIZE_Y_STEP       = 20
SIZE_X_STEPBEEP   = 140
SIZE_Y_STEPBEEP   = 20
SIZE_X_STEPPOPUP  = 140
SIZE_Y_STEPPOPUP  = 20
SIZE_X_DUMPBUTTON = 140
SIZE_Y_DUMPBUTTON = 25

STYLE_FRAME   = wxCAPTION|wxICONIZE

CAPT_APP          = "RoboSep Tracer"
CAPT_BUTTON       = "Enter"
CAPT_AXIS         = "Monitor Axis:"
CAPT_AXIS_X0      = "X0"
CAPT_AXIS_X1      = "X1"
CAPT_AXIS_Y0      = "Y0"
CAPT_AXIS_Y1      = "Y1"
CAPT_CMDTYPE_ALL  = "All Commands"
CAPT_CMDTYPE_LIST = "Command List"
CAPT_CMDTYPE      = "Monitor Type"
CAPT_DUMP         = "Dump at Termination"
CAPT_DUMPNOTIME   = "Dump without Time Info"
CAPT_DUMPNODISP   = "Dump without Displaying"
CAPT_STEP         = "Enable Step Mode"
CAPT_STEPBEEP     = "Step with Beep"
CAPT_STEPPOPUP    = "Step with Popup"
CAPT_DUMPBUTTON   = "Dump Now"
 
FONT_SIZE_TITLE = 24

PASSWORD      = "robosepdebug"

Tracer    = None
CamMgr    = None
SplashMgr = None


BIN_DIR64        = 'C:\\program files (x86)\\sti\\robosep\\bin'   #CWJ ADD
BIN_DIR32        = 'C:\\program files\\sti\\robosep\\bin'         #CWJ ADD
BIN_DIR          = BIN_DIR64

CAM_NAME         = 'RoboCam.exe';
SPLASH_NAME      = 'Form_SplashScreen.exe';


def opj(path):
    """Convert paths to the platform-specific separator"""
    return apply(os.path.join, tuple(path.split('/')))

class RoboTracerFrame(wxFrame):    
    def __init__(self, parent, ID, title):

        self.Owner   = None
        self.Mode    = 0
         
        if os.environ.has_key('ProgramFiles(x86)'):    
            BIN_DIR = BIN_DIR64
        else:
            BIN_DIR = BIN_DIR32
        print ('\n** RoboTracer BIN_DIR: %s\n'%BIN_DIR)


        wxFrame.__init__(self, parent, ID, title,
                         wxPoint(ORIG_X_FRAME,ORIG_Y_FRAME), wxSize(SIZE_X_FRAME, SIZE_Y_FRAME), style=STYLE_FRAME)
        self.panel   = wxPanel(self, ID_PANEL, size = wxSize(SIZE_X_PANEL, SIZE_Y_PANEL))        
        self.button  = wxButton(self.panel, ID_BUTTON, CAPT_BUTTON, wxPoint(ORIG_X_BUTTON, ORIG_Y_BUTTON), 
                                  size =(SIZE_X_BUTTON, SIZE_Y_BUTTON))
        self.txtpwd  = wxTextCtrl(self.panel, ID_PWD, "", wxPoint(ORIG_X_PWD,ORIG_Y_PWD),
                                  size=(SIZE_X_PWD, SIZE_Y_PWD), style=wxTE_PASSWORD)

        bmpname = '%s\\wp.bmp'%BIN_DIR
        self.bmp = wxImage(bmpname, wxBITMAP_TYPE_BMP).ConvertToBitmap()
        self.wallpaper = wxStaticBitmap(self.panel, ID_BMP, self.bmp, 
                                        wxPoint(ORIG_X_WALLPAPER, ORIG_Y_WALLPAPER),
                                        wxSize(self.bmp.GetWidth(), self.bmp.GetHeight()))

        self.capt_axis = wxStaticText(self.panel,ID_CAPT_AXIS, CAPT_AXIS,
                                      wxPoint(ORIG_X_CAPTAXIS, ORIG_Y_CAPTAXIS))
        self.capt_axis.Show(False)

        self.axis_x0 = wxCheckBox(self.panel, ID_AXIS_X0, CAPT_AXIS_X0, 
                                  wxPoint(ORIG_X_CHKX0, ORIG_Y_CHKX0), wxSize(SIZE_X_AXIS, SIZE_Y_AXIS), wxNO_BORDER)
        self.axis_x1 = wxCheckBox(self.panel, ID_AXIS_X0, CAPT_AXIS_X1, 
                                  wxPoint(ORIG_X_CHKX1, ORIG_Y_CHKX1), wxSize(SIZE_X_AXIS, SIZE_Y_AXIS), wxNO_BORDER)
        self.axis_y0 = wxCheckBox(self.panel, ID_AXIS_X0, CAPT_AXIS_Y0, 
                                  wxPoint(ORIG_X_CHKY0, ORIG_Y_CHKY0), wxSize(SIZE_X_AXIS, SIZE_Y_AXIS), wxNO_BORDER)
        self.axis_y1 = wxCheckBox(self.panel, ID_DUMP, CAPT_AXIS_Y1, 
                                  wxPoint(ORIG_X_CHKY1, ORIG_Y_CHKY1), wxSize(SIZE_X_AXIS, SIZE_Y_AXIS), wxNO_BORDER)
        self.axis_x0.Show(false)
        self.axis_x1.Show(false)
        self.axis_y0.Show(false)
        self.axis_y1.Show(false)

        commandtype = [CAPT_CMDTYPE_ALL,CAPT_CMDTYPE_LIST]
        self.commandtype = wxRadioBox(self.panel, ID_CMDTYPE, CAPT_CMDTYPE,
                                      wxPoint(ORIG_X_CMBCMD,ORIG_Y_CMBCMD), wxSize(SIZE_X_CMD, SIZE_Y_CMD),
                                      commandtype, 2, wxRA_SPECIFY_COLS|wxNO_BORDER)
        self.commandtype.Show(false)
        

        self.dump = wxCheckBox(self.panel, ID_DUMP, CAPT_DUMP, 
                               wxPoint(ORIG_X_DUMP, ORIG_Y_DUMP), wxSize(SIZE_X_DUMP, SIZE_Y_DUMP), wxNO_BORDER)
        self.dump.Show(false)
        self.dumpnotime = wxCheckBox(self.panel, ID_DUMPNOTIME, CAPT_DUMPNOTIME, 
                               wxPoint(ORIG_X_DUMPNOTIME, ORIG_Y_DUMPNOTIME), wxSize(SIZE_X_DUMPNOTIME, SIZE_Y_DUMPNOTIME), wxNO_BORDER)
        self.dumpnotime.Show(false)
        self.dumpnodisp = wxCheckBox(self.panel, ID_DUMPNODISP, CAPT_DUMPNODISP, 
                               wxPoint(ORIG_X_DUMPNODISP, ORIG_Y_DUMPNODISP), wxSize(SIZE_X_DUMPNODISP, SIZE_Y_DUMPNODISP), wxNO_BORDER)
        self.dumpnodisp.Show(false)

        self.step = wxCheckBox(self.panel, ID_STEP, CAPT_STEP, 
                               wxPoint(ORIG_X_STEP, ORIG_Y_STEP), wxSize(SIZE_X_STEP, SIZE_Y_STEP), wxNO_BORDER)
        self.step.Show(false)

        self.stepbeep = wxCheckBox(self.panel, ID_STEPBEEP, CAPT_STEPBEEP, 
                               wxPoint(ORIG_X_STEPBEEP, ORIG_Y_STEPBEEP), wxSize(SIZE_X_STEPBEEP, SIZE_Y_STEPBEEP), wxNO_BORDER)
        self.stepbeep.Show(false)

        self.steppopup = wxCheckBox(self.panel, ID_STEPPOPUP, CAPT_STEPPOPUP, 
                               wxPoint(ORIG_X_STEPPOPUP, ORIG_Y_STEPPOPUP), wxSize(SIZE_X_STEPPOPUP, SIZE_Y_STEPPOPUP), wxNO_BORDER)
        self.steppopup.Show(false)

        self.dumpbutton  = wxButton(self.panel, ID_DUMPBUTTON, CAPT_DUMPBUTTON, wxPoint(ORIG_X_DUMPBUTTON, ORIG_Y_DUMPBUTTON), 
                                  size =(SIZE_X_DUMPBUTTON, SIZE_Y_DUMPBUTTON))
        self.dumpbutton.Show(false)
        
        EVT_BUTTON(self, ID_BUTTON, self.OnClick)
        EVT_BUTTON(self, ID_DUMPBUTTON, self.OnDumpClick)
        
    def SetOwner( self, Owner ):
        self.Owner = Owner
        
    def OnClick(self, event):
        if (self.Mode) : 
           self.Iconize( True )
        else :
           if (self.txtpwd.GetLineText(0) == PASSWORD):
              self.Mode = 1
              self.wallpaper.Show(False)
              self.txtpwd.Show(False)
              self.capt_axis.Show(True)
              self.axis_x0.Show(True)
              self.axis_x1.Show(True)
              self.axis_y0.Show(True)
              self.axis_y1.Show(True)
              self.commandtype.Show(True)
              self.dump.Show(True)
              self.dumpnotime.Show(True)
              self.dumpnodisp.Show(True)
              self.step.Show(True)
              self.stepbeep.Show(True)
              self.steppopup.Show(True)
              self.dumpbutton.Show(True)              
           else:
              self.Iconize( True )

    def OnDumpClick(self, event):
        if( tesla.config.SS_EXT_LOGGER == 1 ):  # 2013-01-14 -- sp, added ini file flag
            Dmp = SimpleStep.SimpleStep.GetSSExtLoggerInstance()

            x0 = self.axis_x0.GetValue();
            x1 = self.axis_x1.GetValue();
            y0 = self.axis_y0.GetValue();
            y1 = self.axis_y1.GetValue();

            Dmp.SetTestDebugMode(x0, x1, y0, y1);
        
        pass
        
class RoboTracerApp(wxApp):
    keyword = None
    AspiHeight = 0;    
    def OnInit(self):
        self.frame = RoboTracerFrame(NULL, ID_FRAME, "Tracer")
        self.frame.SetOwner(self)
        self.frame.Show(true)
        self.ObjCheckParam  = None
        self.timer = wxTimer(self, ID_TIMER) 
        EVT_TIMER(self,  ID_TIMER, self.OnTimer)
        return true

    def TerminateApp(self):
        self.ExitMainLoop()

    def OnTimer(self, event):
        if self.ObjCheckParam == None:
           pass
        else:
           if self.ObjCheckParam.isAlive(): 
              pass
           else:   
              self.timer.Stop()
              self.TerminateApp()
              
    def SetObjfunc(self, param):
        self.ObjCheckParam  = param
        self.timer.Start(1000)
        
def GetRoboTracerInstance():
    global Tracer

    if Tracer == None:
       Tracer = RoboTracerApp(redirect = False)

    return Tracer
          
class RoboCamMgr :
    FlagError               = False;
    FileName                = None;
    ErrorFileName           = None; 
    FlagBlock               = False;
    Vid_ID                  = 0;
    Aud_ID                  = 0;
    Prof_ID                 = 29;
    MAX_FILE_LIST           = 5;                    # 2012-01-19 sp add to ini file
    CAM_ORIENTATION         = 0;                    # 2012-01-19 sp add to ini file
    CAM_FORMAT              = -1;                   # 2012-01-19 sp add to ini file
    ROBO_REMOTE_START       = win32con.WM_USER + 1;
    ROBO_REMOTE_STOP        = win32con.WM_USER + 2;
    ROBO_REMOTE_SNAPSHOT    = win32con.WM_USER + 3;
    ROBO_REMOTE_SET_PROFILE = win32con.WM_USER + 4;
    ROBO_REMOTE_SET_VIDEO   = win32con.WM_USER + 5;
    ROBO_REMOTE_SET_AUDIO   = win32con.WM_USER + 6;
    ROBO_REMOTE_SET_EVENT   = win32con.WM_USER + 7;
    ROBO_REMOTE_SET_STEP    = win32con.WM_USER + 8;
    ROBO_REMOTE_SET_ERROR   = win32con.WM_USER + 9;
    ROBO_REMOTE_SET_CLOSE   = win32con.WM_USER + 10;    
    ROBO_REMOTE_SET_FOCUS   = win32con.WM_USER + 11;
    ROBO_REMOTE_SET_MANFOCUS= win32con.WM_USER + 12;
    ROBO_REMOTE_SET_QUADRANT= win32con.WM_USER + 13;
    
    RoboStepString  = ['None', 'HomeAll', 'Transport', 'Mix', 'Incubate', 'TopUp',
                       'ReSuspend', 'Flush', 'Prime', 'Pause','Separate',
                       'MixTrans', 'TopUpMixTrans', 'ResusMixSepTrans', 'ResusMix',
                       'TopUpTrans', 'TopUpTransSepTrans', 'TopUpMixTransSepTrans'];
    RoboEventString = ['None', 'Pause', 'RESUME', 'Halt', 'E-Stop'];
    DemoState       = 'None';
    def __init__(self):
        self.FlagError = False;
        self.wh        = None;
        self.Sec       = -1
        self.ProtID    = -1
        self.ProtSeq   = -1
        
        if os.environ.has_key('ProgramFiles(x86)'):    
            self.BIN_DIR = BIN_DIR64
        else:
            self.BIN_DIR = BIN_DIR32

        # 2012-01-03 sp -- added logging
        self.svrLog = PgmLog( 'svrLog' )
        self.logPrefix = 'RT'
        # 2012-01-03 sp -- added initialization values from configuration file
        self.getIniConfigurations( tesla.config.SERVER_CONFIG_PATH )


    # 2012-01-03 sp -- added initialization from configuration file
    def getIniConfigurations(self, configFile):
        ''' use defaults unless it can retrieve settings from configuration file '''
        funcReference = __name__ + '.getLogConfigurations'

        cfg = ConfigParser()
        # if configuration file exists, extract the settings
        if( os.path.exists( configFile)):
            cfg.read( configFile )
            try:
                # get settings from configuration file
                self.Vid_ID             = int( cfg.get( 'camera', 'videoID' ) )
                self.Aud_ID             = int( cfg.get( 'camera', 'audioID' ) )
                self.Prof_ID            = int( cfg.get( 'camera', 'profID' ) )
                self.MAX_FILE_LIST      = int( cfg.get( 'camera', 'maxFileList' ) )
                self.CAM_ORIENTATION    = int( cfg.get( 'camera', 'orientation' ) )
                self.CAM_FORMAT         = int( cfg.get( 'camera', 'format' ) )
                # generate log message of success, report file and version number
                self.svrLog.logVerbose('', self.logPrefix, funcReference, \
                    'Using log configuration: videoID=%d | audioID=%d | profID=%d' % (self.Vid_ID, self.Aud_ID, self.Prof_ID) ) 
            except Exception, msg:
                self.svrLog.logWarning('', self.logPrefix, funcReference, \
                    'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg) )
        else:
            self.svrLog.logWarning('', self.logPrefix, funcReference, \
                    'Error reading from configuration file [%s]...Default settings used: %s' % (configFile, msg) )

                
    def SetTxData(self, msg):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        win32clipboard.OpenClipboard();
        win32clipboard.EmptyClipboard();
        win32clipboard.SetClipboardData(win32con.CF_TEXT, msg);
        win32clipboard.CloseClipboard();
        
    def MakeFileNames(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        now = time.localtime();
        CaptureDir            = tesla.config.LOG_DIR+'\\videolog\\';        
        CaptureErrorDir       = tesla.config.LOG_DIR+'\\videoErrorlog\\';
        self.FileName         = CaptureDir + ('%04d%02d%02d-%02dh%02dm%02ds.asf' % (now.tm_year, now.tm_mon, now.tm_mday, now.tm_hour, now.tm_min, now.tm_sec))
        self.ErrorFileName    = CaptureErrorDir + ('Error%04d%02d%02d-%02dh%02dm%02ds.asf' % (now.tm_year, now.tm_mon, now.tm_mday, now.tm_hour, now.tm_min, now.tm_sec))
        self.FileSmiName      = string.replace(self.FileName, 'asf', 'smi');
        self.ErrorFileSmiName = string.replace(self.ErrorFileName, 'asf', 'smi');
        FileList = glob.glob( os.path.join(CaptureDir, '*.asf') )
        FileList.sort();
        ListLen = len(FileList);
        if ListLen >= self.MAX_FILE_LIST :
           DeleteMovieName = FileList[0];
           try:
                os.remove( DeleteMovieName );
                print ('\n\n### Delete the file %s!\n'%DeleteMovieName);
           except:
                print ('\n\n### Cannot Delete the file %s!\n'%DeleteMovieName);
           DeleteSMIName = string.replace(DeleteMovieName, 'asf', 'smi');
           try:
                os.remove( DeleteSMIName );
                print ('\n\n### Delete the file %s!\n'%DeleteSMIName);
           except:
                print ('\n\n### Cannot Delete the file %s!\n'%DeleteSMIName);
        
    def ExcuteRoboCam(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
           
        AppName = self.BIN_DIR + '\\' + CAM_NAME + (" %d") % self.CAM_ORIENTATION;
        try:
            oldwh = win32gui.FindWindow('TApplication','RoboCam');
            if (oldwh != 0):
                win32gui.SendMessage(oldwh, self.ROBO_REMOTE_SET_CLOSE, 0, 0);
        except :
            None;
        
        try:                
            print '\n >>> RoboCam Start : %s\n'%AppName
            execCode = win32api.WinExec(AppName);
            self.wh  = win32gui.FindWindow('TApplication','RoboCam');
            if (self.wh != 0):
                print ('\n\n #### RoboCam Running... \n');
                win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_VIDEO,   self.Vid_ID, self.CAM_FORMAT);
                win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_AUDIO,   self.Aud_ID, 0);
                win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_PROFILE, self.Prof_ID, 0);
                self.FlagError = False;
                                          
        except:
            print (">> Error launching app: RoboCam");
        
    def TerminateRoboCam(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
           
        if (self.wh != 0):
            win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_CLOSE, 0, 0);
            print ('\n\n #### RoboCam Terminated... \n');
            if (self.FlagError == True):
                self.FlagError = False;
                try:
                    shutil.copy( self.FileName, self.ErrorFileName ); 
                except: 
                    print ('\n\n #### RoboCam cannot find %s !\n'%self.FileName);
                try:
                    shutil.copy( self.FileSmiName, self.ErrorFileSmiName );                 
                except:
                    print ('\n\n #### RoboCam cannot find %s !\n'%self.FileSmiName);                    

        self.wh = 0;
        self.FlagError = False;
        
    
    def StartCapture(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        self.FlagError = False;
        if (self.wh != 0):
            self.MakeFileNames();
            time.sleep(1);
            self.SetTxData(self.FileName);

            errcode = win32gui.SendMessage(self.wh, self.ROBO_REMOTE_START,0, 0);
            if (errcode == 1):
               print ('\n\n #### RoboCam starts capturing at %s\n\n'%self.FileName);
               return True;
            else:
               print ('\n\n #### RoboCam cannot capture! Error Code: %d\n'%errcode );
               ans = False;
            while (errcode == 3):
               print ('\n\n #### RoboCam fails to receive the file name! Retry...\n');
               self.SetTxData(self.FileName);
               errcode = win32gui.SendMessage(self.wh, self.ROBO_REMOTE_START,0, 0);
            if (errcode == 1):
               ans = True
            
            return ans;   
               
    def StopCapture(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        if (self.wh != 0):
            rtn = win32gui.SendMessage(self.wh, self.ROBO_REMOTE_STOP,0, 0);
            if rtn == 1 :
               print ('\n\n #### RoboCam Stop capturing... \n');
            else:   
               print ('\n\n #### RoboCam Cannot Stop capturing... \n');
                           
            if (self.FlagError == True):
                self.FlagError = False;
                try:
                    shutil.copy( self.FileName, self.ErrorFileName ); 
                except : 
                    print ('\n\n #### RoboCam cannot find %s !\n'%self.FileName);
                try:
                    shutil.copy( self.FileSmiName, self.ErrorFileSmiName );                 
                except:
                    print ('\n\n #### RoboCam cannot find %s !\n'%self.FileSmiName);                    
               
    def SetProcEvent(self, proc_event):
        self.DemoState = self.RoboEventString[proc_event];
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        if (self.wh != 0):
           print ('\n\n #### RoboCam Event %s \n'%self.RoboEventString[proc_event]);
           try:
              win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_EVENT, proc_event, 0);
           except:   
               pass;
    def GetDemoProcEvent(self):
        return self.DemoState;
        
    def ResetDemoProcEvent(self):
        self.DemoState = 'None';
        
    def SetProcStep(self, proc_step):
        if (self.FlagBlock == True):
            return;
            
        self.DemoState = self.RoboStepString[proc_step];
        
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        if (self.wh != 0):
           print ('\n\n #### RoboCam Step %s \n'%self.RoboStepString[proc_step]);
           try: 
              win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_STEP, proc_step, 0);
           except:   
               pass;
               
    def SetProcError(self, Critical = False):
        if os.environ.has_key('ROBO_NOCAM'):
           return;
    
        self.FlagError = True;
        if (self.wh != 0):
           print ('\n\n #### RoboCam Error! \n');
           win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_ERROR, 0, 0);
        
        if (Critical == True):
           print ('\n\n #### RoboCam Error! Critical Error Captured!\n');


            
    def GetVideoCaptureName(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return 'ROBO_NOCAM';    
        return self.FileName;
        
    def GetErrorVideoCaptureName(self):
        if os.environ.has_key('ROBO_NOCAM'):
           return 'ROBO_NOCAM';    
        return self.ErrorFileName;

    def SetCurrentProtocolID(self, id):
        self.ProtID = id;
        print ('\n\n #### RoboCam id %d \n'%self.ProtID);                

    def GetCurrentProtocolID(self):
        return self.ProtID;
        
    def SetCurrentProtocolSeq(self, seq):
        self.ProtSeq = seq;
        print ('\n\n #### RoboCam Seq %d \n'%self.ProtSeq);        
        
    def GetCurrentProtocolSeq(self):
        return self.ProtSeq;
        
    def SetCurrentSec(self, sec):
        self.Sec = sec;
        if os.environ.has_key('ROBO_NOCAM'):
           return;    
        if (self.wh != 0):
           print ('\n\n #### RoboCam Quadrant %d \n'%self.Sec);
           win32gui.SendMessage(self.wh, self.ROBO_REMOTE_SET_QUADRANT, self.Sec, 0);
        
    def GetCurrentSec(self): 
        return self.Sec;
        
    def SetBlockUpdate(self, sw):
        self.FlagBlock = sw;
    def SetupForCommand(self, id, cmd_str, **kw):
        funcReference = __name__ + '.SetupForCommand'
        try:
            idx = self.RoboStepString.index(cmd_str)
            self.SetProcStep(idx);
        except:
            print '\n\n #### RoboCam UNKNOWN COMMAND: %s \n' % cmd_str
            self.svrLog.logWarning('', self.logPrefix, funcReference, \
                                   '\n\n #### RoboCam UNKNOWN COMMAND: %s \n' % (cmd_str))
            
	self.SetCurrentProtocolID(id)
        if kw.has_key('seq'):   
           seq = kw['seq']      
 	   self.SetCurrentProtocolSeq(seq)
        if kw.has_key('initialQuadrant'): 
           qdr = kw['initialQuadrant']
           self.SetCurrentSec(qdr)
        
        
def GetRoboCamMgrInstance():
    global CamMgr;
    
    if CamMgr == None:
       CamMgr = RoboCamMgr();
       
    return CamMgr;

class RoboSplashMgr:
    def __init__(self):
        if os.environ.has_key('ProgramFiles(x86)'):    
            self.BIN_DIR = BIN_DIR64
        else:
            self.BIN_DIR = BIN_DIR32
        self.AppName = self.BIN_DIR + '\\' + SPLASH_NAME; 
        
    def StartSplash(self):
        self.TerminateSplash();
        try:
          execCode = win32api.WinExec(self.AppName);
        except:
          None;
        
    def TerminateSplash(self):        
        try:
            oldwh = win32gui.FindWindow('WindowsForms10.Window.8.app.0.378734a','RoboSep_Splash');
            if (oldwh != 0):
                win32gui.SendMessage(oldwh, win32con.WM_CLOSE, 0, 0);
        except :
            None;
        
def GetRoboSplashMgrInstance():
    global SplashMgr;
    
    if SplashMgr == None:
       SplashMgr = RoboSplashMgr();
       
    return SplashMgr;
        
def RoboSepMessageBox(message):
    SplMgr = GetRoboSplashMgrInstance();
    SplMgr.TerminateSplash();
    MessageBox = ctypes.windll.user32.MessageBoxA;
    MessageBox(None, message, 'RoboSep-S Error', 0); 

