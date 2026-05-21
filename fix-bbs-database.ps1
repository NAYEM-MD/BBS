# Run as Administrator (right-click PowerShell -> Run as administrator)
#
# Front and Manager BOTH use the same database:
#   MDF files: Web\w2.BBS.Front\App_Data\bbs.mdf (+ bbs_log.ldf)
#   Connection: Data Source=.\SQLEXPRESS; Initial Catalog=bbs; ...
#
# Before running:
#   1. Stop debugging in Visual Studio (Shift+F5)
#   2. In Server Explorer, disconnect any connection to bbs.mdf (right-click -> Disconnect)
#   3. Close browser tabs for Front / Manager

$ErrorActionPreference = "Stop"
$mdf = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data\bbs.mdf"
$ldf = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data\bbs_log.ldf"
$appData = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data"
$localDbDbName = "C:\INETPUB\WWWROOT\TRAINING.BBS\WEB\W2.BBS.FRONT\APP_DATA\BBS.MDF"

Write-Host "Stopping IIS..."
iisreset /stop

Write-Host "Detaching bbs.mdf from LocalDB (Visual Studio often locks the file here)..."
sqlcmd -S "(LocalDb)\MSSQLLocalDB" -Q @"
IF DB_ID(N'$localDbDbName') IS NOT NULL
BEGIN
  ALTER DATABASE [$localDbDbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
  EXEC sp_detach_db @dbname = N'$localDbDbName';
END
"@ 2>$null | Out-Null

Write-Host "Detaching any BBS databases from SQL Express..."
sqlcmd -S ".\SQLEXPRESS" -Q @"
DECLARE @sql nvarchar(max) = N'';
SELECT @sql = @sql + N'ALTER DATABASE [' + name + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [' + name + N'];'
FROM sys.databases
WHERE name = N'bbs'
   OR name LIKE N'%BBS.MDF%';
IF LEN(@sql) > 0 EXEC sp_executesql @sql;
"@ 2>$null | Out-Null

Start-Sleep -Seconds 2

Write-Host "Attaching database as 'bbs' on SQL Express..."
sqlcmd -S ".\SQLEXPRESS" -Q @"
CREATE DATABASE bbs ON
(FILENAME = N'$mdf'),
(FILENAME = N'$ldf')
FOR ATTACH;
"@

Write-Host "Granting SQL logins (IIS runs as NT AUTHORITY\SYSTEM or AppPool)..."
sqlcmd -S ".\SQLEXPRESS" -Q @"
USE master;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'NT AUTHORITY\SYSTEM')
  CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS;
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS AppPool\DefaultAppPool')
  CREATE LOGIN [IIS AppPool\DefaultAppPool] FROM WINDOWS;
USE bbs;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'NT AUTHORITY\SYSTEM')
BEGIN
  CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM];
  ALTER ROLE db_owner ADD MEMBER [NT AUTHORITY\SYSTEM];
END
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS AppPool\DefaultAppPool')
BEGIN
  CREATE USER [IIS AppPool\DefaultAppPool] FOR LOGIN [IIS AppPool\DefaultAppPool];
  ALTER ROLE db_owner ADD MEMBER [IIS AppPool\DefaultAppPool];
END
"@ | Out-Null

Write-Host "Granting App_Data folder permissions..."
icacls $appData /grant "IIS_IUSRS:(OI)(CI)M" /T | Out-Null
icacls $appData /grant "NT AUTHORITY\SYSTEM:(OI)(CI)F" /T | Out-Null
icacls $appData /grant "IIS AppPool\DefaultAppPool:(OI)(CI)M" /T | Out-Null

Write-Host "Starting IIS..."
iisreset /start

Write-Host ""
Write-Host "Done. Verify:"
sqlcmd -S ".\SQLEXPRESS" -Q "SELECT name, state_desc FROM sys.databases WHERE name = N'bbs';" -W
Write-Host ""
Write-Host "Front:   http://localhost/Training.BBS/Web/w2.BBS.Front/"
Write-Host "Manager: http://localhost/Training.BBS/Web/w2.BBS.Manager/"
Write-Host "Manager login: admin / admin123"
