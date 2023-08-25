@echo off
SET csOutputPath=.\csOutput
SET protoPath=.\proto
SET CLIENT_PATH=..\Client\Assets\Scripts\Proto
SET SERVER_PATH=..\Server\Server\Proto

if exist %csOutputPath% (
    del /f /s /q %csOutputPath%\*.*
) else ( 
    md %csOutputPath%
)

@REM 编译proto
for %%p in (%protoPath%\*.proto) do ( 
    echo %%p
    protoc  --csharp_out=.\%csOutputPath% %%p
)

@REM 删除客户端工程中的proto
if exist %CLIENT_PATH% (
    del /f /s /q %CLIENT_PATH%\*.*
) else ( 
    md %CLIENT_PATH%
)

@REM 删除服务器工程中的proto
if exist %SERVER_PATH% (
    del /f /s /q %SERVER_PATH%\*.*
) else ( 
    md %SERVER_PATH%
)

@REM 拷贝编译后文件
xcopy %csOutputPath% %CLIENT_PATH%\ /s/y
xcopy %csOutputPath% %SERVER_PATH%\ /s/y

pause
@echo on
