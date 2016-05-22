@setlocal 
@set /P TEMPVER= Enter the version number to build for:

msbuild.exe .\.build\lessio.msbuild /p:TheVersion=%TEMPVER%
