
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/15/2023 11:34:08
-- Generated from EDMX file: C:\Users\Jessica.XPS\source\repos\EventUp\EventUpLib\EventUpModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [EventUpBD];
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

-- Creating table 'PersonSet'
CREATE TABLE [dbo].[PersonSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FamilyName] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [TelephoneNumber] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Role] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'EventSet'
CREATE TABLE [dbo].[EventSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [NumberOfGuests] int  NOT NULL,
    [Budget] float  NOT NULL,
    [Start_DateTime] datetime  NOT NULL,
    [End_DateTime] datetime  NOT NULL,
    [Typ_Event] nvarchar(max)  NOT NULL,
    [isPlannedById] int  NOT NULL
);
GO

-- Creating table 'ServiceSet'
CREATE TABLE [dbo].[ServiceSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [FixCost] float  NOT NULL,
    [HourCost] float  NOT NULL,
    [PersonCost] float  NOT NULL,
    [Capacity] int  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Typ_Event] nvarchar(max)  NOT NULL,
    [Typ_Service] nvarchar(max)  NOT NULL,
    [More] nvarchar(max)  NOT NULL,
    [isOfferedById] int  NOT NULL
);
GO

-- Creating table 'istBookedFor'
CREATE TABLE [dbo].[istBookedFor] (
    [have_Id] int  NOT NULL,
    [isBookedFor_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'PersonSet'
ALTER TABLE [dbo].[PersonSet]
ADD CONSTRAINT [PK_PersonSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EventSet'
ALTER TABLE [dbo].[EventSet]
ADD CONSTRAINT [PK_EventSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ServiceSet'
ALTER TABLE [dbo].[ServiceSet]
ADD CONSTRAINT [PK_ServiceSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [have_Id], [isBookedFor_Id] in table 'istBookedFor'
ALTER TABLE [dbo].[istBookedFor]
ADD CONSTRAINT [PK_istBookedFor]
    PRIMARY KEY CLUSTERED ([have_Id], [isBookedFor_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [isPlannedById] in table 'EventSet'
ALTER TABLE [dbo].[EventSet]
ADD CONSTRAINT [FK_plans]
    FOREIGN KEY ([isPlannedById])
    REFERENCES [dbo].[PersonSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_plans'
CREATE INDEX [IX_FK_plans]
ON [dbo].[EventSet]
    ([isPlannedById]);
GO

-- Creating foreign key on [isOfferedById] in table 'ServiceSet'
ALTER TABLE [dbo].[ServiceSet]
ADD CONSTRAINT [FK_offers]
    FOREIGN KEY ([isOfferedById])
    REFERENCES [dbo].[PersonSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_offers'
CREATE INDEX [IX_FK_offers]
ON [dbo].[ServiceSet]
    ([isOfferedById]);
GO

-- Creating foreign key on [have_Id] in table 'istBookedFor'
ALTER TABLE [dbo].[istBookedFor]
ADD CONSTRAINT [FK_istBookedFor_Service]
    FOREIGN KEY ([have_Id])
    REFERENCES [dbo].[ServiceSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [isBookedFor_Id] in table 'istBookedFor'
ALTER TABLE [dbo].[istBookedFor]
ADD CONSTRAINT [FK_istBookedFor_Event]
    FOREIGN KEY ([isBookedFor_Id])
    REFERENCES [dbo].[EventSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_istBookedFor_Event'
CREATE INDEX [IX_FK_istBookedFor_Event]
ON [dbo].[istBookedFor]
    ([isBookedFor_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------