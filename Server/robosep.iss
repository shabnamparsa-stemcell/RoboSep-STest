; -- robosep.iss --
; InnoSetup installer for the RoboSep server files (Project Tesla)
;
; This uses InnoSetup (http://www.jrsoftware.org/isinfo.php)
; Created with InnoSetup v4.2.0

[Setup]
AppName=RoboSep Server
AppVerName=RoboSep Server
OutputBaseFilename=RoboSepServer
;OutputBaseFilename=RoboSepServer
VersionInfoVersion=5.0.0.0
VersionInfoTextVersion=Production release version
VersionInfoCompany=STEMCELL TECHNOLOGIES
VersionInfoDescription=RoboSep Instrument Server
AppPublisher=STEMCELL TECHNOLOGIES
AppPublisherURL=http://www.stemcell.com/
;
DefaultDirName={pf}\STI\RoboSep
DisableProgramGroupPage=yes
Compression=lzma
SolidCompression=yes
; Don't bother warning us if the STI\RoboSep directory already exists
DirExistsWarning=no
; Keep the uninstall files tucked away
UninstallFilesDir={app}\uninst

[Dirs]
Name: "{app}\config"
Name: "{app}\images"
Name: "{app}\logs"; Permissions: everyone-full
Name: "{app}\logs\videolog";
Name: "{app}\logs\videoerrorlog";
Name: "{app}\reports"; Permissions: everyone-full
Name: "{app}\protocols"; Permissions: everyone-full
;Name: "C:\Share Data"; Permissions: everyone-full

[Files]
; Scripts and support library

;Source: "robosep.zip"; DestDir: "{app}\bin"
;
; This next line is commented out; it's for when we want to ship source code
; Source: "tesla\*"; DestDir: "{app}\bin\tesla\"; Excludes: "*.html"; Flags: recursesubdirs
;
;Source: "robosep.py"; DestDir: "{app}\bin"
Source: "Separator.exe.config.bak"; DestDir: "{app}\bin"
;Source: "tools\simple_client.py"; DestDir: "{app}\bin"
;Source: "tools\procheck.py"; DestDir: "{app}\bin"
;Source: "tools\schedeval.py"; DestDir: "{app}\bin"
;Source: "tesla\instrument\tests\HandsOnTest.py"; DestDir: "{app}\bin"
;Source: "tesla\instrument\tests\TipTest.py"; DestDir: "{app}\bin"
;Source: "tesla\instrument\tests\ZAxisTester.py"; DestDir: "{app}\bin" 
Source: "dist\RoboSepLauncher\*"; DestDir: "{app}\launcher"
Source: "dist\RoboSepLauncher\wx\*"; DestDir: "{app}\launcher\wx"

; Data files
Source: "tesla\control\cmd_times.ini"; DestDir: "{app}\config"
;Source: "tesla\instrument\hardware.ini"; DestDir: "{app}\config"; Flags: onlyifdoesntexist uninsneveruninstall
;Source: ".\data\serial.dat"; DestDir: "{app}\config"; Flags: confirmoverwrite uninsneveruninstall
Source: ".\data\errorConfig.txt"; DestDir: "{app}\config"
Source: ".\data\statusConfig.txt"; DestDir: "{app}\config"
Source: ".\data\Serial.dat"; DestDir: "{app}\config"
Source: ".\data\home_axes.xml"; DestDir: "{app}\bin"
Source: ".\data\prime.xml"; DestDir: "{app}\bin"
Source: ".\data\shutdown.xml"; DestDir: "{app}\bin"
Source: ".\data\STANDARD - Mouse Positive-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Mouse Negative-high recovery.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Mouse Negative-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human WB Positive-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human WB Negative.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human Positive-high recovery.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human Positive-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human Negative-high recovery.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human Negative-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human BC Positive-high recovery.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Human BC Positive-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\STANDARD - Any Species Positive-high purity.xml"; DestDir: "{app}\config"
Source: ".\data\protocolConfig.txt"; DestDir: "{app}\config"
Source: ".\data\images\RoboSep.png"; DestDir: "{app}\images"
Source: ".\data\wp.bmp"; DestDir: "{app}\bin"
Source: ".\data\wkhtmltopdf.exe"; DestDir: "{app}\bin"
Source: ".\data\libeay32.dll"; DestDir: "{app}\bin"
Source: ".\data\libgcc_s_dw2-1.dll"; DestDir: "{app}\bin"
Source: ".\data\mingwm10.dll"; DestDir: "{app}\bin"
Source: ".\data\ssleay32.dll"; DestDir: "{app}\bin"
Source: ".\data\RotateTrans.ax"; DestDir: "{app}\bin"
Source: ".\data\JpegDLL.dll"; DestDir: "{app}\bin"
Source: ".\data\RoboHi.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboMi.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboLo.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboHiVideoOnly.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboMiVideoOnly.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboLoVideoOnly.prx";  DestDir: "{app}\bin"
Source: ".\data\RoboCam.exe"; DestDir: "{app}\bin"
Source: ".\data\Form_SplashScreen.exe"; DestDir: "{app}\bin" 
Source: ".\data\codec.bat"; DestDir: "{app}\bin"; 
Source: ".\data\AGENCYB_0.TTF";           DestDir: "{fonts}" ; FontInstall: "Agency FB"    ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\arial.ttf";               DestDir: "{fonts}" ; FontInstall: "Arial"        ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\arialbd.ttf";             DestDir: "{fonts}" ; FontInstall: "Arial"        ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\arialbi.ttf";             DestDir: "{fonts}" ; FontInstall: "Arial"        ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ariali.ttf";              DestDir: "{fonts}" ; FontInstall: "Arial"        ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ARIALN.TTF";              DestDir: "{fonts}" ; FontInstall: "Arial Narrow" ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ARIALNB.TTF";             DestDir: "{fonts}" ; FontInstall: "Arial Narrow" ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ARIALNBI.TTF";            DestDir: "{fonts}" ; FontInstall: "Arial Narrow" ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ARIALNI.TTF";             DestDir: "{fonts}" ; FontInstall: "Arial Narrow" ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\ariblk.ttf";              DestDir: "{fonts}" ; FontInstall: "Arial Black"  ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\FrutigerLTStd-Light.ttf"; DestDir: "{fonts}" ; FontInstall: "Frutiger LT Std 45 Light" ; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\FrutigerLTStd-Roman.ttf"; DestDir: "{fonts}" ; FontInstall: "Frutiger LT Std 55 Roman" ; Flags: onlyifdoesntexist uninsneveruninstall
;Source: "Readme.txt"; DestDir: "{app}";
;Source: "readme456b.txt"; DestDir: "{app}";
Source: "ServerBuild.txt"; DestDir: "{app}\config"
Source: ".\data\handyxml.py"; DestDir: "{reg:HKLM\Software\Python\PythonCore\2.3\InstallPath,|C:\Python23\}\Lib\site-packages"
Source: ".\data\serverConfig.ini"; DestDir: "{app}\config"; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\homeConfig.ini"; DestDir: "{app}\config"; Flags: onlyifdoesntexist uninsneveruninstall
Source: ".\data\Serial.dat"; DestDir: "{app}\config"; Flags:  onlyifdoesntexist uninsneveruninstall; 

; Pump Data
Source: "tesla\instrument\hardware_drd.ini"; DestDir: "{app}\config"; DestName: "hardware.ini"; Flags: onlyifdoesntexist uninsneveruninstall ; Tasks: pumpconfig\drdpump
Source: "tesla\instrument\hardware_ivek.ini"; DestDir: "{app}\config"; DestName: "hardware.ini"; Flags: onlyifdoesntexist uninsneveruninstall ; Tasks: pumpconfig\ivekpump
;Source: "RoboSep-X3-FirmwareRelease-DRD.iee"; DestDir: "{app}"; Flags: ignoreversion; Tasks: pumpconfig\drdpump
;Source: "RoboSep-X3-FirmwareRelease-IVEK.iee"; DestDir: "{app}"; Flags: ignoreversion; Tasks: pumpconfig\ivekpump
; NOTE: pump config done in final assembly NOT software installation

; Pump Configuration
[Tasks]
Name: pumpconfig; Description: "Pump Configuration"; GroupDescription: "Pump Type:"; Flags: exclusive
Name: pumpconfig\drdpump; Description: "DRD Pump"; GroupDescription: "Pump Type:"; Flags: exclusive
Name: pumpconfig\ivekpump; Description: "IVEK Pump"; GroupDescription: "Pump Type:"; Flags: exclusive
Name: pumpconfig\manpump; Description: "Manual Configuration"; GroupDescription: "Pump Type:"; Flags: exclusive

; Shortcuts
[Icons]
Name: "{userdesktop}\Start RoboSep"; Filename:"{app}\launcher\RoboSepLauncher.exe"; WorkingDir: "{app}"; Comment: "Start the Robosep server and operator console"; Flags: runminimized; IconFileName: "{app}\Launcher.ico";
Name: "{userdesktop}\Shutdown instrument"; Filename:"{app}\launcher\RoboSepShutdown.exe"; WorkingDir: "{app}"; Comment: "Shutdown the Robosep instrument"; Flags: runminimized ;
;Name: "{commonstartup}\Start RoboSep"; Filename: "{app}\launcher\RoboSepLauncher.exe"; WorkingDir: "{app}"; Flags: runminimized
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Start RoboSep"; Filename: "{app}\launcher\RoboSepLauncher.exe"; WorkingDir: "{app}"; Flags: runminimized ; IconFileName: "{app}\Launcher.ico";
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Protocols"; Filename: "{app}\protocols";
Name: "{commonstartup}\Start RoboSep"; Filename:"{app}\launcher\RoboSepLauncher.exe"; WorkingDir: "{app}"; Flags: runminimized; IconFileName: "{app}\Launcher.ico";

[Run]
; share folders
Filename: "net"; WorkingDir: "{app}"; Parameters: "share protocols=""{app}\protocols"" " ; Flags: shellexec runhidden waituntilterminated
Filename: "net"; WorkingDir: "{app}"; Parameters: "share logs=""{app}\logs"" " ; Flags: shellexec runhidden waituntilterminated
Filename: "net"; WorkingDir: "{app}"; Parameters: "share reports=""{app}\reports"" " ; Flags: shellexec runhidden waituntilterminated
Filename: "net"; WorkingDir: "C:\Share Data"; Parameters: "share data=""C:\Share Data"" " ; Flags: shellexec runhidden waituntilterminated
FileName: "{sys}\regsvr32.exe"; WorkingDir: "{app}\bin"; Parameters: "RotateTrans.ax"




