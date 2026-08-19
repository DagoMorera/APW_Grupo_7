-- Agrega el soporte de suscripciones y feed personal a la base de datos APW
USE APW;
GO

ALTER TABLE Users ADD FeedToken UNIQUEIDENTIFIER NULL;
GO

UPDATE Users SET FeedToken = NEWID() WHERE FeedToken IS NULL;
GO

ALTER TABLE Users ALTER COLUMN FeedToken UNIQUEIDENTIFIER NOT NULL;
GO

CREATE UNIQUE INDEX IX_Users_FeedToken ON Users(FeedToken);
GO

CREATE TABLE Subscriptions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    SourceId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Subscriptions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Subscriptions_Sources FOREIGN KEY (SourceId) REFERENCES Sources(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Subscriptions_User_Source UNIQUE (UserId, SourceId)
);
GO

DECLARE @fkSourceItems NVARCHAR(200);
SELECT @fkSourceItems = fk.name
FROM sys.foreign_keys fk
WHERE fk.parent_object_id = OBJECT_ID('SourceItems')
  AND fk.referenced_object_id = OBJECT_ID('Sources');
 
IF @fkSourceItems IS NOT NULL
BEGIN
    EXEC('ALTER TABLE SourceItems DROP CONSTRAINT ' + @fkSourceItems);
END
 
ALTER TABLE SourceItems
ADD CONSTRAINT FK_SourceItems_Sources FOREIGN KEY (SourceId) REFERENCES Sources(Id) ON DELETE CASCADE;
GO
 
DECLARE @fkSettings NVARCHAR(200);
SELECT @fkSettings = fk.name
FROM sys.foreign_keys fk
WHERE fk.parent_object_id = OBJECT_ID('Settings')
  AND fk.referenced_object_id = OBJECT_ID('Sources');
 
IF @fkSettings IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Settings DROP CONSTRAINT ' + @fkSettings);
END
 
ALTER TABLE Settings
ADD CONSTRAINT FK_Settings_Sources FOREIGN KEY (SourceId) REFERENCES Sources(Id) ON DELETE CASCADE;
GO