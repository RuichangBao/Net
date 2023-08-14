@echo off
SET csOutputPath=.\csOutput
SET protoPath=.\proto
SET PROJECT_PATH=..\Client\Assets\Scripts\Proto

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

@REM 删除工程中的proto
if exist %PROJECT_PATH% (
    del /f /s /q %PROJECT_PATH%\*.*
) else ( 
    md %PROJECT_PATH%
)

@REM 拷贝编译后文件

xcopy %csOutputPath% %PROJECT_PATH%\ /s/y

@echo on
pause