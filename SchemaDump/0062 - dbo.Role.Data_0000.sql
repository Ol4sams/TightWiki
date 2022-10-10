CREATE TABLE #tmp_09867a7ed9ed447f86b072b4c576da3d ([Id] [int],[Name] [nvarchar](128),[Description] [nvarchar](1000))
GO

INSERT INTO #tmp_09867a7ed9ed447f86b072b4c576da3d ([Name],[Description]) VALUES
('Administrator','Administrators can do anything. Add, edit, delete, etc.'),
('Member','Read-only user with a profile.'),
('Contributor','Contributor can add and edit pages.'),
('Moderator','Moderators can add, edit and delete pages.'),
('Guest','Guest have no specific privileges, but will be automatically logged in.')
GO
ALTER TABLE [dbo].[Role] NOCHECK CONSTRAINT ALL
GO
INSERT INTO [dbo].[Role] (
	[Name],[Description])
SELECT
	[Name],[Description]
FROM #tmp_09867a7ed9ed447f86b072b4c576da3d as S
ALTER TABLE [dbo].[Role] CHECK CONSTRAINT ALL
GO
DROP TABLE #tmp_09867a7ed9ed447f86b072b4c576da3d
GO