object frmMain: TfrmMain
  Left = 266
  Top = 134
  Caption = 'CamPro'
  ClientHeight = 651
  ClientWidth = 982
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object Panel1: TPanel
    Left = 8
    Top = 8
    Width = 649
    Height = 585
    TabOrder = 0
  end
  object Run_Button: TButton
    Left = 670
    Top = 478
    Width = 97
    Height = 41
    Caption = 'Run'
    TabOrder = 1
    OnClick = Run_ButtonClick
  end
  object Stop_Button: TButton
    Left = 773
    Top = 478
    Width = 97
    Height = 41
    Caption = 'Stop'
    TabOrder = 2
    OnClick = Stop_ButtonClick
  end
  object TrackBar1: TTrackBar
    Left = 670
    Top = 222
    Width = 291
    Height = 59
    Max = 255
    TabOrder = 3
    OnChange = TrackBar1Change
  end
  object Memo1: TMemo
    Left = 670
    Top = 8
    Width = 291
    Height = 161
    ImeName = #54620#44397#50612' '#51077#47141' '#49884#49828#53596' (IME 2000)'
    TabOrder = 4
  end
  object Button1: TButton
    Left = 670
    Top = 175
    Width = 105
    Height = 41
    Caption = 'Send Text'
    TabOrder = 5
    OnClick = Button1Click
  end
  object Button2: TButton
    Left = 670
    Top = 287
    Width = 75
    Height = 25
    Caption = 'Button2'
    TabOrder = 6
    OnClick = Button2Click
  end
  object Button3: TButton
    Left = 773
    Top = 287
    Width = 75
    Height = 25
    Caption = 'Button3'
    TabOrder = 7
    OnClick = Button3Click
  end
  object Button4: TButton
    Left = 670
    Top = 318
    Width = 75
    Height = 25
    Caption = 'Button4'
    TabOrder = 8
    OnClick = Button4Click
  end
  object Button5: TButton
    Left = 773
    Top = 318
    Width = 75
    Height = 25
    Caption = 'Button5'
    TabOrder = 9
    OnClick = Button5Click
  end
end
