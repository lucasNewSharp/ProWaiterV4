DECLARE @banco VARCHAR(255)   
DECLARE @tabela VARCHAR(255)  
DECLARE @cmd NVARCHAR(500)  

DECLARE DatabaseCursor CURSOR FOR  
SELECT name FROM master.dbo.sysdatabases   
WHERE name NOT IN ('master','model','msdb','tempdb')   
ORDER BY 1 

OPEN DatabaseCursor  

FETCH NEXT FROM DatabaseCursor INTO @banco  
WHILE @@FETCH_STATUS = 0  
BEGIN  

   SET @cmd = 'DECLARE TableCursor CURSOR FOR 
				SELECT table_catalog + ''.'' + table_schema + ''.'' + table_name as tableName   
                FROM ' + @banco + '.INFORMATION_SCHEMA.TABLES WHERE table_type = ''BASE TABLE'''   

   -- create table cursor  
   EXEC (@cmd)  
   OPEN TableCursor   

   FETCH NEXT FROM TableCursor INTO @tabela   
   WHILE @@FETCH_STATUS = 0   
   BEGIN   
       	PRINT N'Atualizando indices da tabela ' + @tabela + '...'
		SET @cmd = 'ALTER INDEX ALL ON ' + @tabela + ' REBUILD'  
		EXEC (@cmd)  

		FETCH NEXT FROM TableCursor INTO @tabela   
   END   

   CLOSE TableCursor   
   DEALLOCATE TableCursor  

   FETCH NEXT FROM DatabaseCursor INTO @banco  
END  
CLOSE DatabaseCursor   
DEALLOCATE DatabaseCursor
PRINT N'Atualização concluída!'