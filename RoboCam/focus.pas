unit focus;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls;

type
  TfrmManualFocus = class(TForm)
    Button1   : TButton;
    Label1    : TLabel;
    EditFocus : TEdit;
    procedure Button1Click(Sender: TObject);
    procedure FormCreate(Sender: TObject);
  private
    { Private declarations }
    FDefault : Integer;
    procedure Updatevalue(val : Integer);
  public
    { Public declarations }
    property FocusValue : Integer read FDefault write Updatevalue;
  end;

implementation

{$R *.dfm}

procedure TfrmManualFocus.Button1Click(Sender: TObject);
begin
  FDefault    := StrToInt( EditFocus.Text );
  ModalResult := mrOK;
end;

procedure TfrmManualFocus.FormCreate(Sender: TObject);
begin
  FDefault := 0;
  EditFocus.Text := IntToStr( FDefault );
end;

procedure TfrmManualFocus.Updatevalue(val: Integer);
begin
  FDefault := val;
  EditFocus.Text := IntToStr( FDefault );
end;

end.
