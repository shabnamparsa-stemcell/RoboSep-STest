@echo off

rem 
rem make.bat
rem Script to create a ZIP archive of byte-compiled Python code
rem 
rem Copyright (c) Invetech Pty Ltd, 2004
rem 495 Blackburn Rd
rem Mt Waverley, Vic, Australia.
rem Phone (+61 3) 9211 7700
rem Fax   (+61 3) 9211 7701
rem 
rem The copyright to the computer program(s) herein is the
rem property of Invetech Pty Ltd, Australia.
rem The program(s) may be used and/or copied only with
rem the written permission of Invetech Operations Pty Ltd
rem or in accordance with the terms and conditions
rem stipulated in the agreement/contract under which
rem the program(s) have been supplied.
rem

rem Byte compile the Python files to optimised .pyo files
rem
rem Since there is no source included for some file, some modules will not be present in .pyo format 
rem   and these will not included later in compile; archive is also commented out as no .pyo generated
rem echo Byte compiling
rem set PYTHONOPTIMIZE=1
rem python -c "import compileall; compileall.compile_dir('tesla', force=True, quiet=True)"

rem Copy the .pyo files into the ZIP archive (but ignore the unit tests)
rem 
rem echo Creating archive
rem del robosep.zip 2> NUL
rem zip -q9r robosep.zip tesla -i *.pyo -x *\tests\*.pyo

rem Now build the launcher exe
rem 
echo Building launcher exe
rem python setup.py --quiet py2exe --includes xml.sax.drivers2.drv_pyexpat,dbhash
pyinstaller  -y RoboSepLauncher.py


rem Update the version information in the installer script file
rem 
echo Updating installer script file with new version details
python version_updater.py

rem Finally, build the installer
rem 
echo Creating installer
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" robosep.iss
rem for x32 systems
rem "c:\Program Files\Inno Setup 5\ISCC.exe" robosep.iss

rem Now turn off the optimise flag
set PYTHONOPTIMIZE=

echo Complete...paused
pause
rem eof

