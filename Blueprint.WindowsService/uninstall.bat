@echo off
set currentpath=%cd%
@echo %currentpath%
START /wait %SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File ""%currentpath%\uninstall.ps1""' -Verb RunAs}"
pause