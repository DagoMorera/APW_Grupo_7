-- Tabla de Roles del sistema (Admin, User, etc.)
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Tabla de Usuarios, cada usuario tiene un solo rol asignado
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL, -- nunca se guarda la password en texto plano
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- Tabla de Fuentes (APIs de noticias u otras) agregadas por el usuario Admin
CREATE TABLE Sources (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Url NVARCHAR(500) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NULL,
    ComponentType NVARCHAR(100) NOT NULL, -- 'widget','api','feed'
    RequiresSecret BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedBy NVARCHAR(100) NULL
);
GO

-- Tabla de Items obtenidos de cada Source, guardados como JSON
CREATE TABLE SourceItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceId INT NOT NULL,
    Json NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_SourceItems_Sources FOREIGN KEY (SourceId) REFERENCES Sources(Id)
);
GO

-- Tabla de Settings/Secrets, generica o ligada a una Source especifica
CREATE TABLE Settings (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SourceId INT NULL, -- si es NULL, el setting es global de la aplicacion
    KeyName NVARCHAR(100) NOT NULL,
    KeyValue NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    CONSTRAINT FK_Settings_Sources FOREIGN KEY (SourceId) REFERENCES Sources(Id)
);
GO