@setlocal

SET THIS_PATH=%~dp0
SET LESSIO_TEST_FILES=%THIS_PATH%LessIO.Tests\TestFiles\
packages\xunit.runner.console.2.1.0\tools\xunit.console.x86.exe .\.deploy\LessIO.Tests.dll
