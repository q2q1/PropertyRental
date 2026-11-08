IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [FirstName] nvarchar(max) NOT NULL,
        [LastName] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [Buildings] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(120) NOT NULL,
        [StreetAddress] nvarchar(200) NOT NULL,
        [City] nvarchar(80) NULL,
        [PostalCode] nvarchar(20) NULL,
        [PropertyOwnerId] nvarchar(450) NOT NULL,
        [PropertyManagerId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_Buildings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Buildings_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Buildings_AspNetUsers_PropertyOwnerId] FOREIGN KEY ([PropertyOwnerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [Messages] (
        [Id] int NOT NULL IDENTITY,
        [SenderId] nvarchar(450) NOT NULL,
        [ReceiverId] nvarchar(450) NOT NULL,
        [Subject] nvarchar(200) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [SentUtc] datetime2 NOT NULL,
        [IsRead] bit NOT NULL,
        CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [OwnerEventReports] (
        [Id] int NOT NULL IDENTITY,
        [PropertyManagerId] nvarchar(450) NOT NULL,
        [PropertyOwnerId] nvarchar(450) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(4000) NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [IsAcknowledged] bit NOT NULL,
        CONSTRAINT [PK_OwnerEventReports] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OwnerEventReports_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OwnerEventReports_AspNetUsers_PropertyOwnerId] FOREIGN KEY ([PropertyOwnerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [Apartments] (
        [Id] int NOT NULL IDENTITY,
        [BuildingId] int NOT NULL,
        [UnitNumber] nvarchar(40) NOT NULL,
        [Bedrooms] int NOT NULL,
        [Bathrooms] int NOT NULL,
        [MonthlyRent] decimal(12,2) NOT NULL,
        [Status] int NOT NULL,
        [Description] nvarchar(2000) NULL,
        [ImageUrl] nvarchar(500) NULL,
        CONSTRAINT [PK_Apartments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Apartments_Buildings_BuildingId] FOREIGN KEY ([BuildingId]) REFERENCES [Buildings] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE TABLE [Appointments] (
        [Id] int NOT NULL IDENTITY,
        [TenantId] nvarchar(450) NOT NULL,
        [PropertyManagerId] nvarchar(450) NOT NULL,
        [ApartmentId] int NOT NULL,
        [StartUtc] datetime2 NOT NULL,
        [Status] int NOT NULL,
        [Notes] nvarchar(1000) NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Appointments_Apartments_ApartmentId] FOREIGN KEY ([ApartmentId]) REFERENCES [Apartments] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Appointments_AspNetUsers_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Apartments_BuildingId] ON [Apartments] ([BuildingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_ApartmentId] ON [Appointments] ([ApartmentId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_PropertyManagerId] ON [Appointments] ([PropertyManagerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Appointments_TenantId] ON [Appointments] ([TenantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Buildings_PropertyManagerId] ON [Buildings] ([PropertyManagerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Buildings_PropertyOwnerId] ON [Buildings] ([PropertyOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OwnerEventReports_PropertyManagerId] ON [OwnerEventReports] ([PropertyManagerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_OwnerEventReports_PropertyOwnerId] ON [OwnerEventReports] ([PropertyOwnerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260419172805_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260419172805_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

/*
  PropertyRental — SQL Server DDL (Identity uses default table names: AspNetRoles, AspNetUsers, …)
  Matches EF Core Identity defaults — no ToTable overrides in ApplicationDbContext.
  No __EFMigrationsHistory. Run on empty database; script can create PropertyRental if missing.
*/

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'PropertyRental')
    CREATE DATABASE [PropertyRental];
GO

USE [PropertyRental];
GO

/* --- ASP.NET Core Identity (default table names) --- */

CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR(450) NOT NULL,
    [Name]             NVARCHAR(256) NULL,
    [NormalizedName]   NVARCHAR(256) NULL,
    [ConcurrencyStamp] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR(450) NOT NULL,
    [FirstName]            NVARCHAR(MAX) NOT NULL,
    [LastName]             NVARCHAR(MAX) NOT NULL,
    [Address]              NVARCHAR(MAX) NULL,
    [UserName]             NVARCHAR(256) NULL,
    [NormalizedUserName]   NVARCHAR(256) NULL,
    [Email]                NVARCHAR(256) NULL,
    [NormalizedEmail]      NVARCHAR(256) NULL,
    [EmailConfirmed]       BIT NOT NULL,
    [PasswordHash]         NVARCHAR(MAX) NULL,
    [SecurityStamp]        NVARCHAR(MAX) NULL,
    [ConcurrencyStamp]     NVARCHAR(MAX) NULL,
    [PhoneNumber]          NVARCHAR(MAX) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled]     BIT NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET NULL,
    [LockoutEnabled]       BIT NOT NULL,
    [AccessFailedCount]    INT NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [dbo].[AspNetRoleClaims] (
    [Id]         INT IDENTITY NOT NULL,
    [RoleId]     NVARCHAR(450) NOT NULL,
    [ClaimType]  NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT IDENTITY NOT NULL,
    [UserId]     NVARCHAR(450) NOT NULL,
    [ClaimType]  NVARCHAR(MAX) NULL,
    [ClaimValue] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR(128) NOT NULL,
    [ProviderKey]         NVARCHAR(128) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId]              NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(128) NOT NULL,
    [Name]          NVARCHAR(128) NOT NULL,
    [Value]         NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

/* --- Domain --- */

CREATE TABLE [dbo].[Buildings] (
    [Id]                INT IDENTITY NOT NULL,
    [Name]              NVARCHAR(120) NOT NULL,
    [StreetAddress]     NVARCHAR(200) NOT NULL,
    [City]              NVARCHAR(80) NULL,
    [PostalCode]        NVARCHAR(20) NULL,
    [PropertyOwnerId]   NVARCHAR(450) NOT NULL,
    [PropertyManagerId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_Buildings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Buildings_AspNetUsers_PropertyOwnerId] FOREIGN KEY ([PropertyOwnerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Buildings_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [dbo].[Messages] (
    [Id]         INT IDENTITY NOT NULL,
    [SenderId]   NVARCHAR(450) NOT NULL,
    [ReceiverId] NVARCHAR(450) NOT NULL,
    [Subject]    NVARCHAR(200) NOT NULL,
    [Body]       NVARCHAR(MAX) NOT NULL,
    [SentUtc]    DATETIME2 NOT NULL,
    [IsRead]     BIT NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [dbo].[OwnerEventReports] (
    [Id]                INT IDENTITY NOT NULL,
    [PropertyManagerId] NVARCHAR(450) NOT NULL,
    [PropertyOwnerId]   NVARCHAR(450) NOT NULL,
    [Title]             NVARCHAR(200) NOT NULL,
    [Description]       NVARCHAR(4000) NOT NULL,
    [CreatedUtc]        DATETIME2 NOT NULL,
    [IsAcknowledged]    BIT NOT NULL,
    CONSTRAINT [PK_OwnerEventReports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OwnerEventReports_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OwnerEventReports_AspNetUsers_PropertyOwnerId] FOREIGN KEY ([PropertyOwnerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [dbo].[Apartments] (
    [Id]          INT IDENTITY NOT NULL,
    [BuildingId]  INT NOT NULL,
    [UnitNumber]  NVARCHAR(40) NOT NULL,
    [Bedrooms]    INT NOT NULL,
    [Bathrooms]   INT NOT NULL,
    [MonthlyRent] DECIMAL(12, 2) NOT NULL,
    [Status]      INT NOT NULL,
    [Description] NVARCHAR(2000) NULL,
    [ImageUrl]    NVARCHAR(500) NULL,
    CONSTRAINT [PK_Apartments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Apartments_Buildings_BuildingId] FOREIGN KEY ([BuildingId]) REFERENCES [dbo].[Buildings] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [dbo].[Appointments] (
    [Id]                INT IDENTITY NOT NULL,
    [TenantId]          NVARCHAR(450) NOT NULL,
    [PropertyManagerId] NVARCHAR(450) NOT NULL,
    [ApartmentId]       INT NOT NULL,
    [StartUtc]          DATETIME2 NOT NULL,
    [Status]            INT NOT NULL,
    [Notes]             NVARCHAR(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_Apartments_ApartmentId] FOREIGN KEY ([ApartmentId]) REFERENCES [dbo].[Apartments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_AspNetUsers_PropertyManagerId] FOREIGN KEY ([PropertyManagerId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_AspNetUsers_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE NO ACTION
);
GO

/* --- Indexes (same names EF typically creates) --- */

CREATE INDEX [IX_Apartments_BuildingId] ON [dbo].[Apartments] ([BuildingId]);
GO
CREATE INDEX [IX_Appointments_ApartmentId] ON [dbo].[Appointments] ([ApartmentId]);
GO
CREATE INDEX [IX_Appointments_PropertyManagerId] ON [dbo].[Appointments] ([PropertyManagerId]);
GO
CREATE INDEX [IX_Appointments_TenantId] ON [dbo].[Appointments] ([TenantId]);
GO
CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims] ([RoleId]);
GO
EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
GO
CREATE INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims] ([UserId]);
GO
CREATE INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins] ([UserId]);
GO
CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles] ([RoleId]);
GO
CREATE INDEX [EmailIndex] ON [dbo].[AspNetUsers] ([NormalizedEmail]);
GO
EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
GO
CREATE INDEX [IX_Buildings_PropertyManagerId] ON [dbo].[Buildings] ([PropertyManagerId]);
GO
CREATE INDEX [IX_Buildings_PropertyOwnerId] ON [dbo].[Buildings] ([PropertyOwnerId]);
GO
CREATE INDEX [IX_Messages_ReceiverId] ON [dbo].[Messages] ([ReceiverId]);
GO
CREATE INDEX [IX_Messages_SenderId] ON [dbo].[Messages] ([SenderId]);
GO
CREATE INDEX [IX_OwnerEventReports_PropertyManagerId] ON [dbo].[OwnerEventReports] ([PropertyManagerId]);
GO
CREATE INDEX [IX_OwnerEventReports_PropertyOwnerId] ON [dbo].[OwnerEventReports] ([PropertyOwnerId]);
GO
