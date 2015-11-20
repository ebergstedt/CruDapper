DECLARE @RunDate datetime
SET @RunDate=GETDATE()
CREATE TABLE dbo.Numbers (Number  int  not null)  
INSERT INTO dbo.Numbers(Number)
SELECT TOP 1000000 row_number() over(order by t1.number) as N
FROM master..spt_values t1 
    CROSS JOIN master..spt_values t2
ALTER TABLE dbo.Numbers ADD CONSTRAINT PK_NumbersTest PRIMARY KEY CLUSTERED (Number);