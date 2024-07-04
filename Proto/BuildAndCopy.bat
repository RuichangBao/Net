@echo off
set outPut=csharpOutPut
set clientOutPut=..\UnityClient\Assets\Scripts\Proto
set serverOutPut=..\NetWorkBasic\Protocol\Proto

echo remove file in csharpOutPut
if exist %outPut% (
    del /f /s /q "%outPut%\*"
) else (
    mkdir %outPut%
)

echo compile propt Start
for %%i in (*.proto) do (
    echo %%i
    protoc.exe --csharp_out=%outPut% %%i
)
echo compile propt End

echo remove file in Client
if exist %clientOutPut% (
    del /f /s /q "%clientOutPut%\*"
) else (
    mkdir %clientOutPut%
)

echo copy proto for Client
for %%i in ("%outPut%\*cs") do (
    copy %%i %clientOutPut%\
)


echo remove file in Server
if exist %serverOutPut% (
    del /f /s /q "%serverOutPut%\*"
) else (
    mkdir %serverOutPut%
)
echo copy proto for Client
for %%i in ("%outPut%\*") do (
    copy %%i %serverOutPut%\
)
echo "press any key to exit"
pause