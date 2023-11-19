
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/19/2023 11:12:51
-- Generated from EDMX file: C:\Users\Jessica.XPS\source\repos\EventUp\EventUpLib\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EventUpDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [FamilyName] nvarchar(max)  NULL,
    [TelephoneNumber] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Role_Admin] bit  NOT NULL,
    [Role_Supplier] bit  NOT NULL,
    [Role_Planner] bit  NOT NULL
);
GO

-- Creating table 'Services'
CREATE TABLE [dbo].[Services] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [Typ_Service] nvarchar(max)  NOT NULL,
    [Typ_Event] nvarchar(max)  NULL,
    [Capacity] int  NULL,
    [FixCost] float  NULL,
    [HourCost] float  NULL,
    [PersonCost] float  NULL,
    [City] nvarchar(max)  NOT NULL,
    [More] nvarchar(max)  NULL,
    [isOfferedById] int  NOT NULL
);
GO

-- Creating table 'Events'
CREATE TABLE [dbo].[Events] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NULL,
    [NumberOfGuest] int  NOT NULL,
    [Budget] float  NULL,
    [Typ_Event] nvarchar(max)  NULL,
    [Start_DateTime] datetime  NOT NULL,
    [End_DateTime] datetime  NOT NULL,
    [isPlannedById] int  NOT NULL
);
GO

-- Creating table 'isBookedFor'
CREATE TABLE [dbo].[isBookedFor] (
    [have_Id] int  NOT NULL,
    [isBookedFor_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [PK_Services]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [PK_Events]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [have_Id], [isBookedFor_Id] in table 'isBookedFor'
ALTER TABLE [dbo].[isBookedFor]
ADD CONSTRAINT [PK_isBookedFor]
    PRIMARY KEY CLUSTERED ([have_Id], [isBookedFor_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [isPlannedById] in table 'Events'
ALTER TABLE [dbo].[Events]
ADD CONSTRAINT [FK_plans]
    FOREIGN KEY ([isPlannedById])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_plans'
CREATE INDEX [IX_FK_plans]
ON [dbo].[Events]
    ([isPlannedById]);
GO

-- Creating foreign key on [isOfferedById] in table 'Services'
ALTER TABLE [dbo].[Services]
ADD CONSTRAINT [FK_offers]
    FOREIGN KEY ([isOfferedById])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_offers'
CREATE INDEX [IX_FK_offers]
ON [dbo].[Services]
    ([isOfferedById]);
GO

-- Creating foreign key on [have_Id] in table 'isBookedFor'
ALTER TABLE [dbo].[isBookedFor]
ADD CONSTRAINT [FK_isBookedFor_Service]
    FOREIGN KEY ([have_Id])
    REFERENCES [dbo].[Services]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [isBookedFor_Id] in table 'isBookedFor'
ALTER TABLE [dbo].[isBookedFor]
ADD CONSTRAINT [FK_isBookedFor_Event]
    FOREIGN KEY ([isBookedFor_Id])
    REFERENCES [dbo].[Events]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_isBookedFor_Event'
CREATE INDEX [IX_FK_isBookedFor_Event]
ON [dbo].[isBookedFor]
    ([isBookedFor_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------