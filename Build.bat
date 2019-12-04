@@ECHO OFF
REM Переходим в папку в которой лежит этот командный файл
cd "%~dp0"
REM Устанавливаем пути к программам
SET MSBUILD="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
SET VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
IF EXIST %VSWHERE% (
	FOR /f "usebackq tokens=*" %%i in (`%VSWHERE% -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) DO (
		SET MSBUILD="%%i"
	)
)
SET PATHCMD=%~dp0
SET NGBUILD="%PATHCMD%\.nuget\nuget.exe"
CLS
del Output /S /F /Q
%NGBUILD% restore osm2mssql.sln
%MSBUILD% osm2mssql.sln /p:Configuration=Release