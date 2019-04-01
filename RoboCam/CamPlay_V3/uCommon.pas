unit uCommon;

interface

uses Windows;

const
  CLSID_TRotateTrans: TGUID = '{5C4B9332-ED86-4465-9024-6C167D552AF4}';

type
  IRotateInf = interface(IUnknown)
    ['{6BE5C524-3A19-41BE-B979-11DCB80679CF}']
    function SetRotate(sw : Integer):HResult;stdcall;
    function GetRGB24( pBuffer : PByte ; var Size : Cardinal; var Width : Cardinal ; var Height : Cardinal ):HResult;stdcall;
  end;

implementation

end.
