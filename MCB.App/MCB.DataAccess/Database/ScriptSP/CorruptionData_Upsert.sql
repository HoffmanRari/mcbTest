CREATE PROCEDURE CorruptionData_Upsert
    @CountryName NVARCHAR(100),
    @Iso3 NVARCHAR(3),
    @Region NVARCHAR(100),
    @Year INT,
    @CpiScore FLOAT,
    @Rank INT,
    @Sources INT,
    @StandardError FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM CorruptionData WHERE CountryName = @CountryName AND Year = @Year)
    BEGIN
        UPDATE CorruptionData
        SET Iso3 = @Iso3,
            Region = @Region,
            CpiScore = @CpiScore,
            Rank = @Rank,
            Sources = @Sources,
            StandardError = @StandardError
        WHERE CountryName = @CountryName AND Year = @Year;
    END
    ELSE
    BEGIN
        INSERT INTO CorruptionData (CountryName, Iso3, Region, Year, CpiScore, Rank, Sources, StandardError)
        VALUES (@CountryName, @Iso3, @Region, @Year, @CpiScore, @Rank, @Sources, @StandardError);
    END
END;
