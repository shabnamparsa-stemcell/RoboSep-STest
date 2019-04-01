unit uMain;

interface

uses Windows, Graphics, BaseClass, ActiveX, DirectShow9, DSUtil, Dialogs;

const
  CLSID_TRotateTrans: TGUID = '{EEA1BC2F-B02C-4A32-AF95-B1D53E5F45BB}';

type
  IRotateInf = interface(IUnknown)
    ['{D7C086E6-F0D1-42F3-8072-B0965596951D}']

    function SetRotate(sw : Integer):HResult;stdcall;
    function GetRGB24( pBuffer : PByte ; pSize : PInteger; pWidth : PInteger ; pHeight : PInteger ):HResult;stdcall;
  end;

type
  TRotateTrans = class(TBCTransInPlaceFilter,IRotateInf)
  private
    FGreenValue : Integer;
    FSrcWidth   : Integer;
    FSrcHeight  : Integer;
    FSrcSize    : Cardinal;
    FRotate     : Integer;
    FShift      : Integer;
    FisBusy     : Integer;
  protected
    // ITestInf Interface...
    function    SetRotate(sw : Integer):HResult;stdcall;
    function    GetRGB24( pBuffer : PByte ; pSize : PInteger; pWidth : PInteger ; pHeight : PInteger ):HResult;stdcall;
  public
    constructor Create(ObjName: string; unk: IUnKnown; out hr: HRESULT);
    constructor CreateFromFactory(Factory: TBCClassFactory; const Controller: IUnknown); override;
    destructor  Destroy;override;
    function    Transform(Sample: IMediaSample): HRESULT; override;
    function    CheckInputType(mtin: PAMMediaType): HRESULT; override;
  end;

implementation

uses SysUtils, Classes;

const MAX_VID_WIDTH   = 640;
const MAX_VID_HEIGHT  = 480;
const MAX_WORK_SPACE  = (MAX_VID_WIDTH*MAX_VID_HEIGHT);

type TPixcel = record
  Blue  : BYTE;
  Green : BYTE;
  Red   : BYTE;
end;
type PPixcel = ^TPixcel;

var DestImgSpace  : array [0.. (MAX_WORK_SPACE-1)] of TPixcel;
    pDestImgSpace : PPixcel;

constructor TRotateTrans.Create(ObjName: string; unk: IInterface; out hr: HRESULT);
begin
  inherited Create(ObjName, unk, CLSID_TRotateTrans, hr);
  FGreenValue := 0;
  FSrcWidth   := 0;
  FSrcHeight  := 0;
  FSrcSize    := 0;
  FShift      := 0;
  FisBusy     := 0;

  ZeroMemory( @DestImgSpace, sizeof(DestImgSpace) );
  pDestImgSpace := @DestImgSpace;
  FRotate       := 0;
end;

constructor TRotateTrans.CreateFromFactory(Factory: TBCClassFactory;
  const Controller: IInterface);
var hr : HResult;
begin
  Create(Factory.Name, Controller, hr);
end;

destructor TRotateTrans.Destroy;
begin
  inherited Destroy;
end;


function TRotateTrans.CheckInputType(mtin: PAMMediaType): HRESULT;
var
  pvi : PVIDEOINFO;
  str : String;
begin
  Result := S_FALSE;
  if IsEqualGUID(mtin.majortype, MEDIATYPE_Video)   and
     IsEqualGUID(mtin.subtype, MEDIASUBTYPE_RGB24)  and
     IsEqualGUID(mtin.formattype, FORMAT_VideoInfo) then
  begin
    pvi := mtin.pbFormat;
    str := Format('Preview Rotate Input -[Width: %d, Height: %d]',
                  [pvi.bmiHeader.biWidth, pvi.bmiHeader.biHeight]);
    OutputDebugString( PChar(str) );
    if (pvi.bmiHeader.biWidth  = MAX_VID_WIDTH ) and
       (pvi.bmiHeader.biHeight = MAX_VID_HEIGHT) and
       (pvi.bmiHeader.biCompression = BI_RGB) then
       begin
          FSrcWidth  := pvi.bmiHeader.biWidth;
          FSrcHeight := pvi.bmiHeader.biHeight;
          FSrcSize   := FSrcWidth * FSrcHeight * SizeOf(TPixcel);

          FShift := ((FSrcWidth - FSrcHeight) div 2);
          ZeroMemory( @DestImgSpace, sizeof(DestImgSpace) );
          pDestImgSpace := @DestImgSpace;

          str := Format('Preview Rotate Output -[Width: %d, Height: %d, Shift: %d]',
                        [FSrcWidth, FSrcHeight, FShift]);
          OutputDebugString( PChar(str) );

          result := S_OK;
       end else if (pvi.bmiHeader.biWidth  = (MAX_VID_WIDTH  div 2) ) and
                   (pvi.bmiHeader.biHeight = (MAX_VID_HEIGHT div 2) ) and
                   (pvi.bmiHeader.biCompression = BI_RGB) then
       begin
          FSrcWidth  := pvi.bmiHeader.biWidth;
          FSrcHeight := pvi.bmiHeader.biHeight;
          FSrcSize   := FSrcWidth * FSrcHeight * SizeOf(TPixcel);

          FShift := ((FSrcWidth - FSrcHeight) div 2);
          ZeroMemory( @DestImgSpace, sizeof(DestImgSpace) );
          pDestImgSpace := @DestImgSpace;

          str := Format('Preview Rotate Output -[Width: %d, Height: %d, Shift: %d]',
                        [FSrcWidth, FSrcHeight, FShift]);
          OutputDebugString( PChar(str) );

          result := S_OK;
       end else if (pvi.bmiHeader.biWidth  = (MAX_VID_WIDTH  div 4) ) and
                   (pvi.bmiHeader.biHeight = (MAX_VID_HEIGHT div 4) ) and
                   (pvi.bmiHeader.biCompression = BI_RGB) then
       begin
          FSrcWidth  := pvi.bmiHeader.biWidth;
          FSrcHeight := pvi.bmiHeader.biHeight;
          FSrcSize   := FSrcWidth * FSrcHeight * SizeOf(TPixcel);

          FShift := ((FSrcWidth - FSrcHeight) div 2);
          ZeroMemory( @DestImgSpace, sizeof(DestImgSpace) );
          pDestImgSpace := @DestImgSpace;

          str := Format('Preview Rotate Output -[Width: %d, Height: %d, Shift: %d]',
                        [FSrcWidth, FSrcHeight, FShift]);
          OutputDebugString( PChar(str) );

          result := S_OK;
       end;
  end;
end;

function TRotateTrans.Transform(Sample: IMediaSample): HRESULT;
var
  pBuff     : PByte;
  pSrc, pDst: PPixcel;
  h, w      : Integer;
  iter, idx : Integer;
  entry     : Integer;
  str       : String;
begin

  Sample.GetPointer(pBuff);
  pSrc          := PPixcel(pBuff);
  pDestImgSpace := @DestImgSpace;
  FisBusy := 1;
  case FRotate of
    0:begin
        pDst := pDestImgSpace;
        CopyMemory( pDestImgSpace, pBuff, FSrcSize );
      end;
    1:begin
        entry := (FSrcHeight - 1) * FSrcWidth;
        for h := 0 to FSrcHeight - 1 do
        begin
          Iter := entry + h;
          for w := 0 to FSrcWidth - 1 do
          begin
           idx  := Iter - (FSrcWidth * w) + FShift;

           if idx >= 0 then
           begin
             pDst := pDestImgSpace;
             Inc(pDst,idx);
             pDst^:= pSrc^;
           end;

           Inc(pSrc);
          end;
        end;
        CopyMemory( pBuff, pDestImgSpace, FSrcSize );
      end;
    2:begin
        Iter := FSrcHeight* FSrcWidth;
        pDst := pDestImgSpace;
        Inc(pDst,Iter);
        for h := 0 to FSrcHeight - 1 do
          begin
          for w := 0 to FSrcWidth - 1 do
          begin
           pDst^:= pSrc^;
           Inc(pSrc);
           Dec(pDst);
          end;
        end;
        CopyMemory( pBuff, pDestImgSpace, FSrcSize );
      end;
    3:begin
        entry := FSrcHeight - 1;
        for h := 0 to FSrcHeight - 1 do
        begin
          Iter := entry - h ;
          for w := 0 to FSrcWidth - 1 do
          begin
           idx  := Iter + FSrcWidth * w + FShift;
           if idx <= (FSrcWidth*FSrcHeight) then
           begin
             pDst := pDestImgSpace;
             Inc(pDst,idx);
             pDst^:= pSrc^;
           end;
           Inc(pSrc);
          end;
        end;
        CopyMemory( pBuff, pDestImgSpace, FSrcSize );
      end;
    else begin
        {$IF FALSE}
        pDst := pDestImgSpace;
        for h := 0 to FSrcHeight - 1 do
        begin
          for w := 0 to FSrcWidth - 1 do
          begin
            pDst^:= pSrc^;
            Inc(pSrc);
            Inc(pDst);
          end;
        end;
        CopyMemory( pBuff, pDestImgSpace, FSrcSize );
        {$ELSE}
        pDst := pDestImgSpace;
        CopyMemory( pDestImgSpace, pBuff, FSrcSize );
        {$IFEND}
    end;
  end;
  FisBusy := 0;
  result  := NOERROR;
end;

function TRotateTrans.SetRotate(sw: Integer): HResult;
  var str : String;
begin
  FRotate := sw;

  ZeroMemory( @DestImgSpace, sizeof(DestImgSpace) );
  pDestImgSpace := @DestImgSpace;

  str := Format('[Preview Rotate Param: %d]',[sw]);
  OutputDebugString( PChar(str) );

  result := S_OK;
end;

function TRotateTrans.GetRGB24( pBuffer : PByte ; pSize : PInteger; pWidth : PInteger ; pHeight : PInteger ):HResult;
  var str : String;
begin

  pSize^  := FSrcSize;
  pWidth^ := FSrcWidth;
  pHeight^:= FSrcHeight;

  if FisBusy = 0 then
  begin
    CopyMemory( pBuffer, @DestImgSpace, FSrcSize );
    result := S_OK;
  end else begin
    result := S_FALSE;
  end;

end;

initialization
  TBCClassFactory.CreateFilter(TRotateTrans, '_RotateTrans', CLSID_TRotateTrans,
    CLSID_LegacyAmFilterCategory, MERIT_DO_NOT_USE, 0,nil);

end.







