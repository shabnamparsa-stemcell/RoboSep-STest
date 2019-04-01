; -- Components.iss --
; Demonstrates a components-based installation.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{153F539E-B953-4FCC-881F-C1AFD28AA575}
AppName=RoboSep-S
AppVerName=RoboSep-S
AppVersion=1.2.1.0
VersionInfoVersion=1.2.1.0
VersionInfoCompany=Stemcell Technologies
AppPublisher=Stemcell Technologies
AppPublisherURL=http://www.stemcell.com
AppSupportURL=http://www.stemcell.com
AppUpdatesURL=http://www.stemcell.com
OutputDir=.\output
OutputBaseFilename=RoboSep-S
Compression=lzma
SolidCompression=yes
DefaultDirName={pf}\STI\RoboSep
DisableProgramGroupPage=yes
DirExistsWarning=no
DisableDirPage=yes
RestartIfNeededByRun=no
Uninstallable=no

[Dirs]
Name: "{app}\archive"
Name: "{app}\videos"

[Files]
Source: ".\Bin\Update.dll";                 DestDir: "{app}"; flags: deleteafterinstall;   
source: ".\Bin\RoboSepSServer.exe";         DestDir: "{app}"; flags: deleteafterinstall;
source: ".\Bin\RoboSepSClient.msi";         DestDir: "{app}"; flags: deleteafterinstall;
source: ".\Bin\RoboSepSService.msi";        DestDir: "{app}"; flags: deleteafterinstall;            Tasks: MfcInstall; 
source: ".\Bin\RoboSepSProtocolEditor.msi"; DestDir: "{app}"; flags: deleteafterinstall;            
Source: ".\Bin\RoboSepProtocol.xsd";        DestDir: "{app}"; flags: deleteafterinstall;
source: ".\Bin\CRRuntime_32bit_13_0.msi";   DestDir: "{app}";           flags: deleteafterinstall;  Tasks: EmulInstall;          
source: ".\Bin\Language.ini";               DestDir: "{app}\Config";    flags: uninsneveruninstall
source: ".\Bin\RoboSepService.config";      DestDir: "{pf}\STI";        flags: uninsneveruninstall; Tasks: MfcInstall;
Source: ".\Bin\protocols.exe";              DestDir: "{app}\protocols"; flags: deleteafterinstall;  Tasks: EmulInstall;   
source: ".\Bin\hardware.ini";               DestDir: "{app}\Config";    flags: uninsneveruninstall; Tasks: EmulInstall;   
source: ".\Bin\serverConfig.ini";           DestDir: "{app}\Config";    flags: uninsneveruninstall; Tasks: EmulInstall;   
Source: ".\Bin\STEMhide.exe";               DestDir: "{app}\Bin";       flags: uninsneveruninstall; Tasks: MfcInstall;
Source: ".\Bin\KeyHook.dll";                DestDir: "{app}\Bin";       flags: uninsneveruninstall; Tasks: MfcInstall;
Source: ".\Bin\bcrtest.txt";                DestDir: "{app}\Config";    flags: uninsneveruninstall;
Source: ".\Bin\GUI.ini";                    DestDir: "{app}\Config";    flags: uninsneveruninstall;

[Icons]
Name: "{sendto}\Notepad"; Filename:"{sys}\notepad.exe"; WorkingDir: "{app}"; Flags: runmaximized;
;Name: "{commonstartup}\STEMhide"; Filename:"{app}\bin\STEMhide.exe"; WorkingDir: "{app}\bin"; Flags: runminimized; Tasks: MfcInstall;
Name: "{commonstartup}\STEMhide"; Filename:"{app}\bin\STEMhide.exe"; WorkingDir: "{app}\bin"; Tasks: MfcInstall;

[Run]
FileName: "{app}\RoboSepSServer.exe";                                             flags: waituntilterminated; afterInstall: UpdateSerialDat();
FileName: "msiexec.exe"; Parameters: "/i ""{app}\RoboSepSClient.msi""";           flags: waituntilterminated; 
FileName: "msiexec.exe"; Parameters: "/i ""{app}\CRRuntime_32bit_13_0.msi""";     flags: waituntilterminated; Tasks: EmulInstall;
FileName: "msiexec.exe"; Parameters: "/i ""{app}\RoboSepSService.msi""";          flags: waituntilterminated; Tasks: MfcInstall; Afterinstall: UpdateConfigFile();
;FileName: "msiexec.exe"; Parameters: "/i ""{app}\RoboSepSProtocolEditor.msi""";   flags: waituntilterminated; AfterInstall: CopyProtocolXSD();
Filename: "{app}\protocols\protocols.exe";                                        Tasks: EmulInstall;  AfterInstall: DeleteStartupLink();


[Tasks]
Name: MfcInstall;  Description: "Manufacture Install"; GroupDescription: "Install Type:"; Flags: exclusive;
Name: EmulInstall; Description: "Emulator Install: Please, unplug your network cable before the emulator starts! The emulator doens't support VPN.";    GroupDescription: "Install Type:"; Flags: exclusive unchecked;

[INI]
;Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "init_cmd2" ; String: "DECWIN1;DECLFT0;DECRGT42;DECBOT64;DECTOP20;SNPGAN4;SNPLED1;SHWNRD1;TRGSTO500.";
;Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "pre_cmd" ; String: "IMGSNP1P0E!";

;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "hemisphericalprofilefullvolume_14mlvial";      String: "1000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_14mlvial";        String: "13000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_14mlvial";        String: "85";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullheight_50mlvial";            String: "18";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullvolume_50mlvial";            String: "5000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_50mlvial";        String: "95";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_50mlvial";        String: "53000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_reagentvial";     String: "2250";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_reagentvial";     String: "43";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "deadVolumeprofiledeadvolume_bulkbottle";       String: "15000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume1_bulkbottle";     String: "300000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight1_bulkbottle";     String: "52";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "truncatedconicalprofilefullheight_bulkbottle"; String: "11";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "truncatedconicalprofilefullvolume_bulkbottle"; String: "5000";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullheight_bulkbottle";          String: "978";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullvolume_bulkbottle";          String: "3960";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight2_bulkbottle";     String: "41";
;Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume2_bulkbottle";     String: "48000";

;Filename: "{app}\Config\Hardware.ini"; Section: "Carousel";        Key: "homebackoffsteps";                             String: "800";
;Filename: "{app}\Config\Hardware.ini"; Section: "Carousel";        Key: "homingvelocityprofile";                       String: "1000, 1000, 1";

[Registry]
Root: HKCU; SubKey: "Console"; ValueType: dword;  ValueName: "ScreenBufferSize";  ValueData: $270f0050;

[Code]
procedure UpdateSerialNumber(FilePath, Serial : AnsiString); external 'UpdateSerialNumber@files:Update.dll stdcall';

var
  UserPage  : TInputQueryWizardPage;
  SerialNum : AnsiString;
  FilePath  : AnsiString;
procedure UpdateConfigFile();
  var CONFIGPATH : String;
      DESTPATH   : String;
begin
  CONFIGPATH := ExpandConstant('{pf}') + '\STI\RoboSepService.config';
  DESTPATH   := ExpandConstant('{pf}') + '\STI\RoboSepService\config\RoboSepService.config';
  FileCopy(CONFIGPATH, DESTPATH, False);
  DeleteFile(CONFIGPATH);
end;

procedure UpdateSerialDat();
begin
  FilePath := ExpandConstant('{app}')+'\config\Serial.dat';
  UpdateSerialNumber(PAnsiChar(FilePath), PAnsiChar(SerialNum) );
end;

procedure DeleteStartupLink();
  var File_Path : String;
begin
  File_Path := ExpandConstant('{commonstartup}')+'\Start RoboSep.lnk';
  DeleteFile(File_Path);
end;

procedure InitializeWizard;
begin
  UserPage := CreateInputQueryPage( wpWelcome,
                                    'RoboSep-S Unit Serial Number', 
                                    'Unit Serial Number',
                                    'Please specify a RoboSep-S unit serial number, then click Next.');
  UserPage.Add('Serial Number:', False);
  UserPage.Values[0] := '0000';
end;

procedure CopyProtocolXSD();
  var Src, Dst : String;
begin
  Src := ExpandConstant('{app}\RoboSepProtocol.xsd');
  Dst := ExpandConstant('{app}\..\RoboSepEditor\Config\RoboSepProtocol.xsd');
  FileCopy( Src, Dst, False );
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  l   : Integer;
  str : String;
  rtn : Boolean;
begin
  { Validate certain pages before allowing the user to proceed }
  rtn := False;
  if CurPageID = UserPage.ID then 
  begin
    str := UserPage.Values[0];
    l   := length(str);
    if l <> 4 then 
    begin
      MsgBox('Please define a serial number.', mbError, MB_OK);
      rtn := False;
    end else begin
      SerialNum := str;     
      rtn := True;
    end;
  end else
       rtn := True;
  Result := rtn;
end;
