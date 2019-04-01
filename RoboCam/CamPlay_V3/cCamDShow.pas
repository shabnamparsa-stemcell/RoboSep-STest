unit cCamDShow;

interface
uses
  Windows, Dialogs, SysUtils, Classes, Registry, DirectShow9, ActiveX, ExtCtrls,
  DsUtils, cBaseDShow, Graphics, uCommon;

type
  TCamDShow = class(TBaseDShow)
  private
    Cam         : IBaseFilter;
    VideoRender : IBaseFilter;
    RotateTrans : IBaseFilter;
    RotateInf   : IRotateInf;
  protected
  public
    constructor Create(Screen:TPanel);
    destructor Destroy;override;
    function MakeBaseFilter:HRESULT;
    function ReleaseBaseFilter:HRESULT;
    function ConnectBaseFilter:HRESULT;
    procedure Run;
    procedure Stop;
    procedure SetGreenValue(Value:Integer);
    procedure SetText(Value:String);
    procedure SetRotate(sw : Integer);
  end;

implementation

{ TCamDShow }

constructor TCamDShow.Create(Screen: TPanel);
begin
  inherited Create;
  MakeBaseFilter();
  ConnectBaseFilter();
  VideoWindow.put_Owner(OAHWND(Screen.Handle));
  VideoWindow.put_WindowStyle(WS_CHILD or WS_CLIPSIBLINGS);
  VideoWindow.put_Width(320);
  VideoWIndow.put_Height(240);
  VideoWindow.put_Top(0);
  VideoWindow.put_Left(0);
  VideoWindow.put_AutoShow(TRUE);
end;

destructor TCamDShow.Destroy;
begin
  ReleaseBaseFilter;
  inherited Destroy;
end;

function TCamDShow.MakeBaseFilter: HRESULT;
begin
  Result := S_OK;

  //카메라 필터를 생성하고 추가한다.
  Cam := GetCamFilter;   //카메라를 얻고...
  FilterGraph.AddFilter(Cam,'Cam Filter');  //카메라를 등록한다.
  if Cam = nil then Result := S_FALSE;

  //랜더러 필터를 생성하고 추가한다.
  CreateFilter(CLSID_VideoRenderer, VideoRender);  //비디오 랜더러를 얻고...
  FilterGraph.AddFilter(VideoRender,'VdRender Filter'); //비디오 랜더러를 등록한다.
  if VideoRender = nil then Result := S_FALSE;

  //TestTrans 필터를 생성하고 추가한다.
  CreateFilter(CLSID_TRotateTrans, RotateTrans);  //TestTrans 랜더러를 얻고...
  FilterGraph.AddFilter(RotateTrans,'TestTrans Filter'); //TestTrans를 등록한다.
  if RotateTrans = nil then Result := S_FALSE;

  RotateTrans.QueryInterface(IRotateInf,RotateInf); //TestInf 인터페이스를 얻어온다.
  if RotateInf = nil then
      ShowMessage('Cannot Create Control Interface!');

  if Result = S_FALSE then ShowMessage('MakeBaseFilter is Failed');
end;

function TCamDShow.ConnectBaseFilter: HRESULT;
  function ConnectFilter(A, B: IBaseFilter): HRESULT;
  var
    InPin : IPin;
    OutPin : IPin;
  begin
    FindPinOnFilter(A,PINDIR_OUTPUT,OutPin);
    FindPinOnFilter(B,PINDIR_InPUT,InPin);
    Result := FilterGraph.Connect(OutPin,InPin);
    if Result <> S_OK then ShowMessage('필터 연결에 실패하였습니다.');
  end;
begin
  Result := S_OK;

  if S_OK <> ConnectFilter(Cam, RotateTrans) then Exit;
  if S_OK <> ConnectFilter(RotateTrans, VideoRender) then Exit;

end;

function TCamDShow.ReleaseBaseFilter: HRESULT;
begin
  if Assigned(MediaControl) then MediaControl.Stop;

  FilterGraph.RemoveFilter(Cam);
  FilterGraph.RemoveFilter(VideoRender);
  FilterGraph.RemoveFilter(RotateTrans);

  While Assigned(Cam)             do Cam := nil;
  While Assigned(VideoRender)     do VideoRender := nil;
  While Assigned(RotateTrans)     do RotateTrans := nil;
  While Assigned(RotateInf)       do RotateInf := nil;


  Result := S_OK;
end;

procedure TCamDShow.Run;
begin
  if Assigned(MediaControl) then MediaControl.Run;
end;

procedure TCamDShow.Stop;
begin
  if Assigned(MediaControl) then MediaControl.Stop;
end;

procedure TCamDShow.SetGreenValue(Value: Integer);
begin
//  if Assigned(TestInf) then TestInf.SetGreenValue(Value);
end;

procedure TCamDShow.SetText(Value: String);
begin
//  if Assigned(TestInf) then TestInf.SetText(StringToOleStr(Value));
end;

procedure TCamDShow.SetRotate(sw: Integer);
begin
     RotateInf.SetRotate( sw );
end;

end.
