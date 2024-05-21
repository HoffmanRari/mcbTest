CREATE OR REPLACE PACKAGE BODY data_package AS
    PROCEDURE load_or_update_corruption_data(
        p_country_name IN VARCHAR2,
        p_iso3 IN VARCHAR2,
        p_region IN VARCHAR2,
        p_year IN NUMBER,
        p_cpi_score IN NUMBER,
        p_rank IN NUMBER,
        p_sources IN NUMBER,
        p_standard_error IN NUMBER
    ) IS
    BEGIN
        MERGE INTO corruption_perception_index cpi
        USING (SELECT p_country_name AS country_name, p_iso3 AS iso3, p_region AS region, p_year AS year, 
                      p_cpi_score AS cpi_score, p_rank AS rank, p_sources AS sources, p_standard_error AS standard_error FROM dual) src
        ON (cpi.country_name = src.country_name AND cpi.year = src.year)
        WHEN MATCHED THEN
            UPDATE SET 
                cpi.iso3 = src.iso3,
                cpi.region = src.region,
                cpi.cpi_score = src.cpi_score,
                cpi.rank = src.rank,
                cpi.sources = src.sources,
                cpi.standard_error = src.standard_error
        WHEN NOT MATCHED THEN
            INSERT (country_name, iso3, region, year, cpi_score, rank, sources, standard_error) 
            VALUES (src.country_name, src.iso3, src.region, src.year, src.cpi_score, src.rank, src.sources, src.standard_error);
    END load_or_update_corruption_data;

    PROCEDURE load_or_update_development_data(
        p_country_name IN VARCHAR2,
        p_country_code IN VARCHAR2,
        p_indicator_name IN VARCHAR2,
        p_indicator_code IN VARCHAR2,
        p_year IN NUMBER,
        p_indicator_value IN NUMBER
    ) IS
    BEGIN
        MERGE INTO world_development_indicators wdi
        USING (SELECT p_country_name AS country_name, p_country_code AS country_code, p_indicator_name AS indicator_name, 
                      p_indicator_code AS indicator_code, p_year AS year, p_indicator_value AS indicator_value FROM dual) src
        ON (wdi.country_name = src.country_name AND wdi.indicator_name = src.indicator_name AND wdi.year = src.year)
        WHEN MATCHED THEN
            UPDATE SET 
                wdi.country_code = src.country_code,
                wdi.indicator_code = src.indicator_code,
                wdi.indicator_value = src.indicator_value
        WHEN NOT MATCHED THEN
            INSERT (country_name, country_code, indicator_name, indicator_code, year, indicator_value) 
            VALUES (src.country_name, src.country_code, src.indicator_name, src.indicator_code, src.year, src.indicator_value);
    END load_or_update_development_data;
END data_package;
/

