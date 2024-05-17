CREATE OR REPLACE PACKAGE BODY data_pipeline_pkg IS

    PROCEDURE load_corruption_perception_index(p_country IN VARCHAR2, p_year IN NUMBER, p_cpi_score IN FLOAT) IS
    BEGIN
        INSERT INTO corruption_perception_index (country, year, cpi_score)
        VALUES (p_country, p_year, p_cpi_score);
    END load_corruption_perception_index;

    PROCEDURE load_world_development_indicators(p_country IN VARCHAR2, p_indicator IN VARCHAR2, p_year IN NUMBER, p_value IN FLOAT) IS
    BEGIN
        INSERT INTO world_development_indicators (country, indicator, year, value)
        VALUES (p_country, p_indicator, p_year, p_value);
    END load_world_development_indicators;

END data_pipeline_pkg;
/