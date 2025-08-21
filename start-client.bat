@echo off
echo Starting HTTP Client...
echo.
echo Make sure the server is running before continuing!
echo (Run start-server.bat in another window first)
echo.
pause
cd HttpListenerClientApp
dotnet run
echo.
echo Press any key to close this window...
pause
