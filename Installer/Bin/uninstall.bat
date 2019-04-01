echo Uninstalling Server
start /wait .\uninst\unins000.exe /SILENT /SUPPRESSMSGBOXES
echo uninstalling Client Application
start /wait msiexec.exe /x{FE4A5E9C-FA36-469A-B96E-2EF03253CBB0} /q
echo uninstalling Service Application
start /wait msiexec.exe /x{42C85EB0-B5F4-40B6-AE8B-9FA9CEDA4801} /q
echo uninstalling Editor Application
start /wait msiexec.exe /x{22A835CC-670F-47ED-B71D-990F0969E64A} /q

