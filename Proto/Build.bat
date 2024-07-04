@echo off
set outPut=csharpOutPut

echo remove file in csharpOutPut
if exist %outPut% (
    del /f /s /q "%outPut%\*"
) else (
    mkdir %outPut%
)

echo build proto Start
for %%i in (*.proto) do (
    echo %%i
    protoc.exe --csharp_out=%outPut% %%i
)
echo build proto End

echo "press any key to exit"
pause