unit cBaseDShow;

interface
uses
  Windows,  SysUtils, Classes,  DirectShow9, ActiveX, DsUtils;

type
  TBaseDShow = Class
  private
  public
    //필터그래프와 인터페이스...
    FilterGraph: IGraphBuilder;
    MediaControl: IMediaControl;
    VideoWindow: IVideoWindow;

    constructor Create;
    destructor Destroy; override;
    function GetCamFilter:IBaseFilter;
    function CreateFilterGraph(var Graph: IGraphBuilder): Boolean;              //필터그래프를 생성한다.
    function CreateFilter(const clsid:TGUID; var Filter:IBaseFilter): Boolean;  //각종 필터를 생성한다.
    function FindPinOnFilter(const Filter: IBaseFilter;                         //필터에서 내가 원하는 핀을 찾아주는 함수.
                             const PinDir: TPinDirection; var Pin: IPin): HRESULT;
  end;

implementation

{ TBaseDShow }

constructor TBaseDShow.Create;
begin
  inherited Create;
  CoInitialize(nil);                                                            //COM을 초기화한다.
  CreateFilterGraph(FilterGraph);                                               //필터그래프를 생성한다.
  FilterGraph.QueryInterface(IID_IMediaControl, MediaControl);                  //필터그래프의 인터페이스.
  FilterGraph.QueryInterface(IID_IVideoWindow, VideoWindow);
end;

destructor TBaseDShow.Destroy;
begin
  if Assigned(MediaControl) then MediaControl.Stop;
  While Assigned(VideoWindow) do VideoWindow := nil;
  While Assigned(MediaControl) do MediaControl := nil;
  While Assigned(FilterGraph) do FilterGraph := nil;

  CoUninitialize;                                                               //COM을 셧다운시킨다.
  inherited Destroy;
end;

function TBaseDShow.CreateFilterGraph(var Graph: IGraphBuilder): Boolean;
begin
  Result := False;
  if Failed(CoCreateInstance(CLSID_FilterGraph,
                              nil,
                              CLSCTX_INPROC_SERVER,
                              IID_IFilterGraph,
                              Graph))
  then Exit;
  Result := True;
end;

function TBaseDShow.CreateFilter(const clsid: TGUID; var Filter: IBaseFilter): Boolean;
begin
  Result := False;
  if Failed(CoCreateInstance(clsid,
                             NIL,
                             CLSCTX_INPROC_SERVER,
                             IID_IBaseFilter,
                             Filter))
  then Exit;
  Result := True;
end;

function TBaseDShow.FindPinOnFilter(const Filter: IBaseFilter;
  const PinDir: TPinDirection; var Pin: IPin): HRESULT;
var
  IsConnected : Boolean;
  hr: DWORD;
  EnumPin: IEnumPins;
  ConnectedPin: IPin;
  PinDirection: TPinDirection;
begin
  Result := S_False;

  if not Assigned(Filter) then exit;
 	hr := Filter.EnumPins(EnumPin);

  if(SUCCEEDED(hr)) then
  begin
  	while (S_OK = EnumPin.Next(1, Pin, nil)) do
    begin
      //핀이 연결되었는지 조사.
      hr := Pin.ConnectedTo(ConnectedPin);
      if hr = S_OK then
      begin
        IsConnected  := True;
        ConnectedPin := nil;
      end else IsConnected := False;

      //핀의 방향을 검사
      hr := Pin.QueryDirection(PinDirection);
      //매개변수의 핀방향과 동일하고 현재 연결된 상태가 아니라면 루프에서 탈출. 
      if (hr = S_OK) and (PinDirection = PinDir) and (not IsConnected) then break;
      pin := nil;
    end;
    Result := S_OK;
  end;
  EnumPin := nil;
end;

function TBaseDShow.GetCamFilter: IBaseFilter;
var
  SysEnum: TSysDevEnum;
begin
  SysEnum := TSysDevEnum.Create;
  try
    SysEnum.SelectGUIDCategory(CLSID_VideoInputDeviceCategory);
    Result := SysEnum.GetBaseFilter(0)// 가장 첫번째 장치를 가져온다.
  finally
    SysEnum.Free;
  end;
end;

end.
