CREATE FUNCTION dbo.Tokenize
(
	@Input NVARCHAR(2000)
)
RETURNS @Tokens TABLE 
	(
		TokenNum INT IDENTITY(1,1),
		Token NVARCHAR(2000)
	)
AS
BEGIN
	DECLARE @i INT SET @i = 0
	DECLARE @StartChar INT SET @StartChar = 1
	DECLARE @Quote INT SET @Quote = 0	

	DECLARE @Chars TABLE 
	(
		CharNum INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		TheChar CHAR(1), 
		TheCount INT,
		StartChar INT
	)

	SET @Input = ' , ' + @Input + ' , '
	
	INSERT @Chars (TheChar)
	SELECT SUBSTRING(@Input, n.Number, 1)
	FROM Numbers n
	WHERE n.Number > 0 
		AND n.Number <= LEN(@Input)
	ORDER BY n.Number
	
	UPDATE Chars SET 
		@i = Chars.TheCount = 
			CASE 
				WHEN Chars1.TheChar = ',' 
					AND @Quote % 2 = 0 THEN 0 
				ELSE @i + 1 
			END,
		@Quote = 
			CASE  
				WHEN Chars1.TheChar = '''' THEN @Quote + 1 
				WHEN @i = 0 THEN 0 
				ELSE @Quote 
			END,
		@StartChar = Chars.StartChar =
			CASE
				WHEN @i = 1 THEN Chars1.CharNum - 1
				WHEN @i = 0 THEN @StartChar + 1
				ELSE @StartChar
			END
	FROM @Chars Chars
	JOIN @Chars Chars1 ON Chars1.CharNum = Chars.CharNum + 1

	INSERT @Tokens(Token)
	SELECT
		RTRIM(LTRIM(SUBSTRING(@Input, StartChar, CharNum - StartChar + 1)))
	FROM (
		SELECT StartChar, CharNum
		FROM @Chars
		WHERE TheCount = 0

		UNION ALL

		SELECT 
			MAX
			(
				CASE TheCount 
					WHEN 0 THEN CharNum 
					ELSE 0 
				END
			) + 1, 
			MAX(CharNum)
		FROM @Chars
	) x
	WHERE RTRIM(LTRIM(SUBSTRING(@Input, StartChar, CharNum - StartChar + 1))) NOT IN ('', ',')
	ORDER BY x.StartChar
	RETURN
END