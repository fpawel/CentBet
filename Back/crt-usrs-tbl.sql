CREATE TABLE [users]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [user] NCHAR(15) NOT NULL, 
    [pass] NCHAR(12) NOT NULL,
	[betuser] NCHAR(30) NULL, 
    [betpass] NCHAR(30) NULL,
)