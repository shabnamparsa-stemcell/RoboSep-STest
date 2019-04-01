library Update;

uses
  SysUtils,
  Windows;

procedure MyDllFunc(hWnd: Integer; lpText, lpCaption: PAnsiChar; uType: Cardinal); stdcall;
begin
  MessageBoxA(hWnd, lpText, lpCaption, uType);
end;

procedure UpdateSerialNumber(FilePath, Serial : PAnsiChar);stdcall;
  var msg     : AnsiString;
      txtFile : TextFile;

begin
  msg := Format('>> Rx : %s %s',[FilePath, Serial]);
  OutputDebugStringA(PAnsiChar(msg));
  OutputDebugStringA('>> RX: Start Write');
  try
    OutputDebugStringA('>> RX: Start...');
    AssignFile(txtFile,FilePath);
    Rewrite(txtFile);
    WriteLn(txtFile,Serial);
    Close(txtFile);
    OutputDebugStringA('>> RX: Write done!');
  except
    OutputDebugStringA('>> RX: Write Error!');
  end;
end;

exports MyDllFunc, UpdateSerialNumber;

begin
end.
