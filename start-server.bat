@echo off
echo Starting HTTP Listener Server...
echo.
echo This will start the server on http://localhost:8888/
echo Press 'q' in the server window to stop it
echo.
pause
cd HttpListenerApp
dotnet run
