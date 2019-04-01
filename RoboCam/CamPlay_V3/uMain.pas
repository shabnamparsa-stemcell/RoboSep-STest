unit uMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, cCamDShow, StdCtrls, ExtCtrls, DSPack, ComCtrls;

type
  TfrmMain = class(TForm)
    Panel1: TPanel;
    Run_Button: TButton;
    Stop_Button: TButton;
    TrackBar1: TTrackBar;
    Memo1: TMemo;
    Button1: TButton;
    Button2: TButton;
    Button3: TButton;
    Button4: TButton;
    Button5: TButton;
    procedure Run_ButtonClick(Sender: TObject);
    procedure Stop_ButtonClick(Sender: TObject);
    procedure TrackBar1Change(Sender: TObject);
    procedure Button1Click(Sender: TObject);
    procedure Button2Click(Sender: TObject);
    procedure Button3Click(Sender: TObject);
    procedure Button4Click(Sender: TObject);
    procedure Button5Click(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
    CamDShow :TCamDShow;
  end;

var
  frmMain: TfrmMain;

implementation

{$R *.dfm}

procedure TfrmMain.Button2Click(Sender: TObject);
begin
  CamDShow.SetRotate(1);
end;

procedure TfrmMain.Button3Click(Sender: TObject);
begin
  CamDShow.SetRotate(0);
end;

procedure TfrmMain.Button4Click(Sender: TObject);
begin
  CamDShow.SetRotate(3);
end;

procedure TfrmMain.Button5Click(Sender: TObject);
begin
  CamDShow.SetRotate(2);
end;

procedure TfrmMain.Run_ButtonClick(Sender: TObject);
begin
  if not Assigned(CamDShow) then
  begin
    CamDShow := TCamDShow.Create(Panel1);
  end;

  CamDShow.Run;
end;

procedure TfrmMain.Stop_ButtonClick(Sender: TObject);
begin
  CamDShow.Stop;
end;

procedure TfrmMain.TrackBar1Change(Sender: TObject);
begin
  if not Assigned(CamDShow) then Exit;
  CamDShow.SetGreenValue(TrackBar1.Position);
end;

procedure TfrmMain.Button1Click(Sender: TObject);
begin
  if not Assigned(CamDShow) then Exit;
  CamDShow.SetText(Memo1.Text);
end;

end.
