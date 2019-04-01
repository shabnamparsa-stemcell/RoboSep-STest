unit camformat;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TfrmFormat = class(TForm)
    ListFmt   : TListBox;
    BtnSelect : TButton;
    BtnCancel : TButton;
    procedure BtnSelectClick(Sender: TObject);
    procedure BtnCancelClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }

  end;

implementation

{$R *.dfm}

procedure TfrmFormat.BtnCancelClick(Sender: TObject);
begin
  ModalResult := mrCancel;
end;

procedure TfrmFormat.BtnSelectClick(Sender: TObject);
begin

  ModalResult := mrOK;
end;

end.
