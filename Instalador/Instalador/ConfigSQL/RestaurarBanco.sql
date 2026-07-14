USE [master]
GO
EXEC master.dbo.sp_addumpdevice  @devtype = N'disk', @logicalname = N'ProWaiterBKP', @physicalname = N'C:\NewSharp\BancoDeDados\Backup\ProWaiter.bak'
GO

USE [master]
RESTORE DATABASE [ProWaiter] FROM  DISK = N'C:\NewSharp\BancoDeDados\Backup\ProWaiter.bak' WITH  FILE = 1,  MOVE N'ProWaiter' TO N'C:\Program Files\Microsoft SQL Server\MSSQL15.NEWSHARP\MSSQL\DATA\ProWaiter.mdf',  MOVE N'ProWaiter_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL15.NEWSHARP\MSSQL\DATA\ProWaiter_log.ldf',  NOUNLOAD,  STATS = 5
GO

USE [master]
GO
CREATE LOGIN [ProWaiterDB] WITH PASSWORD=N'ProW123', DEFAULT_DATABASE=[ProWaiter], DEFAULT_LANGUAGE=[Português], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

use [ProWaiter]
GO
alter user ProWaiterDB with login = ProWaiterDB
GO
