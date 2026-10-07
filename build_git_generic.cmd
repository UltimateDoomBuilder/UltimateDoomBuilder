@ECHO OFF

ECHO.
ECHO.     This build script requires the following software to be installed:
ECHO.       - Git command-line client
ECHO.       - Microsoft Visual Studio 2019 or newer
ECHO.       - Microsoft HTML Help compiler
ECHO.       - 7zip
ECHO.
ECHO.     You have to commit your work before using this script.
ECHO.     Results will be in the 'GIT_Build' directory.
ECHO.     Files in the 'GIT_Build' directory may be overwritten.
ECHO.
ECHO.

SET STUDIODIR=C:\Program Files\Microsoft Visual Studio\2022\Community
SET HHWDIR=c:\Program Files (x86)\HTML Help Workshop
SET SEVENZIPDIR=c:\Program Files\7-Zip
SET ISSDIR=c:\Program Files (x86)\Inno Setup 6

IF NOT DEFINED PLATFORM SET PLATFORM=x86

SET DB_OUTDIR="%CD%\GIT_Build"
IF DEFINED BUILD_RELEASE SET DB_OUTDIR="%CD%\Release"

ECHO %DB_OUTDIR%

CALL "%STUDIODIR%\Common7\Tools\vsdevcmd.bat" %PLATFORM%
ECHO.
ECHO Building for platform %PLATFORM%
ECHO.

MKDIR %DB_OUTDIR%

rem git.exe checkout "Source/Core/Properties/AssemblyInfo.cs" > NUL
rem git.exe checkout "Source/Plugins/BuilderModes/Properties/AssemblyInfo.cs" > NUL

ECHO.
ECHO Writing GIT log file...
ECHO.
IF EXIST "%DB_OUTDIR%\Changelog.xml" DEL /F /Q "%DB_OUTDIR%\Changelog.xml" > NUL
(
echo [OB]?xml version="1.0" encoding="UTF-8"?[CB]
echo [OB]log[CB]
git.exe log master --since=2012-04-17 --pretty=format:"[OB]logentry commit=\"%%h\"[CB]%%n[OB]author[CB]%%an[OB]/author[CB]%%n[OB]date[CB]%%aI[OB]/date[CB]%%n[OB]msg[CB]%%B[OB]/msg[CB]%%n[OB]/logentry[CB]"
echo [OB]/log[CB]
) >"%DB_OUTDIR%\Changelog.xml"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "%DB_OUTDIR%\Changelog.xml" GOTO FILEFAIL

rem ECHO.
rem ECHO Compiling HTML Help file...
rem ECHO.
rem IF NOT EXIST "Build" MKDIR "Build"
rem IF EXIST "Build\Refmanual.chm" DEL /F /Q "Build\Refmanual.chm" > NUL
rem "%HHWDIR%\hhc" Help\Refmanual.hhp
rem IF %ERRORLEVEL% NEQ 1 GOTO ERRORFAIL
rem IF NOT EXIST "Build\Refmanual.chm" GOTO FILEFAIL

rem ECHO.
rem ECHO Looking up current repository revision numbers...
rem ECHO.
rem IF EXIST "setenv.bat" DEL /F /Q "setenv.bat" > NUL
rem IF DEFINED EXPERIMENTALNAME (
rem 	VersionFromGIT.exe "Source\Core\Properties\AssemblyInfo.cs" "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" -O "setenv.bat" -N %EXPERIMENTALNAME%
rem ) ELSE (
rem 	VersionFromGIT.exe "Source\Core\Properties\AssemblyInfo.cs" "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" -O "setenv.bat"
rem )
rem IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
rem IF NOT EXIST "setenv.bat" GOTO FILEFAIL
rem 
rem CALL "setenv.bat"
rem DEL /F /Q "setenv.bat"

ECHO.
ECHO Cleaning solutions...
ECHO.
msbuild.exe Builder.sln /t:Clean
rem msbuild.exe Source/Tools/Updater/Updater.csproj /t:Clean

rem ECHO.
rem ECHO Compiling Updater...
rem ECHO.
rem IF EXIST "Build\Updater.exe" DEL /F /Q "Build\Updater.exe" > NUL
rem IF EXIST "Source\Tools\Updater\obj" RD /S /Q "Source\Tools\Updater\obj"
rem msbuild "Source\Tools\Updater\Updater.csproj" /t:Rebuild /p:Configuration=Release /p:Platform=%PLATFORM% /v:minimal
rem IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
rem IF NOT EXIST "Build\Updater.exe" GOTO FILEFAIL

rem VersionFromEXE.exe "Build\Updater.exe" "setenv.bat"
rem IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
rem IF NOT EXIST "setenv.bat" GOTO FILEFAIL
rem CALL "setenv.bat"
rem DEL /F /Q "setenv.bat"

ECHO.
ECHO Looking up current repository revision number...
ECHO.
git rev-list --count HEAD > REVISIONNUMBER.txt
IF %ERRORLEVEL% EQU 0 (
	set /p REVISIONNUMBER=<REVISIONNUMBER.txt
) ELSE (
	set REVISIONNUMBER=unknown
)
IF EXIST REVISIONNUMBER.txt DEL /F /Q REVISIONNUMBER.txt > NUL
ECHO Got revision number: %REVISIONNUMBER%

ECHO.
ECHO Compiling Doom Builder...
ECHO.
IF DEFINED EXPERIMENTALNAME (
	echo ##### BUILDING EXPERIMENTAL VERSION %EXPERIMENTALNAME%
	msbuild.exe Builder.sln /t:Rebuild /p:Configuration=Release /p:Platform=%PLATFORM% /v:minimal /p:DefineConstants="TRACE;NO_UPDATER"
) ELSE (
	msbuild.exe Builder.sln /t:Rebuild /p:Configuration=Release /p:Platform=%PLATFORM% /v:minimal
)
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "Build\Builder.exe" GOTO FILEFAIL
IF NOT EXIST "Build\BuilderNative.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\AutomapMode.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\BuilderEffects.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\BuilderModes.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\ColorPicker.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\CommentsPanel.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\NodesViewer.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\SoundPropagationMode.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\StairSectorBuilder.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\TagExplorer.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\TagRange.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\ThreeDFloorMode.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Plugins\VisplaneExplorer.dll" GOTO FILEFAIL
IF NOT EXIST "Build\Updater.exe" GOTO FILEFAIL

ECHO.
ECHO Creating changelog...
ECHO.
ChangelogMaker.exe "%DB_OUTDIR%\Changelog.xml" "Build" "m-x-d>MaxED" %REVISIONNUMBER%
IF %ERRORLEVEL% NEQ 0 GOTO LOGFAIL

VersionFromEXE.exe "Build\Updater.exe" "setenv.bat"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "setenv.bat" GOTO FILEFAIL
CALL "setenv.bat"
DEL /F /Q "setenv.bat"

ECHO.
ECHO Packing release...
ECHO.

IF NOT DEFINED BUILD_RELEASE GOTO PACKGIT

set DEL_PATHSPEC="%DB_OUTDIR%\UltimateDoomBuilder-Setup*-%PLATFORM%.exe"
IF EXIST %DEL_PATHSPEC% DEL /F /Q %DEL_PATHSPEC% > NUL
"%ISSDIR%\iscc.exe" /DUDB_arch=%PLATFORM% "Setup\UDBuilder_setup.iss"
IF %ERRORLEVEL% NEQ 0 GOTO ERRORFAIL
IF NOT EXIST "%DB_OUTDIR%\Setup.exe" GOTO FILEFAIL

REN "%DB_OUTDIR%\Setup.exe" "UltimateDoomBuilder-Setup-R%REVISIONNUMBER%-%PLATFORM%.exe"

REM GOTO BUILDDONE

:PACKGIT
SET DEL_PATHSPEC="%DB_OUTDIR%\UltimateDoomBuilder*-%PLATFORM%.7z"
IF EXIST %DEL_PATHSPEC% DEL /F /Q %DEL_PATHSPEC% > NUL
IF EXIST "%DB_OUTDIR%\UDB_Updater-%PLATFORM%.7z" DEL /F /Q "%DB_OUTDIR%\UDB_Updater-%PLATFORM%.7z" > NUL
"%SEVENZIPDIR%\7z" a %DB_OUTDIR%\udb.7z .\Build\* -xr!*.xml -xr!JetBrains.Profiler.Core.Api.dll -xr!ScintillaNET.3.5.pdb -x!Setup -x!OpenGLDebug.log -x!Builder.vshost.* -x!.gitignore -x!UDBScript\Scripts\*.js
"%SEVENZIPDIR%\7z" a %DB_OUTDIR%\UDB_Updater-%PLATFORM%.7z .\Build\Updater.exe .\Build\Updater.ini
IF %ERRORLEVEL% NEQ 0 GOTO PACKFAIL
IF NOT EXIST %DB_OUTDIR%\udb.7z GOTO FILEFAIL
IF NOT EXIST %DB_OUTDIR%\UDB_Updater-%PLATFORM%.7z GOTO FILEFAIL

REN "%DB_OUTDIR%\udb.7z" UltimateDoomBuilder-r%REVISIONNUMBER%-%PLATFORM%.7z

IF EXIST "Build\Changelog.txt" DEL /F /Q "Build\Changelog.txt" > NUL

@ECHO %REVISIONNUMBER%> %DB_OUTDIR%\Version.txt
@ (ECHO %REVISIONNUMBER% && ECHO %EXEREVISIONNUMBER%) > %DB_OUTDIR%\Versions.txt

rem git.exe checkout "Source\Core\Properties\AssemblyInfo.cs" > NUL
rem git.exe checkout "Source\Plugins\BuilderModes\Properties\AssemblyInfo.cs" > NUL

:BUILDDONE
ECHO.
ECHO.     BUILD DONE !
ECHO.
ECHO.     Revision:  %REVISIONNUMBER% (%REVISIONHASH%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:ERRORFAIL
ECHO.
ECHO.     BUILD FAILED (Tool returned error %ERRORLEVEL%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:PACKFAIL
ECHO.
ECHO.     PACKAGING FAILED (7zip returned error %ERRORLEVEL%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:FILEFAIL
ECHO.
ECHO.     BUILD FAILED (Output file was not built)
ECHO.
PAUSE > NUL
GOTO LEAVE

:LOGFAIL
ECHO.
ECHO.     CHANGELOG GENERATION FAILED (Tool returned error %ERRORLEVEL%)
ECHO.
PAUSE > NUL
GOTO LEAVE

:LEAVE
exit