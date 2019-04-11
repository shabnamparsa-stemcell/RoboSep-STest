unit main;

interface

//{$DEFINE DEF_DELPHI7}

//{$DEFINE DEF_VID_LOG}
//{$DEFINE DEF_VID_PRIORITY}

{$DEFINE DEF_CAPTURE_BEEP}

{$DEFINE DEF_POPUP_PREVIEW}

{$IFDEF DEF_DELPHI7}
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, DSUtil, StdCtrls, DSPack, DirectShow9, Menus, ExtCtrls, Subtitle,
  ShellApi, IniFiles;
{$ELSE}
uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, DSUtils, StdCtrls, DSPack, DirectShow9, Menus, ExtCtrls, Subtitle,
  ShellApi, IniFiles;
{$ENDIF}

const WM_USER_NOTIFYICON = WM_USER + $80;
      WM_USER_MINIMIZE   = WM_USER + $81;

type TPixcel = record
  Blue  : BYTE;
  Green : BYTE;
  Red   : BYTE;
end;
type PPixcel = ^TPixcel;

type TWindowHook = function(var Message : TMessage) : Boolean of object;

const
  CLSID_TRotateTrans: TGUID = '{EEA1BC2F-B02C-4A32-AF95-B1D53E5F45BB}';

type
 IRotateInf = interface(IUnknown)
    ['{D7C086E6-F0D1-42F3-8072-B0965596951D}']
 function SetRotate(sw : Integer):HResult;stdcall;
 function GetRGB24( pBuffer : PByte ; pSize   : PInteger;
                                      pWidth  : PInteger ;
                                      pHeight : PInteger ):HResult;stdcall;
end;

type TSETAUTOFOCUS = procedure(sw : Integer) of object;

type
  TVideoForm = class(TForm)
    MainMenu1     : TMainMenu;
    vDevices      : TMenuItem;
    aDevices      : TMenuItem;
    Profiles      : TMenuItem;
    Capture       : TMenuItem;
    PopupMenu     : TPopupMenu;
    PopShow       : TMenuItem;
    SaveDialog    : TSaveDialog;
    SaveSnapDialog: TSaveDialog;

    FilterGraph   : TFilterGraph;
    vFilter       : TFilter;           //Video Capture Device - Web Cam
    aFilter       : TFilter;           //Audio Capture Device - Mic
    rFilter       : TFilter;           //Rotator for Web Cam
    rFilter2      : TFilter;           //Rotator for ASFWriter
    ASFWriter     : TASFWriter;
    SampleGrabber : TSampleGrabber;
    VideoWindow   : TVideoWindow;

    TimerSubTitle : TTimer;
    TimerMinimize : TTimer;
    Image         : TImage;
    TimerAWB: TTimer;
    TimerStopRecording: TTimer;

    procedure FormCreate(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure TimerSubTitleTimer(Sender: TObject);
    procedure PopShowClick(Sender: TObject);
    procedure TimerMinimizeTimer(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure TimerAWBTimer(Sender: TObject);
    procedure TimerStopRecordingTimer(Sender: TObject);

  private
    { D?larations priv?s }
    FaDevNum    : Integer;
    FvDevNum    : Integer;
    FCamFormat  : Integer;
    FcProfile   : Integer;
    FmState     : Integer;
    FvFocus     : Integer;
    FvFocusIdx  : Integer;
    FipPort     : Cardinal;
    FDelayTime  : Integer;
    FFileName   : String;
    FOriginCap  : String;
    FAppDir     : String;
    FStartTime  : String;
    FEvent      : String;
    FStep       : String;
    FError      : String;
    FQaudrant   : Integer;
    FStartTick  : Cardinal;
    FHookProc   : TWindowHook;
    FPlaying    : Boolean;
    FRotate     : Integer;
    FPreview    : Boolean;
    FRotateInf  : IRotateInf;
    FRotateInf2 : IRotateInf;
    FAFFilter   : IAMCameraControl;
    FAWBFilter  : IAMVideoProcAmp;
    FSubtitle   : TSubtitle;
    FDefFocus   : Integer;
    FDefExp     : Integer;
    FWrkFocus   : Integer;
    FSetFocus   : TSETAUTOFOCUS;
    FProfile    : Boolean;
    FCamMute    : Boolean;
    FAWBalance  : Boolean;
    FOnMinimize : TNotifyEvent;
    function  StartCapture(FileName : String)      : Boolean;
    function  StopCapture()                        : Boolean;
    function  TakeSnapshot(FileName : AnsiString)  : Boolean;
    function  AppHookProc(var Message : TMessage)  : Boolean;
    procedure SetEventCaption(EventNum : Integer);
    procedure SetStepCaption(StepNum : Integer);
    procedure SetErrorCaption();
    procedure SetRotateFilter();
    procedure SetAWBalance(sw:Boolean);
    procedure SetPreview( sw : Boolean );
    procedure RefreshScreen();
    function  UpdateVideoDevice(): Boolean;
    function  UpdateAudioDevice(): Boolean;
    function  UpdateCaptureMenu(): Boolean;
    function  UpdateProfileMenu(): Boolean;
  protected
    FNotifyIconData  : TNotifyIconData;
    FisTrayIcon      : Boolean;
    procedure ProcTrayIcon( var msg : TMessage ); message WM_USER_NOTIFYICON;
    procedure OnUserMinimize( var msg : TMessage ); message WM_USER_MINIMIZE;
    procedure OnCamMinimize(sender : TObject);
  public
    { D?larations publiques }
    procedure OnSelectCPUProfile(sender: TObject);
    procedure OnSelectVideoDevice(sender: TObject);
    procedure OnSelectAudioDevice(sender: TObject);
    procedure OnSelectProfiles(sender: TObject);
    procedure OnSelectCapture(sender: TObject);
    procedure OnSelectSnapshot(sender: TObject );
    procedure OnSelectCamFormat(sender: TObject);
    procedure OnSelectPreview(sender: TObject);
    procedure OnAppMessage(var Msg: TMsg; var Handled: Boolean);
    procedure OnSelectVideoFocus(sender: TObject);
    procedure OnSelectWhiteBalance(sender: TObject);
    procedure OnSelectFocusParam(sender: TObject);
    procedure SetAutoFocus(sw : Integer);
    procedure SetAutoFocus2(sw : Integer);
    procedure SetStopRecordingTimer(sw : Boolean);
  end;

var
  VideoForm : TVideoForm;

implementation

{$R *.dfm}
uses ActiveX, clipbrd, JPEG, focus, camformat;

type TSaveJpeg = function(pBuffer : PByte; Width, Height, Ratio : Integer; pFileName : PAnsiChar) : Integer ; cdecl;

var
  ImgSpace : array [0..3145728] of BYTE; //1024x1024xRGB Work Space

var
  vSysDev         : TSysDevEnum;
  aSysDev         : TSysDevEnum;
  VideoMediaTypes : TEnumMediaType;

const TIMERINTV     : Integer   = 1000;
const JPEGENGINENAME: String    = '.\JpegDLL.dll';
const JPEGFUNCNAME  : String    = 'SaveColorJpg';
const JPEGCOMPRATIO             = 80;
const INIFILENAME   : String    = '.\RoboCam';
const INIEXTNAME    : String    = '.ini';
const INISECTION    : String    = 'CameraFormat';
const INIENTRY      : String    = 'FormatIndex';
const INIENTRYDELAY : String    = 'StopDelayTimeInMilliSecond';
const IP_PORT                   = 32768;
const MAXPROFILES               = 33;
const ROTATENAME    : String = '_RotateTrans';
const ASFFmtStrings : array[0..32] of string = (
                      'wmp_V80_255VideoPDA',              // 0
                      'wmp_V80_150VideoPDA',              // 1
                      'wmp_V80_28856VideoMBR',            // 2
                      'wmp_V80_100768VideoMBR',           // 3
                      'wmp_V80_288100VideoMBR',           // 4
                      'wmp_V80_288Video',                 // 5
                      'wmp_V80_56Video',                  // 6
                      'wmp_V80_100Video',                 // 7
                      'wmp_V80_256Video',                 // 8
                      'wmp_V80_384Video',                 // 9
                      'wmp_V80_768Video',                 //10
                      'wmp_V80_700NTSCVideo',             //11
                      'wmp_V80_1400NTSCVideo',            //12
                      'wmp_V80_384PALVideo',              //13
                      'wmp_V80_700PALVideo',              //14
                      'wmp_V80_288MonoAudio',             //15
                      'wmp_V80_288StereoAudio',           //16
                      'wmp_V80_32StereoAudio',            //17
                      'wmp_V80_48StereoAudio',            //18
                      'wmp_V80_64StereoAudio',            //19
                      'wmp_V80_96StereoAudio',            //20
                      'wmp_V80_128StereoAudio',           //21
                      'wmp_V80_288VideoOnly',             //22
                      'wmp_V80_56VideoOnly',              //23
                      'wmp_V80_FAIRVBRVideo',             //24
                      'wmp_V80_HIGHVBRVideo',             //25
                      'wmp_V80_BESTVBRVideo',             //26
                      'Custom_Robo_Hi',                   //27
                      'Custom_Robo_Mid',                  //28
                      'Custom_Robo_Lo',                   //29
                      'Custom_Robo_Hi_VideoOnly',         //30
                      'Custom_Robo_Mi_VideoOnly',         //31
                      'Custom_Robo_Lo_VideoOnly'          //32
                      );


const PROFILE_ROBO_HI     : String = 'RoboHi.prx';
      PROFILE_ROBO_Mi     : String = 'RoboMi.prx';
      PROFILE_ROBO_LO     : String = 'RoboLo.prx';
const PROFILE_ROBO_HI_VO  : String = 'RoboHiVideoOnly.prx';
      PROFILE_ROBO_Mi_VO  : String = 'RoboMiVideoOnly.prx';
      PROFILE_ROBO_LO_VO  : String = 'RoboLoVideoOnly.prx';

const ROBO_REMOTE_START        = WM_USER + 1;
      ROBO_REMOTE_STOP         = WM_USER + 2;
      ROBO_REMOTE_SNAPSHOT     = WM_USER + 3;
      ROBO_REMOTE_SET_PROFILE  = WM_USER + 4;
      ROBO_REMOTE_SET_VIDEO    = WM_USER + 5;
      ROBO_REMOTE_SET_AUDIO    = WM_USER + 6;
      ROBO_REMOTE_SET_EVENT    = WM_USER + 7;
      ROBO_REMOTE_SET_STEP     = WM_USER + 8;
      ROBO_REMOTE_SET_ERROR    = WM_USER + 9;
      ROBO_REMOTE_SET_CLOSE    = WM_USER + 10;
      ROBO_REMOTE_SET_FOCUS    = WM_USER + 11;
      ROBO_REMOTE_SET_MANFOCUS = WM_USER + 12;
      ROBO_REMOTE_SET_QUADRANT = WM_USER + 13;
      ROBO_REMOTE_ENABLE_CAM   = WM_USER + 14;
      ROBO_REMOTE_DISABLE_CAM  = WM_USER + 15;

const APPNAME                  = 'RoboCam';

const ROBO_EVT_NONE            = 0;
      ROBO_EVT_PAUSE           = 1;
      ROBO_EVT_RESUME          = 2;
      ROBO_EVT_HALT            = 3;
      ROBO_EVT_ESTOP           = 4;
const RoboEventString : array[0..4] of String = ('None',
                                                 'Pause',
                                                 'RESUME',
                                                 'Halt',
                                                 'E-Stop');
const ROBO_STEP_NONE           = 0;
      ROBO_STEP_HOMEALL        = 1;
      ROBO_STEP_TRANSPORT      = 2;
      ROBO_STEP_MIX            = 3;
      ROBO_STEP_INCUBATE       = 4;
      ROBO_STEP_TOPUP          = 5;
      ROBO_STEP_RESUSPEND      = 6;
      ROBO_STEP_FLUSH          = 7;
      ROBO_STEP_PRIME          = 8;
      ROBO_STEP_PAUSE          = 9;
      ROBO_STEP_SEPARATE       = 10;
      ROBO_STEP_MIXTRANS       = 11;
      ROBO_STEP_TOPUPMIXTRANS  = 12;
      ROBO_STEP_RESMIXSEPTRANS = 13;
      ROBO_STEP_RESMIX         = 14;
      ROBO_STEP_TOPUPTRANS     = 15;
      ROBO_STEP_TOPUPTRANSSEPTRANS    = 16;
      ROBO_STEP_TOPUPMIXTRANSSEPTRANS = 17;
      ROBO_STEP_ENDOFPROTOCOL         = 18;

const RoboStepString : array[0..18] of String = ('None',
                                                'HomeAll',
                                                'Transport',
                                                'Mix',
                                                'Incubate',
                                                'TopUp',
                                                'ReSuspend',
                                                'Flush',
                                                'Prime',
                                                'Pause',
                                                'Separate',
                                                'MixTrans',
                                                'TopUpMixTrans',
                                                'ResMixSepTrans',
                                                'ResMix',
                                                'TopUpTrans',
                                                'TopUpTransSepTrans',
                                                'TopUpMixTransSepTrans',
                                                'EndOfProtocol');
const PRIORITY_CAPTURE = 2;
      PRIORITY_NORMAL  = 1;
procedure SetPriorityLevel(P: byte);
begin
{$IFDEF DEF_VID_PRIORITY}
  case p of
  0: Setpriorityclass(GetCurrentProcess(), IDLE_PRIORITY_CLASS);
  1: Setpriorityclass(GetCurrentProcess(), NORMAL_PRIORITY_CLASS);
  2: Setpriorityclass(GetCurrentProcess(), $00008000);//ABOVE_NORMAL_PRIORITY_CLASS
  3: Setpriorityclass(GetCurrentProcess(), HIGH_PRIORITY_CLASS);
  4: Setpriorityclass(GetCurrentProcess(), REALTIME_PRIORITY_CLASS);
  end;
{$ENDIF}
end;

function GetTimeStampLogName() : String;
  var stamp   : String;
      msg     : String;
      appPath : String;
begin
  appPath := ExtractFilePath(Application.ExeName);
  DateSeparator    := '-';
  TimeSeparator    := ':';
  ShortDateFormat	 := 'yyyymmdd-hhnnss';
  stamp            := DateToStr(Now);
  result           := appPath+'..\logs\robocam'+stamp+'.log';
  msg              := Format('[RoboCam Log File : %s ]',[result]);
  OutputDebugString(PChar(msg));
end;

function GetTimeStamp() : String;
  var Msg : String;
begin
  DateSeparator    := '-';
  TimeSeparator    := ':';
  ShortDateFormat	 := 'yyyymmdd-hh:nn:ss';
  Msg              := DateToStr(Now);
  result           := Msg;
end;

function CheckProfileFiles(Dir : String) : Boolean;
  var rtn      : Boolean;
      AppDir   : String;
      FullName : String;
begin
  rtn := True;

  AppDir := Dir;

  FullName := AppDir + PROFILE_ROBO_HI;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  FullName := AppDir + PROFILE_ROBO_MI;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  FullName := AppDir + PROFILE_ROBO_LO;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  FullName := AppDir + PROFILE_ROBO_HI_VO;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  FullName := AppDir + PROFILE_ROBO_MI_VO;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  FullName := AppDir + PROFILE_ROBO_LO_VO;
  rtn := FileExists(FullName);
  if rtn = False then
  begin
    Result := rtn;
    Exit;
  end;

  Result := rtn;
end;

function TVideoForm.UpdateVideoDevice():Boolean;
  var i : Integer;
      Device : TMenuItem;
      bVidInit : Boolean;
begin
  bVidInit := False;

  FRotateInf  := nil;
  FRotateInf2 := nil;
  vSysDev     := nil;

  vSysDev := TSysDevEnum.Create(CLSID_VideoInputDeviceCategory);
  if vSysDev.CountFilters > 0 then
  begin
    for i := 0 to vSysDev.CountFilters - 1 do
    begin
      Device          := TMenuItem.Create(vDevices);
      Device.Caption  := vSysDev.Filters[i].FriendlyName;
      Device.Tag      := i;
      Device.OnClick  := OnSelectVideoDevice;
      vDevices.Add(Device);
      bVidInit := True
    end;
  end else begin
     bVidInit := False;
  end;
  Result := bVidInit;
end;

function TVideoForm.UpdateAudioDevice : Boolean;
  var i : Integer;
      Device : TMenuItem;
      bAudInit : Boolean;
begin
  bAudInit := False;
  aSysDev := nil;
  aSysDev := TSysDevEnum.Create(CLSID_AudioInputDeviceCategory);
  if aSysDev.CountFilters > 0 then
    for i := 0 to aSysDev.CountFilters - 1 do
    begin
        Device          := TMenuItem.Create(vDevices);
        Device.Caption  := aSysDev.Filters[i].FriendlyName;
        Device.Tag      := i;
        Device.OnClick  := OnSelectAudioDevice;
        aDevices.Add(Device);
        bAudInit := True;
    end
  else begin
    bAudInit := False;
  end;
  Result :=  bAudInit;
end;

function TVideoForm.UpdateCaptureMenu() : Boolean;
  var Cap : TMenuItem;
      rtn : Boolean;
begin
  rtn := False;

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Start';
  Cap.Tag     := 0;
  Cap.OnClick := OnSelectCapture;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Stop';
  Cap.Tag     := 1;
  Cap.OnClick := OnSelectCapture;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Snapshot';
  Cap.Tag     := 2;
  Cap.OnClick := OnSelectSnapshot;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 3;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Auto Focus';
  Cap.Tag     := 4;
  Cap.OnClick := OnSelectVideoFocus;
  Capture.Add(Cap);
  FvFocusIdx  := 4;

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 5;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Manual Focus Parameter';
  Cap.Tag     := 6;
  Cap.OnClick := OnSelectFocusParam;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 7;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Profile CPU';
  Cap.Tag     := 8;
  Cap.OnClick := OnSelectCPUProfile;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 9;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Select Camera Format';
  Cap.Tag     := 10;
  Cap.OnClick := OnSelectCamFormat;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 11;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Auto White Balance';
  Cap.Tag     := 12;
  Cap.OnClick := OnSelectWhiteBalance;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := '-';
  Cap.Tag     := 13;
  Cap.OnClick := nil;
  Capture.Add(Cap);

  Cap         := TMenuItem.Create(Capture);
  Cap.Caption := 'Preview';
  Cap.Tag     := 14;
  Cap.OnClick := OnSelectPreview;
  Capture.Add(Cap);

  rtn := True;

  Result := rtn;
end;

function TVideoForm.UpdateProfileMenu() : Boolean;
  var Prof : TMenuItem;
      i    : Integer;
      rtn  : Boolean;
begin
  rtn := False;
  for i := 0 to MAXPROFILES - 1 do
  begin
    Prof              := TMenuItem.Create(Profiles);
    Prof.Caption      := ASFFmtStrings[i];
    Prof.Tag          := i;
    Prof.OnClick      := OnSelectProfiles;
    Profiles.Add(Prof);
  end;
  rtn := True;
  Result := rtn;
end;

procedure TVideoForm.FormCreate(Sender: TObject);
var
  i, Counts,num     : Integer;
  Prof, Cap         : TMenuItem;
  str,dir           : String;
  bAudInit,bVidInit : Boolean;
  bSubInit          : Boolean;
  MyIni             : TIniFile;
begin

  bAudInit    := False;
  bVidInit    := False;
  bSubInit    := False;
  FPlaying    := False;
  FStartTick  := 0;
  FStartTime  := '';
  FEvent      := '';      //GetTickCount();
  FStep       := '';
  FError      := '';
  FRotate     := 0;
  FPreview    := False;
  FvFocus     := 0;
  FDefFocus   := 0;
  FDefExp     := 0;
  FWrkFocus   := 0;
  FSetFocus   := SetAutoFocus;
  FProfile    := False;
  FQaudrant   := -1;
  FisTrayIcon := False;
  FFileName   := '';
  FipPort     := IP_PORT;
  FOriginCap  := APPNAME;
  FCamFormat  := -1;
  FCamMute    := False;
  FAWBalance  := True;
  FDelayTime  := 30000;
  SaveDialog.DefaultExt     := 'asf';
  SaveDialog.Filter         := 'ASF Format (*.asf)|*.asf';

  SaveSnapDialog.DefaultExt := 'jpg';
  SaveSnapDialog.Filter     := 'JPEG Format (*.jpg)|*.jpg';

  OutputDebugString('+ RoboCam Starts!');

  Counts  := ParamCount();

  try
    MyIni := TIniFile.Create( ChangeFileExt( INIFILENAME, INIEXTNAME ) );  
    FCamFormat := MyIni.ReadInteger(INISECTION, INIENTRY, FCamFormat);
    FDelayTime := MyIni.ReadInteger(INISECTION, INIENTRYDELAY, FDelayTime);
  finally
    MyIni.Free();
  end;

{$IF FALSE}
  if Counts <> 0 then
  begin
    num := StrToInt(ParamStr(1));

    case num of
      0..3 : FRotate := num;
    else
      FRotate := 0;
    end;
    str := Format('+ RoboCam Rotate Set : %d',[FRotate]);
    OutputDebugString(PChar(str));
  end;
{$IFEND}
  if Counts = 1 then
  begin
    num := StrToInt(ParamStr(1));
    FRotate := num;
    str := Format('+ RoboCam Rotate Set : %d',[FRotate]);
    OutputDebugString(PChar(str));
  end;

  if Counts = 2 then
  begin
    num := StrToInt(ParamStr(1));
    FRotate := num;
    str := Format('+ RoboCam Rotate Set : %d',[FRotate]);
    OutputDebugString(PChar(str));
    num := StrToInt(ParamStr(2));
    if num = 0  then
    begin
       FPreview := False;
       str := '+ RoboCam Preview Set : False';
    end else
    begin
       FPreview := True;
       str := '+ RoboCam Preview Set : True';
    end;
    OutputDebugString(PChar(str));
  end;

  TimerSubTitle.Enabled := False;
  TimerMinimize.Enabled := False;
  TimerAWB.Enabled      := False;

  dir := ExtractFilePath(Application.ExeName);
  FAppDir         := dir;
  str := Format('RoboCam Current Dir: %s',[FAppDir]);
  OutputDebugString(PChar(str));

  VideoMediaTypes := TEnumMediaType.Create;
  if CheckProfileFiles(dir) = True then
  begin
    bVidInit := UpdateVideoDevice();
    if ( bVidInit = True ) then
    begin
      bAudInit := UpdateAudioDevice();
      if ( bAudInit = True ) then
      begin
        FSubtitle   := nil;
        FSubtitle   := TSubtitle.Create();
        if FSubtitle <> nil then
             bSubInit := True;
        if ( bSubInit = True ) then
        begin
          FSubtitle.CPUProfile := FProfile;

          UpdateProfileMenu();
          UpdateCaptureMenu();

          FaDevNum   :=  0;
          FvDevNum   :=  0;
          FcProfile  :=  0;
          FmState    :=  1;

          if (vSysDev.CountFilters > 0) and (aSysDev.CountFilters > 0) then
          begin
            if bAudInit = True then aDevices.Items[FaDevNum].Checked := True;
            if bVidInit = True then vDevices.Items[FvDevNum].Checked := True;
            Profiles.Items[FcProfile].Checked:= True;
            if FvFocus = 0 then
               Capture.Items[FvFocusIdx].Checked := False
            else
               Capture.Items[FvFocusIdx].Checked := True;
          end;
          Capture.Items[0].Enabled := True;
          Capture.Items[1].Enabled := False;
          Capture.Items[2].Enabled := False;

          FillChar( FNotifyIconData, sizeof(TNotifyIconData), 0 );
          FNotifyIconData.cbSize := Sizeof(FNotifyIconData);
          FNotifyIconData.Wnd    := Handle;
          FNotifyIconData.uID    := 255;
          FNotifyIconData.uFlags := NIF_MESSAGE + NIF_ICON + NIF_TIP;
          FNotifyIconData.uCallbackMessage := WM_USER_NOTIFYICON;
          FNotifyIconData.hIcon  := Application.Icon.Handle;
          FNotifyIconData.szTip  := 'RoboCam';

          Application.HookMainWindow( AppHookProc );
          Application.OnMessage := OnAppMessage;

          FOnMinimize := Application.OnMinimize;
          Application.OnMinimize := OnCamMinimize;

          TimerSubTitle.Enabled := False;
          TimerSubTitle.Interval:= TIMERINTV;

          TimerMinimize.Interval:= 500;
          TimerMinimize.Enabled := True;
{$IFDEF DEF_CAPTURE_BEEP}
          SetStopRecordingTimer(False);
{$ENDIF}
        end else
        begin
          //Application.MessageBox('Cannot Init Subtitle!', 'RoboCam');
          PostMessage( Handle, WM_CLOSE, 0,0);
        end;
      end else
      begin
          //Application.MessageBox('Cannot Init Audio!', 'RoboCam');
          PostMessage( Handle, WM_CLOSE, 0,0);
      end;
    end else
    begin
      //Application.MessageBox('Cannot Init Video!', 'RoboCam');
      PostMessage( Handle, WM_CLOSE, 0,0);
    end;
  end else
  begin
    //Application.MessageBox('Profile error!', 'RoboCam');
    PostMessage( Handle, WM_CLOSE, 0,0);
  end;

end;

procedure TVideoForm.SetStopRecordingTimer(sw : Boolean);
begin
  TimerStopRecording.Enabled  := sw;
  if FDelayTime = 0 then
     TimerStopRecording.Interval := 5
  else
     TimerStopRecording.Interval := FDelayTime;

  if FDelayTime <> 0 then
     FOriginCap := APPNAME + ' [Stop delay]'
  else
     FOriginCap := APPNAME;

  Caption := FOriginCap;
end;

procedure TVideoForm.FormDestroy(Sender: TObject);
  var MyIni : TIniFile;
begin
  vDevices.Clear();
  vDevices.Free();

  aDevices.Clear();
  aDevices.Free();

  Profiles.Clear();
  Profiles.Free();

  Capture.Clear();
  Capture.Free();

  if Assigned(FSubtitle) then
  begin
    FSubtitle.Clear();
    FSubtitle.Free();
  end;

  Shell_NotifyIcon(NIM_DELETE, @FNotifyIconData);
  Application.UnHookMainWindow(AppHookProc);

  try
    MyIni := TIniFile.Create( ChangeFileExt( INIFILENAME, INIEXTNAME ) );  
    MyIni.WriteInteger(INISECTION, INIENTRY, FCamFormat);
    MYini.WriteInteger(INISECTION, INIENTRYDELAY, FDelayTime);
  finally
    MyIni.Free();
  end;

  OutputDebugString('+ RoboCam Closed!');
end;

procedure TVideoForm.OnSelectVideoDevice(sender: TObject);
begin
  vDevices.Items[FvDevNum].Checked := False;
  FvDevNum := TMenuItem(Sender).Tag;
  vDevices.Items[FvDevNum].Checked := True;
end;

procedure TVideoForm.OnSelectAudioDevice(sender: TObject);
begin
  aDevices.Items[FaDevNum].Checked := False;
  FaDevNum := TMenuItem(Sender).Tag;
  aDevices.Items[FaDevNum].Checked := True;;
end;

procedure TVideoForm.OnSelectProfiles(sender: TObject);
begin
  Profiles.Items[FcProfile].Checked := False;
  FcProfile := TMenuItem(Sender).Tag;
  Profiles.Items[FcProfile].Checked := True;
end;

procedure TVideoForm.OnSelectCapture(sender: TObject);
  var OriginalStage, SelectStage : Integer;
begin

  OriginalStage := FmState;
  SelectStage   := TMenuItem(Sender).Tag;

  case SelectStage of
    0:begin         //Start Capture
        if FmState <> SelectStage then
        begin
           FmState := SelectStage;
           Capture.Items[0].Enabled  := False;
           Capture.Items[1].Enabled  := True;
           Capture.Items[2].Enabled  := True;
           Capture.Items[14].Enabled := False;
           vDevices.Enabled          := False;
           aDevices.Enabled          := False;
           Profiles.Enabled          := False;
        end;
      end;
    1:begin         //Stop Capture
        if FmState <> SelectStage then
        begin
           FmState := SelectStage;
           Capture.Items[0].Enabled  := True;
           Capture.Items[1].Enabled  := False;
           Capture.Items[2].Enabled  := False;
           Capture.Items[14].Enabled := True;
           vDevices.Enabled          := True;
           aDevices.Enabled          := True;
           Profiles.Enabled          := True;
        end;
      end;
  end;

  case FmState of
  0:begin
      if SaveDialog.Execute() then
      begin
        FFileName := SaveDialog.FileName;
        if not StartCapture(FFileName) then
        begin
            //Capture.Items[FmState].Checked := False;
            //FmState := OriginalStage;
            //Capture.Items[FmState].Checked := True;
            ShowMessage('Cannot Open devices/filters!');

            Capture.Items[FmState].Enabled := True;
            FmState := OriginalStage;
            Capture.Items[FmState].Enabled := False;
            Capture.Items[2].Enabled       := False;
            vDevices.Enabled         := True;
            aDevices.Enabled         := True;
            Profiles.Enabled         := True;

            FFileName := '';
        end;
      end else begin
                  FmState := OriginalStage;
                  Capture.Items[0].Enabled := True;
                  Capture.Items[1].Enabled := False;
                  Capture.Items[2].Enabled := False;
                  FFileName := '';

                  vDevices.Enabled         := True;
                  aDevices.Enabled         := True;
                  Profiles.Enabled         := True;
               end;
    end;
  1:begin
      StopCapture();
    end;
  end;

end;

procedure TVideoForm.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin

  FilterGraph.Stop();
  FilterGraph.ClearGraph();
  FilterGraph.Active := False;

  TimerSubTitle.Enabled := False;
  TimerMinimize.Enabled := False;

  if (Assigned(FSubtitle)) and
     (FSubtitle.isRecording = True) then
  begin
    FSubtitle.Finish();
    FSubtitle.SaveToFile();
  end;

  if vSysDev <> nil then
  begin
     vSysDev.Free();
  end;

  if aSysDev <> nil then
  begin
     aSysDev.Free();
  end;

  vFilter.Free();
  aFilter.Free();
  rFilter.Free();
  rFilter2.Free();

  VideoMediaTypes.Free;
end;

function TVideoForm.StartCapture(FileName : String) : Boolean;
  var msg, elapse   : String;
      start, finish : Cardinal;
      PinList       : TPinList;
      str           : String;
begin
  OutputDebugString('>> Receiving [CAPTURE] command.');

  if FCamMute = True then
  begin
    OutputDebugString('>> RoboCam Muted, Exit!');
    Result := False;
    Exit;
  end;

  start := GetTickCount();

  FilterGraph.Stop();
  FilterGraph.ClearGraph();
  FilterGraph.Active := False;
{$IFDEF DEF_VID_LOG}
  FilterGraph.LogFile := GetTimeStampLogName();
{$ENDIF}

  FEvent := '';
  FStep  := '';
  FError := '';
  FSubtitle.Clear();
  FSubtitle.SetMovieName(FileName);

  case FcProfile of
    0..26:begin
            ASFWriter.CustomProfileName :='';
            ASFWriter.Profile := TWMPofiles8(FcProfile);
          end;
    27:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_HI;
       end;
    28:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_Mi;
       end;
    29:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_LO;
       end;
    30:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_HI_VO;
       end;
    31:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_Mi_VO;
       end;
    32:begin
          ASFWriter.CustomProfileName := FAppDir +'\' +PROFILE_ROBO_LO_VO;
       end;
  end;
  str := Format('RoboCam CustomProfile: %s',[ASFWriter.CustomProfileName]);
  OutputDebugString(PChar(str));

  ASFWriter.FileName         := FileName;
  ASFWriter.Port             := FipPort;
  vFilter.BaseFilter.Moniker := vSysDev.GetMoniker(FvDevNum);
  aFilter.BaseFilter.Moniker := aSysDev.GetMoniker(FaDevNum);
  FilterGraph.Active         := True;

  if FCamFormat <> -1 then
  begin
    try
      PinList := TPinList.Create(vFilter as IBaseFilter);
      VideoMediaTypes.Assign(PinList.First);
      with (PinList.First as IAMStreamConfig) do
           SetFormat(VideoMediaTypes.Items[FCamFormat].AMMediaType^);
    finally
      PinList.Free;
    end;
  end;

  FSetFocus(FvFocus);

  case FcProfile of
    15..21:begin       // Audio Only Profile
            try
              with FilterGraph as ICaptureGraphBuilder2 do
              begin
                CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil, aFilter as IBaseFilter, nil, ASFWriter as IbaseFilter));
              end;
              SetPriorityLevel( PRIORITY_CAPTURE );
              FilterGraph.Play();
              Caption      := Caption + '  recording [Audio Only]';
              Result       := True;
              FPlaying     := True;
              FStartTick   := GetTickCount();
              msg := Format('>> RoboCam starts capturing at %s profile: %d',[FileName, FcProfile]);
              TimerSubtitle.Enabled := True;
            except
              Caption      := FOriginCap;
              Result       := False;
              FPlaying     := False;
              FStartTick   := 0;
              msg := '>> RoboCam fails to start capturing!';
              TimerSubtitle.Enabled := False;
              SetPriorityLevel( PRIORITY_NORMAL );
            end;
           end;
    22..23,
    30..32:begin       // Video Only Profile
            try
              with FilterGraph as ICaptureGraphBuilder2 do
              begin
                if FRotate = -1 then
                begin
                  CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                             vFilter as IBaseFilter,
                                             nil,
                                             ASFWriter as IBaseFilter));
                  if FPreview = True then
                  begin
                    CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                               vFilter as IBaseFilter,
                                               nil,
                                               VideoWindow as IBaseFilter));
                  end;
                end else if FRotate = 0 then
                begin
                  CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                             vFilter as IBaseFilter,
                                             nil,
                                             ASFWriter as IBaseFilter));
                  if FPreview = True then
                  begin
                    CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                               vFilter as IBaseFilter,
                                               SampleGrabber as IBaseFilter,
                                               VideoWindow as IBaseFilter));
                  end;
                end else
                begin
                  CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                             vFilter as IBaseFilter,
                                             rFilter2 as IBaseFilter,
                                             ASFWriter as IBaseFilter));
                  if FPreview = True then
                  begin
                    CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                               vFilter as IBaseFilter,
                                               rFilter as IBaseFilter,
                                               VideoWindow as IBaseFilter));
                  end;
                end;

                SetRotateFilter();

              end;
              SetPriorityLevel( PRIORITY_CAPTURE );
              FilterGraph.Play();
              Caption      := Caption + '  recording [Video Only]';
              Result       := True;
              FPlaying     := True;
              FStartTick   := GetTickCount();
              msg := Format('>> RoboCam starts capturing at %s profile: %d',[FileName, FcProfile]);
              TimerSubtitle.Enabled := True;
              TimerAWB.Enabled      := True;
              FSubtitle.Start();
              SetAWBalance( True );
            except
              Caption      := FOriginCap;
              Result       := False;
              FPlaying     := False;
              FStartTick   := 0;
              msg := '>> RoboCam fails to start capturing!';
              TimerSubtitle.Enabled := False;
              TimerAWB.Enabled      := False;
              SetPriorityLevel( PRIORITY_NORMAL );
            end;
           end;
  else
    try
      with FilterGraph as ICaptureGraphBuilder2 do
      begin
        if FRotate = -1 then
        begin
          CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                     vFilter as IBaseFilter,
                                     nil,
                                     ASFWriter as IBaseFilter));
          if FPreview = True then
          begin
            CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                       vFilter as IBaseFilter,
                                       nil,
                                       VideoWindow as IBaseFilter));
          end;
        end else if FRotate = 0 then
        begin
          CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                     vFilter as IBaseFilter,
                                     nil,
                                     ASFWriter as IBaseFilter));
          if FPreview = True then
          begin
            CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                       vFilter as IBaseFilter,
                                       SampleGrabber as IBaseFilter,
                                       VideoWindow as IBaseFilter));
          end;
        end else
        begin
          CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                     vFilter as IBaseFilter,
                                     rFilter2 as IBaseFilter,
                                     ASFWriter as IbaseFilter));
          if FPreview = True then
          begin
            CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil,
                                       vFilter as IBaseFilter,
                                       rFilter as IBaseFilter,
                                       VideoWindow as IBaseFilter));
          end;
        end;

        CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil,
                                   aFilter as IBaseFilter, nil,
                                   ASFWriter as IbaseFilter));

        SetRotateFilter();

      end;
      SetPriorityLevel( PRIORITY_CAPTURE );
      FilterGraph.Play();
      Caption     := FOriginCap + '  recording';
      Result      := True;
      FPlaying    := True;
      FStartTick  := GetTickCount();
      msg := Format('>> RoboCam starts capturing at %s profile: %d',[FileName, FcProfile]);
      TimerSubtitle.Enabled := True;
      TimerAWB.Enabled      := True;
      FSubtitle.Start();
      SetAWBalance( True );
    except
      Caption     := FOriginCap;
      Result      := False;
      FPlaying    := False;
      FStartTick  := 0;
      msg := '>> RoboCam fails to start capturing!';
      TimerSubtitle.Enabled := False;
      TimerAWB.Enabled      := False;
      SetPriorityLevel( PRIORITY_NORMAL );
    end;
  end;
  finish := abs(GetTickCount() - start );
  elapse := Format('>> RoboCam Camera Open Time : %d ms',[finish]);
  OutputDebugString( PChar( elapse ) );
  OutputDebugString( PChar( msg ) );
end;

function TVideoForm.StopCapture() : Boolean;
begin
  OutputDebugString('>> Receiving [STOP] command.');

  FilterGraph.Stop();
  FilterGraph.ClearGraph();
  FilterGraph.Active := False;
  Caption     := FOriginCap;
  FPlaying    := False;
  FStartTick  := 0;
  FRotateInf  := nil;

  TimerSubtitle.Enabled := False;
  TimerAWB.Enabled      := False;
  if FSubtitle.isRecording = True then
  begin
    FSubtitle.Finish();
    FSubtitle.SaveToFile();
  end;
  SetPriorityLevel( PRIORITY_NORMAL );

  OutputDebugString('>> RoboCam stops!');

  Result      := True;
end;

procedure TVideoForm.OnAppMessage(var Msg: tagMSG; var Handled: Boolean);
  var ClipBoard              : TClipboard;
      str                    : String;
begin

  if Msg.message = ROBO_REMOTE_START then
  begin

    if TimerStopRecording.Enabled = True then
    begin
      TimerStopRecordingTimer( self );
    end;

    if FPlaying = True then
    begin
      OutputDebugString('>> RoboCam Capturing in progress...');
      Handled := True;
      Exit;
    end else begin

    //PopShowClick(self);
    RefreshScreen();
    Application.ProcessMessages();

    try
      Clipboard := TClipboard.Create();
      str := ClipBoard.AsText;
      ClipBoard.Clear();
      Clipboard.Free();
    except
      ClipBoard.Clear();
      Clipboard.Free();
      OutputDebugString('>> RoboCam fail to receive the file name!');
      Handled := True;
      Exit;
    end;

    Handled := StartCapture( str );

    if Handled = True then
    begin
      Capture.Items[0].Enabled := False;
      Capture.Items[1].Enabled := True;
      Capture.Items[2].Enabled := True;

      vDevices.Enabled         := False;
      aDevices.Enabled         := False;
      Profiles.Enabled         := False;
    end;

    PostMessage( Handle, WM_USER_MINIMIZE, 0, 0);
    end;
  end else if Msg.message = ROBO_REMOTE_STOP then
  begin
{$IFDEF DEF_CAPTURE_BEEP}
    SetStopRecordingTimer(True);
    Handled := True;
{$ELSE}
    Handled := StopCapture();
    if Handled = True then
    begin
      Capture.Items[0].Enabled := True;
      Capture.Items[1].Enabled := False;
      Capture.Items[2].Enabled := False;

      vDevices.Enabled         := True;
      aDevices.Enabled         := True;
      Profiles.Enabled         := True;
    end;
{$ENDIF}
  end else if Msg.message = ROBO_REMOTE_SNAPSHOT then
  begin
    OutputDebugString('RoboCam Receives <SNAPSHOT> command.');
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_PROFILE then
  begin
    Profiles.Items[FcProfile].Checked := False;
    FcProfile := Msg.wParam;
    if FcProfile < 0  then
       FcProfile := 0
    else if FcProfile > (Profiles.Count - 1) then
       FcProfile := Profiles.Count - 1;

    Profiles.Items[FcProfile].Checked := True;

    str := format('RoboCam Receives <SET_PROFILE> --> %d',[FcProfile]);
    OutputDebugString(PChar(str));

    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_VIDEO then
  begin
    OutputDebugString('RoboCam Receives <SET_VIDEO> command.');

    vDevices.Items[FvDevNum].Checked := False;
    FvDevNum := Msg.wParam;
    if FvDevNum < 0  then
       FvDevNum := 0
    else if FvDevNum > (vDevices.Count - 1) then
       FvDevNum := vDevices.Count - 1;
    vDevices.Items[FvDevNum].Checked := True;
    FCamFormat := Msg.lParam;

    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_AUDIO then
  begin
    OutputDebugString('RoboCam Receives <SET_AUDIO> command.');

    aDevices.Items[FaDevNum].Checked := False;
    FaDevNum := Msg.wParam;
    if FaDevNum < 0  then
       FaDevNum := 0
    else if FaDevNum > (aDevices.Count - 1) then
       FaDevNum := aDevices.Count - 1;
    aDevices.Items[FaDevNum].Checked := True;

    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_EVENT then
  begin
    SetEventCaption(Msg.wParam);
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_STEP then
  begin
    SetStepCaption(Msg.wParam);
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_ERROR then
  begin
    SetErrorCaption();
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_CLOSE then
  begin
    StopCapture();
    PostMessage( Handle, WM_CLOSE, 0, 0 );
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_FOCUS then
  begin
    FvFocus := Msg.wParam;

    if FvFocus = 0 then
    begin
       Capture.Items[FvFocusIdx].Checked := False;
       OutputDebugString('RoboCam Receives <SET_FOCUS> command : OFF');
    end else begin
       Capture.Items[FvFocusIdx].Checked := True;
       OutputDebugString('RoboCam Receives <SET_FOCUS> command : ON');
    end;
    FSetFocus := SetAutoFocus;
    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_MANFOCUS then
  begin
    FvFocus   := 0;
    FWrkFocus := Msg.wParam;

    Capture.Items[FvFocusIdx].Checked := False;
    OutputDebugString('RoboCam Receives <SET_FOCUS_MANUAL> command');

    FSetFocus := SetAutoFocus2;

    Handled := True;
  end else if Msg.message = ROBO_REMOTE_SET_QUADRANT then
  begin
    FQaudrant := Msg.wParam;
    OutputDebugString('RoboCam Receives <SET_QUADRANT> command');
    Handled   := True;
  end else if Msg.message = ROBO_REMOTE_ENABLE_CAM then
  begin
    OutputDebugString('RoboCam Receives <CAM_ENABLE> command');
    FCamMute  := False;
    Handled   := True;
  end else if Msg.message = ROBO_REMOTE_DISABLE_CAM then
  begin
    OutputDebugString('RoboCam Receives <CAM_DISABLE> command');
    FCamMute  := True;
    Handled   := True;
  end;
end;

function TVideoForm.AppHookProc(var Message: TMessage) : Boolean;
  var str       : String;
      Clipboard : TClipboard;
begin
  Result := False;

  if Message.Msg = WM_POWERBROADCAST then
  begin
    if Message.WParam = PBT_APMSUSPEND then
    begin
      StopCapture();
      Message.Result := 1;
      Result := True;
      Application.Terminate();
    end;
  end;

  if Message.Msg = ROBO_REMOTE_START then
  begin

    if TimerStopRecording.Enabled = True then
    begin
      TimerStopRecordingTimer( self );
    end;

    if FPlaying = True then
    begin
      OutputDebugString('>> RoboCam Capturing in progress...');
      Result := True;
      Message.Result := 2;
    end else begin

    //PopShowClick(self);
    RefreshScreen();
    Application.ProcessMessages();

    try
      Clipboard := TClipboard.Create();
      str := ClipBoard.AsText;
      ClipBoard.Clear();
      Clipboard.Free();
    except
      ClipBoard.Clear();
      Clipboard.Free();
      OutputDebugString('>> RoboCam Fail to receive the file name!');
      Result := True;
      Message.Result := 3;
      Exit;
    end;

    if StartCapture( str ) = True then
    begin
      Capture.Items[0].Enabled := False;
      Capture.Items[1].Enabled := True;
      Capture.Items[2].Enabled := True;

      vDevices.Enabled         := False;
      aDevices.Enabled         := False;
      Profiles.Enabled         := False;

      Message.Result := 1;
    end else begin
      Capture.Items[0].Enabled := True;
      Capture.Items[1].Enabled := False;
      Capture.Items[2].Enabled := False;

      vDevices.Enabled         := True;
      aDevices.Enabled         := True;
      Profiles.Enabled         := True;

      Message.Result := 0;
    end;

    PostMessage( Handle, WM_USER_MINIMIZE, 0, 0);

    Result := True;
    end;

  end else if Message.Msg = ROBO_REMOTE_STOP then
  begin
{$IFDEF  DEF_CAPTURE_BEEP}
    SetStopRecordingTimer(True);
{$ELSE}
    if StopCapture() = True then
    begin
      Capture.Items[0].Enabled := True;
      Capture.Items[1].Enabled := False;
      Capture.Items[2].Enabled := False;

      vDevices.Enabled         := True;
      aDevices.Enabled         := True;
      Profiles.Enabled         := True;
    end;
{$ENDIF}
    Message.Result := 1;
    Result := True;

  end else if Message.Msg = ROBO_REMOTE_SNAPSHOT then
  begin
    Clipboard := TClipboard.Create();
    str := ClipBoard.AsText;
    ClipBoard.Clear();
    Clipboard.Free();
    if FPlaying = True then
    begin
      OutputDebugString('RoboCam Receives <SNAPSHOT> command.');
      if TakeSnapshot(AnsiString(str)) = True then
         Message.Result := 1
      else
         Message.Result := 2;
    end else begin
      OutputDebugString('RoboCam Receives <SNAPSHOT> command, but cannot comply!');
      Message.Result := 0;
    end;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_PROFILE then
  begin

    Profiles.Items[FcProfile].Checked := False;
    FcProfile := Message.wParam;
    if FcProfile < 0  then
       FcProfile := 0
    else if FcProfile > (Profiles.Count - 1) then
       FcProfile := Profiles.Count - 1;
    Profiles.Items[FcProfile].Checked := True;

    str := format('RoboCam Receives <SET_PROFILE> --> %d',[FcProfile]);
    OutputDebugString(PChar(str));

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_VIDEO then
  begin
    OutputDebugString('RoboCam Receives <SET_VIDEO> command.');

    vDevices.Items[FvDevNum].Checked := False;
    FvDevNum := Message.wParam;
    if FvDevNum < 0  then
       FvDevNum := 0
    else if FvDevNum > (vDevices.Count - 1) then
       FvDevNum := vDevices.Count - 1;
    vDevices.Items[FvDevNum].Checked := True;
    FCamFormat := Message.LParam;

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_AUDIO then
  begin
    OutputDebugString('RoboCam Receives <SET_AUDIO> command.');

    aDevices.Items[FaDevNum].Checked := False;
    FaDevNum := Message.wParam;
    if FaDevNum < 0  then
       FaDevNum := 0
    else if FaDevNum > (aDevices.Count - 1) then
       FaDevNum := aDevices.Count - 1;
    aDevices.Items[FaDevNum].Checked := True;

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_EVENT then
  begin
    SetEventCaption(Message.WParam);

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_STEP then
  begin
    SetStepCaption(Message.WParam);

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_ERROR then
  begin
    SetErrorCaption();

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_CLOSE then
  begin
    StopCapture();
    PostMessage( Handle, WM_CLOSE, 0, 0 );
    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_FOCUS then
  begin
    FvFocus := Message.wParam;

    if FvFocus = 0 then
    begin
       Capture.Items[FvFocusIdx].Checked := False;
       OutputDebugString('RoboCam Receives <SET_FOCUS> command : OFF');
    end else begin
       Capture.Items[FvFocusIdx].Checked := True;
       OutputDebugString('RoboCam Receives <SET_FOCUS> command : ON');
    end;
    FSetFocus := SetAutoFocus;

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_MANFOCUS then
  begin
    FvFocus   := 0;
    FWrkFocus := Message.wParam;;

    Capture.Items[FvFocusIdx].Checked := False;
    OutputDebugString('RoboCam Receives <SET_FOCUS_MANUAL> command');

    FSetFocus := SetAutoFocus2;

    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_SET_QUADRANT then
  begin
    FQaudrant := Message.wParam;
    OutputDebugString('RoboCam Receives <SET_QUADRANT> command');
    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_ENABLE_CAM then
  begin
    OutputDebugString('RoboCam Receives <CAM_ENABLE> command');
    FCamMute  := False;
    Message.Result := 1;
    Result := True;
  end else if Message.Msg = ROBO_REMOTE_DISABLE_CAM then
  begin
    OutputDebugString('RoboCam Receives <CAM_DISABLE> command');
    FCamMute  := True;
    Message.Result := 1;
    Result := True;
  end;
end;

procedure TVideoForm.SetEventCaption(EventNum : Integer);
begin
    FEvent := RoboEventString[EventNum];
    OutputDebugString( PChar('RoboCam Receives [EVENT] ' + FEvent));
end;

procedure TVideoForm.SetStepCaption(StepNum : Integer);
begin
    if StepNum < 11 then
       FStep := RoboStepString[StepNum]
    else
       FStep := 'None';

    OutputDebugString( PChar('RoboCam Receives [STEP] ' + FStep));
end;

procedure TVideoForm.SetErrorCaption();
begin
    FError  := 'Error!';
    OutputDebugString( PChar('RoboCam Receives [ERROR] ' + FError));
end;

procedure TVideoForm.OnSelectSnapshot(sender: TObject);
begin

  if SaveSnapDialog.Execute() then
  begin
    TakeSnapshot(AnsiString(SaveSnapDialog.FileName));
  end;

end;

function TVideoForm.TakeSnapshot(FileName : AnsiString) : Boolean;
  var width, height, size : Cardinal;
      pBuffer             : PByte;
      str,jpeg            : String;
      hDLL                : THandle;
      fPtr                : Pointer;
      SaveJPEGImage       : TSaveJpeg;
      rtn                 : Boolean;
      myJpeg              : TJpegImage;
begin
  if FRotate < 0 then
  begin
     ShowMessage('RoboCam Screenshot disabled! Try Later...');
     rtn := False;
  end
  else if FRotate > 0 then
  begin
    rtn  := False;
    jpeg := ExtractFilePath(Application.ExeName) + JPEGENGINENAME;
    hDLL := LoadLibrary( PChar(jpeg) );
    if hDLL <> 0 then
    begin
      fPtr := GetProcAddress( hDLL, PChar(JPEGFUNCNAME) );
      SaveJPEGImage := TSaveJpeg(fPtr);
      ZeroMemory( @ImgSpace, Sizeof(ImgSpace) );
      pBuffer := @ImgSpace;
      if FRotateInf2.GetRGB24( pBuffer,@size,@width,@height ) = S_OK then
      begin
        str := Format('[Snapshot: Buffer Size %d, Width %d, Height %d]',[size, width, height]);
        OutputDebugString( PChar(str) );
        SaveJPEGImage( pBuffer, width, height, JPEGCOMPRATIO,PAnsiChar(FileName));
        rtn := True;
      end else
      begin
        ShowMessage('RoboCam Busy! Try Later...');
        rtn := False;
      end;
      FreeLibrary( hDLL );
    end else
    begin
      ShowMessage('RoboCam cannot locate "JpegDLL.DLL" file!');
      rtn := False;
    end;
    Result := rtn;
  end else if FRotate = 0 then
  begin
    rtn := SampleGrabber.GetBitmap(Image.Picture.Bitmap);
    myJpeg := TJpegImage.Create;
    if myJpeg <> nil then
    begin
      myJpeg.Assign(Image.Picture.Bitmap);
      myJpeg.SaveToFile(FileName);
      myJpeg.Free;
    end else
    begin
      rtn := False;
    end;
    Result := rtn;
  end;
end;

procedure TVideoForm.TimerAWBTimer(Sender: TObject);
begin
  TimerAWB.Enabled := False;
  SetAWBalance( False );
end;

procedure TVideoForm.TimerMinimizeTimer(Sender: TObject);
begin
  TimerMinimize.Enabled := False;
  OnCamMinimize(Sender);
end;

procedure TVideoForm.TimerStopRecordingTimer(Sender: TObject);
begin
  OutputDebugString('>> RoboCam TimerStopRecordingTimer Event');
  TimerStopRecording.Enabled := False;
{$IFDEF DEF_CAPTURE_BEEP}
  if StopCapture() = True then
  begin
      Capture.Items[0].Enabled := True;
      Capture.Items[1].Enabled := False;
      Capture.Items[2].Enabled := False;

      vDevices.Enabled         := True;
      aDevices.Enabled         := True;
      Profiles.Enabled         := True;
  end;
{$ENDIF}
end;

procedure TVideoForm.TimerSubTitleTimer(Sender: TObject);
  var str : String;
begin

  if FQaudrant = -1 then
     str := Format('%s %s %s %s',[GetTimeStamp, FEvent, FStep, FError])
  else
     str := Format('%s %s %s %s Q:%d',[GetTimeStamp, FEvent, FStep, FError, FQaudrant]);

  FSubtitle.SetSubtitle( str );
end;

procedure TVideoForm.SetPreview( sw : Boolean );
begin
  FPreview := sw;
end;

procedure TVideoForm.SetAWBalance(sw:Boolean);
  var Min, Max, Delta, Default       : Integer;
      Flag                           : tagVideoProcAmpFlags;
      Hr                             : Cardinal;
      msg                            : String;
begin
  Hr := vFilter.QueryInterface(IID_IAMVideoProcAmp, FAWBFilter);
  if Hr = 0 then
  begin
    FAWBFilter.Get(VideoProcAmp_WhiteBalance,
                   Default,
                   Flag);
    if Flag = VideoProcAmp_Flags_Manual then
       msg := Format('+++RoboCam- AWB value Get: ON (%d)',[Default])
    else
       msg := Format('+++RoboCam- AWB value Get: OFF (%d)',[Default]);
    OutputDebugString(PChar(msg));

    case sw of
    True: begin
          Hr := FAWBFilter.Set_(VideoProcAmp_WhiteBalance,
                                Default,
                                VideoProcAmp_Flags_Auto);
          if Hr = 0 then
          begin
            Capture.Items[12].Checked := sw;
            FAWBalance                := sw;
            msg := Format('+++RoboCam- AWB value Set: ON (%d)', [Default]);
            OutputDebugString(PChar(msg));
          end;
          end;
    False:begin
          Hr := FAWBFilter.Set_(VideoProcAmp_WhiteBalance,
                                Default,
                                VideoProcAmp_Flags_Manual);
          if Hr = 0 then
          begin
            Capture.Items[12].Checked := sw;
            FAWBalance                := sw;
            msg := Format('+++RoboCam- AWB value Set: OFF (%d)', [Default]);
            OutputDebugString(PChar(msg));
          end;
        end;
    end;
  end;
end;

procedure TVideoForm.SetAutoFocus(sw: Integer);
  var Min, Max, Delta, Flag     : Integer;
      Hr                        : Cardinal;
begin
  Hr := vFilter.QueryInterface(IID_IAMCameraControl, FAFFilter);
  if Hr = 0 then
  begin
     FAFFilter.GetRange(
                         CameraControl_Focus,
                         Min,
                         Max,
                         Delta,
                         FDefFocus,
                         Flag
                       );
     FAFFilter.GetRange(
                         CameraControl_Exposure,
                         Min,
                         Max,
                         Delta,
                         FDefExp,
                         Flag
                       );

    case sw of
      1: begin
              FAFFilter.Set_(
                              CameraControl_Focus,
                              FDefFocus,
                              CameraControl_Flags_Manual
                            );
{$IF FALSE}
              FAFFilter.Set_(
                              CameraControl_Exposure,
                              FDefExp,
                              CameraControl_Flags_Manual
                            );
{$IFEND}
            end;
      0:begin
              FAFFilter.Set_(
                              CameraControl_Focus,
                              FDefFocus,
                              CameraControl_Flags_Auto
                            );
{$IF FALSE}
              FAFFilter.Set_(
                              CameraControl_Exposure,
                              FDefExp,
                              CameraControl_Flags_Auto
                            );
{$IFEND}
            end;
    end;

  end;
end;

procedure TVideoForm.SetAutoFocus2(sw: Integer);         //For Manual Focus
  var Min, Max, Delta, Flag     : Integer;
      Hr                        : Cardinal;

begin
  Hr := vFilter.QueryInterface(IID_IAMCameraControl, FAFFilter);
  if Hr = 0 then
  begin
     FAFFilter.GetRange(
                         CameraControl_Focus,
                         Min,
                         Max,
                         Delta,
                         FDefFocus,
                         Flag
                       );

     if FWrkFocus < Min then
             FWrkFocus := Min;

     if FWrkFocus > Max then
             FWrkFocus := Max;

     FAFFilter.GetRange(
                         CameraControl_Exposure,
                         Min,
                         Max,
                         Delta,
                         FDefExp,
                         Flag
                       );

    case sw of
      1: begin
              FAFFilter.Set_(
                              CameraControl_Focus,
                              FWrkFocus,
                              CameraControl_Flags_Manual
                            );
{$IF FALSE}
              FAFFilter.Set_(
                              CameraControl_Exposure,
                              FDefExp,
                              CameraControl_Flags_Manual
                            );
{$IFEND}
            end;
      0:begin
              FAFFilter.Set_(
                              CameraControl_Focus,
                              FWrkFocus,
                              CameraControl_Flags_Auto
                            );
{$IF FALSE}
              FAFFilter.Set_(
                              CameraControl_Exposure,
                              FDefExp,
                              CameraControl_Flags_Auto
                            );
{$IFEND}
            end;
    end;

  end;
end;

procedure TVideoForm.OnSelectVideoFocus(sender: TObject);
  var idx : Integer;
begin
  idx := TMenuItem(Sender).Tag;

  if Capture.Items[idx].Checked = True then
  begin
     Capture.Items[idx].Checked := False;
     FvFocus := 0;
  end else
  begin
     Capture.Items[idx].Checked := True;
     FvFocus := 1;
  end;
  FSetFocus := SetAutoFocus;

  FSetFocus(FvFocus);
end;

procedure TVideoForm.OnSelectWhiteBalance(sender: TObject);
begin
  if FAWBalance = True then
  begin
     SetAWBalance( False );
  end else
  begin
     SetAWBalance( True );
  end;
end;

procedure TVideoForm.OnSelectPreview(sender: TObject);
  var idx : Integer;
begin
  idx := TMenuItem(sender).Tag;
  if FPreview = True then
  begin
      SetPreview( False );
  end else
  begin
      SetPreview( True );
  end;
  Capture.Items[idx].Checked := FPreview;
end;

procedure TVideoForm.OnSelectFocusParam(sender: TObject);
  var DlgFocus : TfrmManualFocus;
begin
  FvFocus := 0;
  Capture.Items[FvFocusIdx].Checked := False;

  DlgFocus := TfrmManualFocus.Create(Self);

  if Assigned(DlgFocus) then
  begin
    DlgFocus.FocusValue := FWrkFocus;
    DlgFocus.ShowModal();
    FWrkFocus := DlgFocus.FocusValue;
    DlgFocus.Free();
    FSetFocus := SetAutoFocus2;
    FSetFocus(FvFocus);
  end;
end;

procedure TVideoForm.OnSelectCPUProfile(sender: TObject);
  var idx : Integer;
begin
  idx := TMenuItem(sender).Tag;

  if Capture.Items[idx].Checked = True then
  begin
     Capture.Items[idx].Checked := False;
     FProfile := False;
     FSubtitle.CPUProfile := FProfile;
  end else
  begin
     Capture.Items[idx].Checked := True;
     FProfile := True;
     FSubtitle.CPUProfile := FProfile;
  end;

end;

procedure TVideoForm.RefreshScreen();
begin
   ShowWindow( Application.Handle, SW_RESTORE );
   Application.ProcessMessages();
   ShowWindow( Application.Handle, SW_SHOW );
   Application.ProcessMessages();
   Shell_NotifyIcon(NIM_DELETE, @FNotifyIconData);
end;

procedure TVideoForm.PopShowClick(Sender: TObject);
begin
{$IFDEF DEF_POPUP_PREVIEW}
  RefreshScreen();
{$ENDIF}
end;

procedure TVideoForm.ProcTrayIcon(var msg: TMessage);
  var pt : TPoint;
begin
  if msg.LParam = WM_RBUTTONDOWN then
  begin
    GetCursorPos(pt);
    PopupMenu.Popup(pt.X, pt.Y);
  end;
end;

procedure TVideoForm.OnCamMinimize(sender: TObject);
begin
  FOnMinimize(sender);
  ShowWindow( Application.Handle, SW_MINIMIZE );
  ShowWindow( Application.Handle, SW_HIDE );
  Shell_NotifyIcon( NIM_ADD, @FNotifyIconData );
end;

procedure TVideoForm.OnUserMinimize(var msg: TMessage);
begin
  OnCamMinimize(self);
end;

procedure TVideoForm.OnSelectCamFormat(sender: TObject);
  var frmFormat : TfrmFormat;
      PinList   : TPinList;
      i         : Integer;
      str       : String;
begin
  frmFormat := TfrmFormat.Create(Self);

  if Assigned(frmFormat) then
  begin

    if FvDevNum <> -1 then
    begin
      FilterGraph.Stop();
      FilterGraph.ClearGraph();
      FilterGraph.Active := False;

      vFilter.BaseFilter.Moniker := vSysDev.GetMoniker(FvDevNum);
      FilterGraph.Active         := True;
      PinList := TPinList.Create(vFilter as IBaseFilter);

      VideoMediaTypes.Assign(PinList.First);

      frmFormat.ListFmt.Clear();
      for i := 0 to VideoMediaTypes.Count - 1 do
      begin
        str := VideoMediaTypes.MediaDescription[i];
        frmFormat.Listfmt.Items.Add(str);
      end;

      FilterGraph.Active := False;
      PinList.Free;
    end;

    if FCamFormat <> -1 then
       frmFormat.ListFmt.ItemIndex := FCamFormat;

    if frmFormat.ShowModal() = mrOK then
    begin
      FCamFormat := frmFormat.ListFmt.ItemIndex;
    end;

    frmFormat.Free();
  end;
end;

procedure TVideoForm.SetRotateFilter();
begin

  rFilter.QueryInterface(IRotateInf, FRotateInf);
  if FRotateInf <> nil then
    FRotateInf.SetRotate(FRotate);

  rFilter2.QueryInterface(IRotateInf, FRotateInf2);
  if FRotateInf2 <> nil then
    FRotateInf2.SetRotate(FRotate);

end;

end.
