object frmFormat: TfrmFormat
  Left = 0
  Top = 0
  BorderIcons = []
  Caption = 'Camera Format'
  ClientHeight = 188
  ClientWidth = 632
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'Tahoma'
  Font.Style = []
  OldCreateOrder = False
  Position = poOwnerFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object ListFmt: TListBox
    Left = 8
    Top = 8
    Width = 529
    Height = 169
    ItemHeight = 13
    TabOrder = 0
  end
  object BtnSelect: TButton
    Left = 553
    Top = 8
    Width = 75
    Height = 25
    Caption = 'Select'
    TabOrder = 1
    OnClick = BtnSelectClick
  end
  object BtnCancel: TButton
    Left = 553
    Top = 39
    Width = 75
    Height = 25
    Caption = 'Cancel'
    TabOrder = 2
    OnClick = BtnCancelClick
  end
end
