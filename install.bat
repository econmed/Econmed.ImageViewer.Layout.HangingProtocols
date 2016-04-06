@echo off
cls
setlocal
set installationPath=%ProgramFiles%\ClearCanvas\ClearCanvas DICOM Viewer
set /p installationPath=Enter the ClearCanvas Workstation or DicomViewer installation directory [%installationPath%]: 
echo.
pushd %installationPath%
for /F "delims=" %%r in ('pushd "%ProgramFiles(x86)%\Microsoft SDKs\" ^& dir /b /s /od resgen.exe ^& popd') do (
    "%%r" "%~dp0\source\SR.resx"
    goto resgen-break
)
:resgen-break
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe" /nologo ^
/target:library ^
/reference:.\common\ClearCanvas.Common.dll ^
/reference:.\plugins\ClearCanvas.Dicom.dll ^
/reference:.\plugins\ClearCanvas.Desktop.dll ^
/reference:.\plugins\ClearCanvas.ImageViewer.dll ^
/reference:.\plugins\ClearCanvas.ImageViewer.Layout.Basic.dll ^
/out:.\plugins\Econmed.ImageViewer.Layout.HangingProtocols.dll ^
/resource:"%~dp0\source\SR.resources",Econmed.ImageViewer.Layout.HangingProtocols.SR.resources ^
/resource:"%~dp0\source\Icons\NextLarge.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.NextLarge.png ^
/resource:"%~dp0\source\Icons\NextMedium.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.NextMedium.png ^
/resource:"%~dp0\source\Icons\NextSmall.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.NextSmall.png ^
/resource:"%~dp0\source\Icons\PreviousLarge.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.PreviousLarge.png ^
/resource:"%~dp0\source\Icons\PreviousMedium.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.PreviousMedium.png ^
/resource:"%~dp0\source\Icons\PreviousSmall.png",Econmed.ImageViewer.Layout.HangingProtocols.Icons.PreviousSmall.png ^
/appconfig:"%~dp0\source\app.config" ^
"%~dp0\source\*.cs" "%~dp0\source\Properties\*.cs"
popd
echo.
if errorlevel 1 (
  echo Installation of HangingProtocols plugin failed.
) else (
  echo Installation of HangingProtocols plugin successfully done.
)
pause>NUL
endlocal
