ALTER TABLE dbo.AspNetUsers ADD NormalizedUserName NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetUsers ADD NormalizedEmail NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetUsers ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
ALTER TABLE dbo.AspNetUsers ADD LockoutEnd DATETIMEOFFSET NULL;
GO

UPDATE dbo.AspNetUsers SET NormalizedUserName = UPPER(UserName), NormalizedEmail = UPPER(Email), ConcurrencyStamp = CAST(NEWID() AS NVARCHAR(MAX));
GO

ALTER TABLE dbo.AspNetRoles ADD NormalizedName NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetRoles ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
GO

UPDATE dbo.AspNetRoles SET NormalizedName = UPPER(Name), ConcurrencyStamp = CAST(NEWID() AS NVARCHAR(MAX));
GO
