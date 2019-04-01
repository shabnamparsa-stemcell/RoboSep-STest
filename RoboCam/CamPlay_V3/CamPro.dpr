program CamPro;

uses
  Forms,
  uMain in 'uMain.pas' {frmMain},
  cBaseDShow in 'cBaseDShow.pas',
  cCamDShow in 'cCamDShow.pas',
  uCommon in 'uCommon.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TfrmMain, frmMain);
  Application.Run;
end.
