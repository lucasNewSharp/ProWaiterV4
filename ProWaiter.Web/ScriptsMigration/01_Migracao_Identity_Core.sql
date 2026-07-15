-- =================================================================================
-- Script de Migração: Identity do .NET Framework 4.8 para .NET Core 10 (Identity Core)
-- =================================================================================

-- 1. Adicionando as colunas necessárias na tabela AspNetUsers
ALTER TABLE dbo.AspNetUsers ADD NormalizedUserName NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetUsers ADD NormalizedEmail NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetUsers ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
ALTER TABLE dbo.AspNetUsers ADD LockoutEnd DATETIMEOFFSET NULL;
GO

-- 2. Populando as novas colunas da tabela AspNetUsers com dados existentes
UPDATE dbo.AspNetUsers 
SET NormalizedUserName = UPPER(UserName), 
    NormalizedEmail = UPPER(Email), 
    ConcurrencyStamp = CAST(NEWID() AS NVARCHAR(MAX));
GO

-- 3. Adicionando as colunas necessárias na tabela AspNetRoles
ALTER TABLE dbo.AspNetRoles ADD NormalizedName NVARCHAR(256) NULL;
ALTER TABLE dbo.AspNetRoles ADD ConcurrencyStamp NVARCHAR(MAX) NULL;
GO

-- 4. Populando as novas colunas da tabela AspNetRoles
UPDATE dbo.AspNetRoles 
SET NormalizedName = UPPER(Name), 
    ConcurrencyStamp = CAST(NEWID() AS NVARCHAR(MAX));
GO

-- 5. Criando a nova tabela de Tokens utilizada nativamente pelo Identity Core
CREATE TABLE dbo.AspNetUserTokens (
    UserId nvarchar(128) NOT NULL,
    LoginProvider nvarchar(128) NOT NULL,
    Name nvarchar(128) NOT NULL,
    Value nvarchar(max) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY CLUSTERED (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE
);
GO

-- 6. Criando a tabela de RoleClaims utilizada nativamente pelo Identity Core
CREATE TABLE dbo.AspNetRoleClaims (
    Id int IDENTITY(1,1) NOT NULL,
    RoleId nvarchar(128) NOT NULL,
    ClaimType nvarchar(max) NULL,
    ClaimValue nvarchar(max) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles (Id) ON DELETE CASCADE
);
GO
