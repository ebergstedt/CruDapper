CREATE FUNCTION dbo.PatternReplace
(
   @InputString NVARCHAR(4000),
   @Pattern NVARCHAR(100),
   @ReplaceText NVARCHAR(4000)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
   DECLARE @Result NVARCHAR(4000) SET @Result = ''
   -- First character in a match
   DECLARE @First INT
   -- Next character to start search on
   DECLARE @Next INT SET @Next = 1
   -- Length of the total string -- 8001 if @InputString is NULL
   DECLARE @Len INT SET @Len = COALESCE(LEN(@InputString), 8001)
   -- End of a pattern
   DECLARE @EndPattern INT
 
   WHILE (@Next <= @Len) 
   BEGIN
      SET @First = PATINDEX('%' + @Pattern + '%', SUBSTRING(@InputString, @Next, @Len))
      IF COALESCE(@First, 0) = 0 --no match - return
      BEGIN
         SET @Result = @Result + 
            CASE --return NULL, just like REPLACE, if inputs are NULL
               WHEN  @InputString IS NULL
                     OR @Pattern IS NULL
                     OR @ReplaceText IS NULL THEN NULL
               ELSE SUBSTRING(@InputString, @Next, @Len)
            END
         BREAK
      END
      ELSE
      BEGIN
         -- Concatenate characters before the match to the result
         SET @Result = @Result + SUBSTRING(@InputString, @Next, @First - 1)
         SET @Next = @Next + @First - 1
 
         SET @EndPattern = 1
         -- Find start of end pattern range
         WHILE PATINDEX(@Pattern, SUBSTRING(@InputString, @Next, @EndPattern)) = 0
            SET @EndPattern = @EndPattern + 1
         -- Find end of pattern range
         WHILE PATINDEX(@Pattern, SUBSTRING(@InputString, @Next, @EndPattern)) > 0
               AND @Len >= (@Next + @EndPattern - 1)
            SET @EndPattern = @EndPattern + 1

         --Either at the end of the pattern or @Next + @EndPattern = @Len
         SET @Result = @Result + @ReplaceText
         SET @Next = @Next + @EndPattern - 1
      END
   END
   RETURN(@Result)
END