@echo off


rem Now build the launcher exe
rem 
echo Building launcher exe
rem python setup.py --quiet py2exe --includes abc --includes xml.sax.drivers2.drv_pyexpat --includes dbhash
python setup.py --quiet py2exe --includes xml.sax.drivers2.drv_pyexpat,dbhash 

rem Finally, build the installer
rem 
echo Creating installer
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" robosep.iss

echo Server Installer Build is Complete...pausing
pause
rem eof

