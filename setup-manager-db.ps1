# Run this script as Administrator once to fix Manager DB connection under IIS.
# Right-click PowerShell -> Run as administrator, then:
#   Set-ExecutionPolicy Bypass -Scope Process -Force
#   & "C:\inetpub\wwwroot\Training.BBS\setup-manager-db.ps1"

$ErrorActionPreference = "Stop"
$mdf = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data\bbs.mdf"
$ldf = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data\bbs_log.ldf"
$appData = "C:\inetpub\wwwroot\Training.BBS\Web\w2.BBS.Front\App_Data"

Write-Host "Stopping IIS..."
iisreset /stop

Write-Host "Granting folder permissions..."
icacls $appData /grant "IIS_IUSRS:(OI)(CI)M" /T | Out-Null
icacls $appData /grant "NT AUTHORITY\SYSTEM:(OI)(CI)F" /T | Out-Null
icacls $appData /grant "IIS AppPool\DefaultAppPool:(OI)(CI)M" /T | Out-Null

Write-Host "Detaching database from LocalDB if attached..."
sqlcmd -S "(LocalDb)\MSSQLLocalDB" -Q "IF DB_ID(N'TrainingBBS') IS NOT NULL BEGIN ALTER DATABASE TrainingBBS SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname = N'TrainingBBS'; END" | Out-Null
sqlcmd -S "(LocalDb)\MSSQLLocalDB" -Q "IF DB_ID(N'$($mdf.ToUpper())') IS NOT NULL BEGIN ALTER DATABASE [$($mdf.ToUpper())] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; EXEC sp_detach_db @dbname = N'$($mdf.ToUpper())'; END" 2>$null | Out-Null

Write-Host "Granting SQL logins for IIS..."
sqlcmd -S ".\SQLEXPRESS" -Q "USE master; IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'NT AUTHORITY\SYSTEM') CREATE LOGIN [NT AUTHORITY\SYSTEM] FROM WINDOWS; IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS AppPool\DefaultAppPool') CREATE LOGIN [IIS AppPool\DefaultAppPool] FROM WINDOWS;" | Out-Null
sqlcmd -S ".\SQLEXPRESS" -d bbs -Q "IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'NT AUTHORITY\SYSTEM') BEGIN CREATE USER [NT AUTHORITY\SYSTEM] FOR LOGIN [NT AUTHORITY\SYSTEM]; ALTER ROLE db_owner ADD MEMBER [NT AUTHORITY\SYSTEM]; END; IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS AppPool\DefaultAppPool') BEGIN CREATE USER [IIS AppPool\DefaultAppPool] FOR LOGIN [IIS AppPool\DefaultAppPool]; ALTER ROLE db_owner ADD MEMBER [IIS AppPool\DefaultAppPool]; END;" | Out-Null

Write-Host "Starting IIS..."
iisreset /start

Write-Host "Done. Database should be attached to SQL Express as 'bbs'."
Write-Host "Open http://localhost/Training.BBS/Web/w2.BBS.Manager/ and log in with admin / admin123"
