CREATE FUNCTION dbo.SplitString
(
	@List NVARCHAR(MAX),
	@Delimiter CHAR(1)
)
RETURNS @ReturnTbl TABLE 
(
	OutParam NVARCHAR(20)
)
WITH SCHEMABINDING
AS
BEGIN
	DECLARE @LeftSplit VARCHAR(7998)
	DECLARE @SplitStart INT SET @SplitStart = 0
	DECLARE @SplitEnd INT
	SET @SplitEnd = 7997

	SELECT 
		@SplitEnd = MAX(Number)
	FROM dbo.Numbers
	WHERE 
		(
			REPLACE(SUBSTRING(@List, Number, 1), ' ', CHAR(255)) = 
				REPLACE(@Delimiter, ' ', CHAR(255))
			OR Number = DATALENGTH(@List) + 1
		)
		AND Number BETWEEN @SplitStart AND @SplitEnd

	WHILE @SplitStart < @SplitEnd
	BEGIN
		SET @LeftSplit = 
			@Delimiter + 
			SUBSTRING(@List, @SplitStart, @SplitEnd - @SplitStart) + 
			@Delimiter

		INSERT @ReturnTbl 
		(
			OutParam
		)
		SELECT 
			LTRIM
			(
				RTRIM
				(
					SUBSTRING
					(
						@LeftSplit, 
						Number + 1,
	                    CHARINDEX(@Delimiter, @LeftSplit, Number + 1) - Number - 1
					)
				)
			) AS Value
		FROM dbo.Numbers
		WHERE  
			Number <= LEN(@LeftSplit) - 1
			AND REPLACE(SUBSTRING(@LeftSplit, Number, 1), ' ', CHAR(255)) = 
				REPLACE(@Delimiter, ' ', CHAR(255))
			AND '' <>
				SUBSTRING
				(
					@LeftSplit, 
					Number + 1, 
					CHARINDEX(@Delimiter, @LeftSplit, Number + 1) - Number - 1
				)

		SET @SplitStart = @SplitEnd + 1
		SET @SplitEnd = @SplitEnd + 7997

		SELECT 
			@SplitEnd = MAX(Number) + @SplitStart
		FROM dbo.Numbers
		WHERE 
			(
				REPLACE(SUBSTRING(@List, Number + @SplitStart, 1), ' ', CHAR(255)) = 
					REPLACE(@Delimiter, ' ', CHAR(255))
				OR Number + @SplitStart = DATALENGTH(@List) + 1
			)
			AND Number BETWEEN 1 AND @SplitEnd - @SplitStart
	END

	RETURN
END
GO