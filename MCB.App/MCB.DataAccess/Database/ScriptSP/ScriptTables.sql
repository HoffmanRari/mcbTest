CREATE TABLE CorruptionData (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(100),
    Iso3 NVARCHAR(3),
    Region NVARCHAR(100),
    Year INT,
    CpiScore FLOAT,
    Rank INT,
    Sources INT,
    StandardError FLOAT
);

CREATE TABLE DevelopmentData (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CountryName NVARCHAR(100),
    CountryCode NVARCHAR(10),
    IndicatorName NVARCHAR(200),
    IndicatorCode NVARCHAR(50),
    Year INT,
    IndicatorValue FLOAT
);
