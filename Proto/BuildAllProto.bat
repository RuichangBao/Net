@echo off
set outPut=csharpOutPut

if exist outPut(
    @REM del /f /s /q "%outPut%\*.*"
    println("Hello, world!")
) else (
    mkdir %outPut%
)

@REM for %%i in (*.proto) do (
@REM     echo %%i
@REM     protoc.exe --csharp_out=%outproto%
@REM )
pause