unit main;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, DSUtil, StdCtrls, DSPack, DirectShow9, Menus, ExtCtrls;

type


  TVideoForm = class(TForm)
    FilterGraph : TFilterGraph;
    MainMenu1   : TMainMenu;
    vDevices    : TMenuItem;
    aDevices    : TMenuItem;
    OpenDialog  : TOpenDialog;
    vFilter     : TFilter;
    aFilter     : TFilter;
    ASFWriter   : TASFWriter;
    VideoWindow : TVideoWindow;

    procedure FormCreate(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
  private
    { Déclarations privées }
    FaDevNum : Integer;
    FvDevNum : Integer;
  public
    { Déclarations publiques }
    procedure OnSelectVideoDevice(sender: TObject);
    procedure OnSelectAudioDevice(sender: TObject);
  end;

var
  VideoForm : TVideoForm;
  vSysDev   : TSysDevEnum;
  aSysDev   : TSysDevEnum;

implementation

uses Math;

{$R *.dfm}

procedure TVideoForm.FormCreate(Sender: TObject);
var
  i : integer;
  Device : TMenuItem;
begin
  FaDevNum := 0;
  FvDevNum := 0;

  vSysDev:= TSysDevEnum.Create(CLSID_VideoInputDeviceCategory);
  if vSysDev.CountFilters > 0 then
    for i := 0 to vSysDev.CountFilters - 1 do
    begin
      Device := TMenuItem.Create(vDevices);
      Device.Caption := vSysDev.Filters[i].FriendlyName;
      Device.Tag := i;
      Device.OnClick := OnSelectVideoDevice;
      vDevices.Add(Device);
    end;
  aSysDev:= TSysDevEnum.Create(CLSID_AudioInputDeviceCategory);
  if aSysDev.CountFilters > 0 then
    for i := 0 to aSysDev.CountFilters - 1 do
    begin
      Device := TMenuItem.Create(vDevices);
      Device.Caption := aSysDev.Filters[i].FriendlyName;
      Device.Tag := i;
      Device.OnClick := OnSelectAudioDevice;
      aDevices.Add(Device);
    end;
end;

procedure TVideoForm.OnSelectVideoDevice(sender: TObject);
begin
  FvDevNum := TMenuItem(Sender).Tag;
  FilterGraph.ClearGraph;
  FilterGraph.Active := false;
  vFilter.BaseFilter.Moniker := vSysDev.GetMoniker(FvDevNum);
  aFilter.BaseFilter.Moniker := aSysDev.GetMoniker(FaDevNum);
  FilterGraph.Active := true;
  with FilterGraph as ICaptureGraphBuilder2 do
  begin
    CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil, vFilter as IBaseFilter, nil, ASFWriter as IbaseFilter));
    CheckDSError(RenderStream(@PIN_CATEGORY_CAPTURE , nil, aFilter as IBaseFilter, nil, ASFWriter as IbaseFilter));
    CheckDSError(RenderStream(@PIN_CATEGORY_PREVIEW , nil, vFilter as IBaseFilter, nil, VideoWindow as IbaseFilter));
  end;
  FilterGraph.Play;
end;

procedure TVideoForm.OnSelectAudioDevice(sender: TObject);
begin
  FaDevNum := TMenuItem(Sender).Tag;
end;

procedure TVideoForm.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  vSysDev.Free;
  aSysDev.Free;
  FilterGraph.ClearGraph;
  FilterGraph.Active := false;
end;

end.
