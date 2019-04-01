unit Subtitle;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ExtCtrls;

type TSubtitle = class(TObject)
  private
    FSubtitleList : TStringList;
    FFileName     : String;
    FStartTick    : Cardinal;
    FTick         : Cardinal;
    FisRecording  : Boolean;
    FidleTime     : _FILETIME;
    FkernelTime   : _FILETIME;
    FuserTime     : _FILETIME;
    FCPUProfile   : Boolean;
    function GetTick() : Cardinal;
    function GetCPUTime() : Integer;
  public
    constructor  Create();   virtual;
    destructor   Destory();  virtual;
    procedure    Free();     virtual;
    procedure    Clear();
    procedure    Start();
    procedure    Finish();
    procedure    SaveToFile();
    procedure    SetSubtitle( subtitle : String );
    procedure    SetMovieName( MovieName : String );
    property     SubtitleName : String read FFilename;
    property     isRecording : Boolean read FisRecording;
    property     CPUProfile  : Boolean read FCPUProfile write FCPUProfile;
end;

implementation

uses StrUtils;

function MakeSubtitleName(  SourceName : String ) : String;
  var OutputName : String;
      TotalLen   : Integer;
begin

  TotalLen   := Length( SourceName );
  OutputName := LeftStr( SourceName, TotalLen - 3 );
  OutputName := OutputName + 'smi';

  Result := OutputName;
end;

constructor TSubtitle.Create();
begin
  FSubtitleList := TStringList.Create();
  FFileName     := '';
  FStartTick    := 0;
  FTick         := 0;
  FisRecording  := False;
  FCPUProfile   := False;
end;

destructor TSubtitle.Destory();
begin
  //
end;

procedure TSubtitle.Free();
begin
  FSubtitleList.Free();
  inherited;
end;

procedure TSubtitle.Clear();
begin
  FFileName   := '';
  FStartTick  := 0;
  FTick       := 0;
  FSubtitleList.Clear();
end;

procedure TSubtitle.SetMovieName(MovieName: string);
begin
  FFileName := MakeSubtitleName(MovieName);
end;

procedure TSubtitle.SaveToFile();
begin
  try
    FSubtitleList.SaveToFile(FFileName);
  except
    ShowMessage('Cannot save the subtitle file!');
  end;
end;

procedure TSubtitle.SetSubtitle(subtitle: string);
  var str : String;
begin
  FTick := GetTick();
  str   := Format('<SYNC Start=%d><P Class=ENCC>',[FTick]);
  FSubtitleList.Add( str );

  if FCPUProfile = True then
     str   := Format('%s %d%%',[subtitle,GetCPUTime()])
  else
     str   := Format('%s',[subtitle]);

  FSubtitleList.Add( str );
end;

procedure TSubtitle.Start();
begin
  FSubtitleList.Clear();
  FStartTick   := GetTickCount();

  FSubtitleList.Add('<SAMI>');
  FSubtitleList.Add('<HEAD>');
  FSubtitleList.Add('<TITLE>RoboCam Capture</TITLE>');
  FSubtitleList.Add('<STYLE TYPE="text/css">');
  FSubtitleList.Add('<!-- P { margin-left:2pt; margin-right:2pt;');
  FSubtitleList.Add('         margin-bottom:1pt; margin-top:1pt; font-size:10pt; ');
  FSubtitleList.Add('         text-align:center; font-family:Arial, Sans-serif; ');
  FSubtitleList.Add('         font-weight:bold; color:white; }');
  FSubtitleList.Add(' .ENCC { Name: English; lang:en-US; SAMI_TYPE: CC;} -->');
  FSubtitleList.Add('</STYLE>');
  FSubtitleList.Add('</HEAD>');
  FSubtitleList.Add('<BODY>');

  FisRecording := True;
  OutputDebugString('[**** Start RoboCam Subtitle!]');
end;

procedure TSubtitle.Finish();
begin
  FSubtitleList.Add('</BODY>');
  FSubtitleList.Add('</SAMI>');
  FisRecording := False;
  OutputDebugString('[**** Stop RoboCam Subtitle Stop!]');
end;

function TSubtitle.GetTick() : Cardinal ;
  var rtn : Cardinal;
begin
  result := abs( GetTickCount() - FStartTick );
end;

function TSubtitle.GetCPUTime() : Integer;
  var sys, usr, idl, ker, cpu       : Cardinal;
      thisIdl, thisKernel, thisUsr  : _FILETIME;
begin
  GetSystemTimes( thisIdl, thisKernel, thisUsr );

  usr := thisUsr.dwLowDateTime - FuserTime.dwLowDateTime;
  ker := thisKernel.dwLowDateTime - FkernelTime.dwLowDateTime;
  idl := thisIdl.dwLowDateTime - FidleTime.dwLowDateTime;

  sys := ker + usr;
  cpu := ((sys - idl) * 100) div sys;

  FidleTime   := thisIdl;
  FkernelTime := thisKernel;
  FuserTime   := thisUsr;

  Result := cpu;
end;

end.
