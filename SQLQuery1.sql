CREATE TABLE [dbo].[Registrations] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FullName] NVARCHAR(100) NOT NULL,
    [Phone] VARCHAR(20) NOT NULL,
    [BirthDate] DATE NOT NULL,
    [Gender] NVARCHAR(10) NOT NULL,
    [CourseId] VARCHAR(20) NOT NULL,
    [CourseName] NVARCHAR(100) NOT NULL,
    [Extras] NVARCHAR(200) NULL
);