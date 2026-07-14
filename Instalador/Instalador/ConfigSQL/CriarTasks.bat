schtasks /create /tn "Reconstruir todos os indices" /xml "C:\NewSharp\BancoDeDados\Backup\Reconstruir Indices do Sql Server.xml"
schtasks /create /tn "Backup SQL Server Full" /xml "C:\NewSharp\BancoDeDados\Backup\Backup SQL Server Full.xml"
schtasks /create /tn "Backup SQL Server Diferencial" /xml "C:\NewSharp\BancoDeDados\Backup\Backup SQL Server Diferencial.xml"