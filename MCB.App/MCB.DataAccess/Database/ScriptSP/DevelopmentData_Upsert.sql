CREATE PROCEDURE DevelopmentData_Upsert
    @CountryName NVARCHAR(100),
    @CountryCode NVARCHAR(10),
    @IndicatorName NVARCHAR(200),
    @IndicatorCode NVARCHAR(50),
    @Year INT,
    @IndicatorValue FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM DevelopmentData WHERE CountryName = @CountryName AND IndicatorCode = @IndicatorCode AND Year = @Year)
    BEGIN
        UPDATE DevelopmentData
        SET CountryCode = @CountryCode,
            IndicatorName = @IndicatorName,
            IndicatorValue = @IndicatorValue
        WHERE CountryName = @CountryName AND IndicatorCode = @IndicatorCode AND Year = @Year;
    END
    ELSE
    BEGIN
        INSERT INTO DevelopmentData (CountryName, CountryCode, IndicatorName, IndicatorCode, Year, IndicatorValue)
        VALUES (@CountryName, @CountryCode, @IndicatorName, @IndicatorCode, @Year, @IndicatorValue);
    END
END;
