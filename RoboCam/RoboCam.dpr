program RoboCam;

uses
  Forms,
  windows,
  main in 'main.pas' {VideoForm},
  Subtitle in 'Subtitle.pas',
  focus in 'focus.pas' {frmManualFocus},
  camformat in 'camformat.pas' {frmFormat};

{$R *.res}

const MUTEXKEY : String  = 'RoboCamKey';

function CheckSingleInstance() : Boolean;
  var hMutex : Integer;
      bFound : Integer;
begin
  hMuTex := CreateMutex( 0, True, PChar(MUTEXKEY) )  ;

  bFound := 0;
  if(GetLastError() = ERROR_ALREADY_EXISTS) then
        bFound := 1;

  if (hMuTex <> 0) then
      ReleaseMutex(hMuTex);

  if bFound = 0 then
     Result := True;

  if bFound = 1 then
     Result := False;
end;

begin

  ReportMemoryLeaksOnShutDown := True;

  if CheckSingleInstance() = True then
  begin
    Application.Initialize;
    Application.Title := 'RoboCam';
    Application.CreateForm(TVideoForm, VideoForm);
  Application.Run;
  end else begin
    //Application.MessageBox(PChar('There is another instance. Please, quit the instnace! Terminating...'),PChar('RoboCam'));
  end;

end.
