; -- Components.iss --
; Demonstrates a components-based installation.

; SEE THE DOCUMENTATION FOR DETAILS ON CREATING .ISS SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{C9291226-105E-45B9-9BFE-DB3911BBDCF0}
AppName=RoboSep-S
AppVerName=RoboSep-S
AppVersion=1.3.0.0
VersionInfoVersion=1.3.0.0
VersionInfoCompany=Stemcell Technologies
AppPublisher=Stemcell Technologies
AppPublisherURL=http://www.stemcell.com
AppSupportURL=http://www.stemcell.com
AppUpdatesURL=http://www.stemcell.com
OutputDir=.\output
OutputBaseFilename=Update_RoboSep-S_v1300
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

[InstallDelete]
Type: files; Name:"{app}\RoboSepSServer.exe";         AfterInstall: DeleteSleep(3000);
Type: files; Name:"{app}\RoboSepSClient.msi";         AfterInstall: DeleteSleep(3000);
Type: files; Name:"{app}\RoboSepSService.msi";        AfterInstall: DeleteSleep(3000);
Type: files; Name:"{app}\RoboSepSProtocolEditor.msi"; AfterInstall: DeleteSleep(3000);

[Files]
source: ".\Bin\RoboSepSServer.exe";         DestDir: "{tmp}";           flags: deleteafterinstall; BeforeInstall: TaskKill('RoboSepLauncher.exe');
source: ".\Bin\RoboSepSClient.msi";         DestDir: "{tmp}";           flags: deleteafterinstall; 
source: ".\Bin\RoboSepSService.msi";        DestDir: "{tmp}";           flags: deleteafterinstall; 
source: ".\Bin\RoboSepSProtocolEditor.msi"; DestDir: "{tmp}";           flags: deleteafterinstall;  
source: ".\Bin\Uninstall.bat";              DestDir: "{app}";           flags: deleteafterinstall;  
source: ".\Bin\Language.ini";               DestDir: "{app}\Config";    flags: uninsneveruninstall;
source: ".\Bin\RoboSepService.config";      DestDir: "{pf}\STI";        flags: uninsneveruninstall; afterInstall:SaveSerialDatFile;
Source: ".\Bin\GUI.ini";                    DestDir: "{app}\Config";    flags: uninsneveruninstall;
Source: ".\Bin\RoboSepProtocol.xsd";        DestDir: "{app}";           flags: deleteafterinstall;
Source: ".\Bin\WallpaperChange.exe";        DestDir: "{app}\Bin";       flags: uninsneveruninstall;
Source: ".\Bin\Wallpaper.jpg";              DestDir: "{app}\Bin";       flags: uninsneveruninstall;
Source: ".\Bin\WPC.bat";                    DestDir: "{app}\Bin";       flags: uninsneveruninstall;
Source: ".\Bin\Form_SplashScreen.exe";      DestDir: "{app}";           flags: deleteafterinstall;
Source: ".\Bin\RoboCam.exe";                DestDir: "{app}\Bin";       flags: uninsneveruninstall;

[Run]
Filename: "{app}\uninst\unins003.exe"; Parameters: "/SILENT /SUPPRESSMSGBOXES /FORCE";      flags: skipifdoesntexist waituntilterminated; afterInstall: DeleteSleep(10000);
Filename: "{app}\uninst\unins002.exe"; Parameters: "/SILENT /SUPPRESSMSGBOXES /FORCE";      flags: skipifdoesntexist waituntilterminated; afterInstall: DeleteSleep(10000);
Filename: "{app}\uninst\unins001.exe"; Parameters: "/SILENT /SUPPRESSMSGBOXES /FORCE";      flags: skipifdoesntexist waituntilterminated; afterInstall: DeleteSleep(10000);
Filename: "{app}\uninst\unins000.exe"; Parameters: "/SILENT /SUPPRESSMSGBOXES /FORCE";      flags: skipifdoesntexist waituntilterminated; afterInstall: ClearUninstaller();
;editor
;Filename: "msiexec.exe"; Parameters:"/x {{22A835CC-670F-47ED-B71D-990F0969E64A} /q"; flags: waituntilterminated; afterInstall: DeleteSleep(10000);
;Service
Filename: "msiexec.exe"; Parameters:"/x {{42C85EB0-B5F4-40B6-AE8B-9FA9CEDA4801} /q"; flags: waituntilterminated; afterInstall: DeleteSleep(10000);
;GUI
Filename: "msiexec.exe"; Parameters:"/x {{FE4A5E9C-FA36-469A-B96E-2EF03253CBB0} /q"; flags: waituntilterminated; afterInstall: DeleteSleep(10000);      
FileName: "{tmp}\RoboSepSServer.exe";                                                flags: waituntilterminated; afterInstall: DeleteSleep(3000);
FileName: "msiexec.exe"; Parameters: "/i ""{tmp}\RoboSepSClient.msi""";              flags: waituntilterminated; afterInstall: DeleteSleep(3000);
;FileName: "msiexec.exe"; Parameters: "/i ""{tmp}\RoboSepSProtocolEditor.msi""";      flags: waituntilterminated; afterInstall: CopyProtocolXSDAndSleep(3000);
FileName: "msiexec.exe"; Parameters: "/i ""{tmp}\RoboSepSService.msi""";             flags: waituntilterminated; afterInstall: UpdateConfigFile(); 
Filename: "{app}\Bin\WPC.bat";                                                       flags: waituntilterminated; AfterInstall: OverwriteSplashScreen();

[ICONS]
Name: "{commonstartup}\Change Wallpaper"; Filename:"{app}\Bin\WPC.bat"; WorkingDir: "{app}\Bin"; Flags: runminimized; IconFileName: "{app}\Launcher.ico";

[INI]
Filename: "{app}\Config\Hardware.ini"; Section: "Barcode_reader";  Key: "barcode_offset_degrees" ; String: "-65.00";
Filename: "{app}\Config\Hardware.ini"; Section: "Barcode_reader";  Key: "barcode_rescan_offset_degrees" ; String: "0.39";
Filename: "{app}\Config\Hardware.ini"; Section: "Barcode_reader";  Key: "max_count" ; String: "6";
Filename: "{app}\Config\Hardware.ini"; Section: "Barcode_reader";  Key: "poll_duration" ; String: "750";
Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "init_cmd3" ; String: "\x16M";
Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "init_cmd4" ; String: "SHWNRD1;TRGSTO500.";
Filename: "{app}\Config\Hardware.ini"; Section: "Platform";        Key: "z_traveloffset_1mltip_mm";     String: "-90.0";
Filename: "{app}\Config\Hardware.ini"; Section: "Platform";        Key: "tipstripper_sensor_delay";     String: "0";
Filename: "{app}\Config\Hardware.ini"; Section: "Pump";            Key: "standardpowerprofile";         String: "150,80,0";
Filename: "{app}\Config\Hardware.ini"; Section: "Pump";            Key: "homingvelocityprofile";        String: "421, 883, 10";
Filename: "{app}\Config\Hardware.ini"; Section: "Pump";            Key: "prehome_pos_ct";               String: "500";
Filename: "{app}\Config\Hardware.ini"; Section: "Pump";            Key: "home_delta_pos_ct";            String: "20";
Filename: "{app}\Config\Hardware.ini"; Section: "Pump";            Key: "home_inc_pos_ct";              String: "5";
; Filename: "{app}\Config\Hardware.ini"; Section: "Aspiration_1";    Key: "compensation_scalingfactor";   String: "1.0";
; Filename: "{app}\Config\Hardware.ini"; Section: "Aspiration_1";    Key: "compensation_offset";          String: "0";
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ThetaAxis"; Key: "standardpowerprofile";         String: "150,150,80";
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ThetaAxis"; Key: "standardvelocityprofile";      String: "300, 3000, 12";
; Filename: "{app}\Config\Hardware.ini"; Section: "Dispense_1";      Key: "compensation_scalingfactor";   String: "1.0";
; Filename: "{app}\Config\Hardware.ini"; Section: "Dispense_1";      Key: "compensation_offset";          String: "0";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "baseoffset_bulkcontainer_mm";  String: "230.0";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "wickingextractoffset_aspirate_bccm_mm";  String: "73";
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ZAxis";     Key: "homingvelocityprofile";        String: "2000, 10000, 5";
; Adding following lines Jun 07,2016
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ZAxis";     Key: "standardpowerprofile";         String: "70,170,0"
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ZAxis";     Key: "homingpowerprofile";           String: "120,170,0"

Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "init_cmd2" ; String: "DECWIN1;DECLFT0;DECRGT42;DECBOT64;DECTOP20;SNPGAN4;SNPLED1;SHWNRD1;TRGSTO500.";
Filename: "{app}\Config\Hardware.ini"; Section: "N5600";           Key: "pre_cmd" ; String: "IMGSNP1P0E!";

Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "hemisphericalprofilefullvolume_14mlvial";      String: "1000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_14mlvial";        String: "13000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_14mlvial";        String: "85";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullheight_50mlvial";            String: "18";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullvolume_50mlvial";            String: "5000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_50mlvial";        String: "95";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_50mlvial";        String: "53000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume_reagentvial";     String: "2250";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight_reagentvial";     String: "43";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "deadVolumeprofiledeadvolume_bulkbottle";       String: "15000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume1_bulkbottle";     String: "300000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight1_bulkbottle";     String: "52";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "truncatedconicalprofilefullheight_bulkbottle"; String: "11";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "truncatedconicalprofilefullvolume_bulkbottle"; String: "5000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullheight_bulkbottle";          String: "978";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "conicalprofilefullvolume_bulkbottle";          String: "3960";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullheight2_bulkbottle";     String: "41";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cylindricalprofilefullvolume2_bulkbottle";     String: "48000";
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "filledvialvolume_2ml_ul";                      String: "1100"
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "deadvolume_bulkcontainer_ul";                  String: "75000";      
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "bccm_containername";                           String: "Buffer Bottle"
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "waste_vialname";                               String: "Waste Tube"
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "lysis_vialname";                               String: "Lysis Buffer Tube"
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "cocktail_vialname";                            String: "Cocktail Vial (Square)" 
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "particle_vialname";                            String: "Magnetic Particle Vial (Triangle)" 
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "antibody_vialname";                            String: "Antibody Vial (Circle)" 
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "sample_vialname";                              String: "Sample Tube" 
Filename: "{app}\Config\Hardware.ini"; Section: "Tesla";           Key: "separation_vialname";                          String: "Separation Tube" 
 
Filename: "{app}\Config\Hardware.ini"; Section: "Carousel";        Key: "homebackoffsteps";                             String: "800";
Filename: "{app}\Config\Hardware.ini"; Section: "Carousel";        Key: "homingvelocityprofile";                        String: "1000, 1000, 1";

Filename: "{app}\Config\Hardware.ini"; Section: "Platform";        Key: "z_striptip_powerprofile";                      String: "200, 80, 0";
Filename: "{app}\Config\Hardware.ini"; Section: "Platform";        Key: "z_pickuptip_idlepowerprofile";                 String: "200, 80, 0";
Filename: "{app}\Config\Hardware.ini"; Section: "Platform";        Key: "z_pickuptip_powerprofile ";                    String: "200, 80, 0";

Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ZAxis";     Key: "standardpowerprofile ";                        String: "70, 80, 0";
Filename: "{app}\Config\Hardware.ini"; Section: "Robot_ZAxis";     Key: "homingpowerprofile ";                          String: "120, 80, 0";

Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "May2014 Server 1.0.4.0 Upgrade Installer";   String: "Changing z_traveloffset_1mltip_mm from -91 to -90."
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_1"; String: "Changing z_traveloffset_1mltip_mm from -91 to -90."
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_2"; String: "Adding tipstripper_sensor_delay = 0"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_3"; String: "Adding prehome_pos_ct = 500"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_4"; String: "Adding home_delta_pos_ct = 20"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_5"; String: "Adding home_inc_pos_ct = 5"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_6"; String: "Changing [Robot_ThetaAxis] standardpowerprofile = 150,150,80 (was 150,150,0)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_7"; String: "Changing [Robot_ThetaAxis] standardvelocityprofile = 300, 3000, 12 (was 300, 3750, 7)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_8"; String: "Changing [Tesla] baseoffset_bulkcontainer_mm = 230.0 (was 239.0)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_8"; String: "Changing [Tesla] wickingextractoffset_aspirate_bccm_mm = 73 ((was 30 for classic vacuum bottle, slow extraction eliminate wall droplets)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Mar2016 Server 1.1.0.3 Upgrade Installer_9"; String: "Changing [Robot_ZAxis] homingvelocityprofile  = 2000, 10000, 5 (was 1000, 5000, 5)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Jun2016 Server 1.1.0.4 Upgrade Installer_10"; String: "Changing [Robot_ZAxis] standardpowerprofile  = 70,170,0 (was 170,170,0)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Jun2016 Server 1.1.0.4 Upgrade Installer_11"; String: "Changing [Robot_ZAxis] homingpowerprofile  = 120,170,0 (was 70,170,0)"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_12"; String: "Changing [N5600] init_cmd2 =DECWIN1;DECLFT0;DECRGT42;DECBOT64;DECTOP20;SNPGAN4;SNPLED1;SHWNRD1;TRGSTO500."
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_13"; String: "Adding [N5600] pre_cmd =IMGSNP1P0E!"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_14"; String: "Adding [N5600] pre_cmd =IMGSNP1P0E!"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_15"; String: "Adding [Tesla] hemisphericalprofilefullvolume_14mlvial =1000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_16"; String: "Adding [Tesla] cylindricalprofilefullvolume_14mlvial =13000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_17"; String: "Adding [Tesla] cylindricalprofilefullheight_14mlvial =85"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_18"; String: "Adding [Tesla] conicalprofilefullheight_50mlvial =18"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_19"; String: "Adding [Tesla] conicalprofilefullvolume_50mlvial =5000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_20"; String: "Adding [Tesla] cylindricalprofilefullheight_50mlvial =95"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_21"; String: "Adding [Tesla] cylindricalprofilefullvolume_50mlvial =53000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_22"; String: "Adding [Tesla] cylindricalprofilefullvolume_reagentvial =2250"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_23"; String: "Adding [Tesla] cylindricalprofilefullheight_reagentvial =43"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_15"; String: "Adding [Tesla] deadVolumeprofiledeadvolume_bulkbottle =15000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_16"; String: "Adding [Tesla] cylindricalprofilefullvolume1_bulkbottle =300000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_17"; String: "Adding [Tesla] cylindricalprofilefullheight1_bulkbottle =52"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_18"; String: "Adding [Tesla] truncatedconicalprofilefullheight_bulkbottle =11"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_19"; String: "Adding [Tesla] truncatedconicalprofilefullvolume_bulkbottle =5000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_20"; String: "Adding [Tesla] conicalprofilefullheight_bulkbottle =978"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_21"; String: "Adding [Tesla] conicalprofilefullvolume_bulkbottle =3960"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_22"; String: "Adding [Tesla] cylindricalprofilefullheight2_bulkbottle =41"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Aug2017 Server 1.2.0.2 Upgrade Installer_23"; String: "Adding [Tesla] cylindricalprofilefullvolume2_bulkbottle =48000"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Jan2018 Server 1.2.0.2 Upgrade Installer_24"; String: "Changing [Carousel] homebackoffsteps = from 600 to 800."
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Jan2018 Server 1.2.0.2 Upgrade Installer_25"; String: "Changing [Carousel] homingvelocityprofile = from 1500, 1500, 1 to 1000, 1000, 1"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Nov2018 Server 1.2.1.1 Upgrade Installer_26"; String: "Changing [Platform]/[Robot_ZAxis] Power profiles"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Nov2018 Server 1.2.1.1 Upgrade Installer_27"; String: "Adding [Tesla] Custom vial name definitions"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Nov2018 Server 1.2.1.1 Upgrade Installer_28"; String: "Adding [Tesla] filledvialvolume_2ml_ul = 1100"
Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Nov2018 Server 1.2.1.1 Upgrade Installer_29"; String: "Adding [Tesla] deadvolume_bulkcontainer_ul = 75000"
Filename: "{app}\Config\serverConfig.ini"; Section: "Comments";    Key: "May2019 Server 1.3.0.0 Upgrade Installer_30"; String: "Disable SS_OLDPCB"
Filename: "{app}\Config\serverConfig.ini"; Section: "Comments";    Key: "May2019 Server 1.3.0.0 Upgrade Installer_31"; String: "Adding [debugger] section"
;Filename: "{app}\Config\serverConfig.ini"; Section: "Comments";    Key: "May2019 Server 1.3.0.0 Upgrade Installer_32"; String: "Adding SS_XY_MICRO_LC = 1"
Filename: "{app}\Config\serverConfig.ini"; Section: "Comments";    Key: "May2019 Server 1.3.0.0 Upgrade Installer_33"; String: "Adding SS_ENABLE_STRIP_ARM_POSITION_CHECK = 1"
; Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Dec2015 Server 1.1.0.1 Upgrade Installer_10"; String: "Changing [Aspiration_1] and [Dispense_1] compensation_scalingfactor   = 1.0 (was 1.0190)"
; Filename: "{app}\Config\Hardware.ini"; Section: "Comments";        Key: "Dec2015 Server 1.1.0.1 Upgrade Installer_11"; String: "Changing [Aspiration_1] and [Dispense_1] compensation_offset    = -0.0 (was -0.7843)"
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_DEBUGGER_LOG";        String: "0";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_EXT_LOGGER";          String: "1";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_CUSTOM_HOME";         String: "0";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_USE460_PUMPHOMING";   String: "0";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_BAD_H_TEST";          String: "0";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_OLDPCB";              String: "1  ; Obsoleted - No longer supported";
;Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_XY_MICRO_LC";         String: "1";
Filename: "{app}\Config\serverConfig.ini"; Section: "environment"; Key: "SS_ENABLE_STRIP_ARM_POSITION_CHECK";              String: "1";

Filename: "{app}\Config\serverConfig.ini"; Section: "new_cmd_logic"; Key: "apply_cmd_sub_logic";  String: "1";
Filename: "{app}\Config\serverConfig.ini"; Section: "new_cmd_logic"; Key: "subfor_transseptrans_onlyif_sep_is_lessthan_seconds"; String: "60";

;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxis"; String: "X0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisPower"; String: "P120,80,0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedBegin"; String: "B3000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedSlope"; String: "S5";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedEnd"; String: "E15000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedHomeBegin"; String: "B2000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedHomeSlope"; String: "S5";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisSpeedHomeEnd"; String: "E10000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisHalfStep"; String: "H3";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisBackOffHome"; String: "600";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisHome"; String: "N-110s";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpAxis"; String: "X1";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpPower"; String: "P150,50,0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedBegin"; String: "B421";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedSlope"; String: "S10";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedEnd"; String: "E1883";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedHomeBegin"; String: "B421";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedHomeSlope"; String: "S10";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpSpeedHomeEnd"; String: "E1883";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpHalfStep"; String: "H0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpBackOffHome"; String: "-1200";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpHome"; String: "N+110s";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArm"; String: "Y0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmPower"; String: "P140,100,0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedBegin"; String: "B300";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedSlope"; String: "S12";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedEnd"; String: "E3000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedHomeBegin"; String: "B900";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedHomeSlope"; String: "S1";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmSpeedHomeEnd"; String: "E900";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmHalfStep"; String: "H4";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmBackOffHome"; String: "1200";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmHome"; String: "N-110s";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "Carousel"; String: "Y1";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselPower"; String: "P200,200,0";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedBegin"; String: "B400";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedSlope"; String: "S5";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedEnd"; String: "E8000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedHomeBegin"; String: "B1000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedHomeSlope"; String: "S1";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselSpeedHomeEnd"; String: "E1000";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselHalfStep"; String: "H4";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselBackOffHome"; String: "800";
;Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselHome"; String: "N-100s";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselHomeSwitch"; String: "0";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PowerOff"; String: "P1,0,0";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZPowerOff"; String: "P20,20,0";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ZAxisPowerProfile";    String: "homingpowerprofile";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "PumpPowerProfile";     String: "homingpowerprofile";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "ThetaArmPowerProfile"; String: "homingpowerprofile";
Filename: "{app}\Config\serverConfig.ini"; Section: "debugger"; Key: "CarouselPowerProfile"; String: "homingpowerprofile";

[Code]
var
  UserPage  : TInputQueryWizardPage;
  SerialNum : AnsiString;
  FilePath  : AnsiString;

procedure UpdateConfigFile();
  var CONFIGPATH : String;
      DESTPATH   : String;
      SERIALSRC  : String;
      SERIALDST  : String;
begin
  CONFIGPATH := ExpandConstant('{pf}') + '\STI\RoboSepService.config';
  DESTPATH   := ExpandConstant('{pf}') + '\STI\RoboSepService\config\RoboSepService.config';
  SERIALSRC  := ExpandConstant('{app}') + '\Serial.dat';
  SERIALDST  := ExpandConstant('{app}') + '\Config\Serial.dat';
  FileCopy(CONFIGPATH, DESTPATH, False);
  DeleteFile(CONFIGPATH);
  FileCopy(SERIALSRC, SERIALDST, False);
  DeleteFile(SERIALSRC);
end;

procedure DeleteSleep( ms : Integer );
begin
    Sleep(ms);
end;

procedure ClearUninstaller();
  var path : String;
      rtn : Boolean;
begin
  DeleteSleep(10000);
  path := '{app}\uninst\*';
  DelTree(ExpandConstant(path), False, True, True);
end;

procedure CopyProtocolXSDAndSleep( ms : Integer );
  var Src, Dst : String;
begin
  Src := ExpandConstant('{app}\config\RoboSepProtocol.xsd');
  Dst := ExpandConstant('{app}\..\RoboSepEditor\config\RoboSepProtocol.xsd');
  FileCopy( Src, Dst, False )
  Sleep(ms);
end;

procedure OverwriteSplashScreen();
  var Src, Dst : String;
begin
  Src := ExpandConstant('{app}\Form_SplashScreen.exe');
  Dst := ExpandConstant('{app}\bin\Form_SplashScreen.exe');
  DeleteFile(Dst);
  FileCopy( Src, Dst, True )
end;

procedure SaveSerialDatFile();
  var SRCPATH : String;
      DSTPATH : String;
begin
    SRCPATH := ExpandConstant('{app}') + '\Config\Serial.dat';
    DSTPATH := ExpandConstant('{app}') + '\Serial.dat';
    FileCopy(SRCPATH, DSTPATH, False);    
end;

procedure ChangeWallPaper();
  var code : Integer;
      src, param : String;  
begin
  src := ExpandConstant('{app}\WallpaperChange.exe');
  param := ExpandConstant('{app}\Wallpaper.jpg');
  MsgBox('Replace wallpaper', mbInformation, MB_OK);
  if Exec( src,
           param, '', 
           SW_SHOW, 
           ewWaitUntilTerminated, 
           Code) then
   begin
      MsgBox('Replace Success', mbInformation, MB_OK);   
   end
   else
   begin
      MsgBox('Replace fail', mbInformation, MB_OK);   
   end;             
end;

procedure TaskKill(FileName: String);
var
  ResultCode: Integer;
begin
    Exec(ExpandConstant('taskkill.exe'), '/f /im ' + '"' + FileName + '"', '', SW_HIDE,
     ewWaitUntilTerminated, ResultCode);
end;

function InitializeSetup(): Boolean;
begin
  Result := MsgBox('RoboSep S Software Upgrade Information:'#13#13'This updater will upgrade to v1.3.0.0'#13#13' Do you wish to continue?',mbConfirmation,MB_YESNO)=idYes;
end;



