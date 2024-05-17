CREATE OR REPLACE PACKAGE data_pipeline_pkg IS
    PROCEDURE load_corruption_perception_index(p_country IN VARCHAR2, p_year IN NUMBER, p_cpi_score IN FLOAT);
    PROCEDURE load_world_development_indicators(p_country IN VARCHAR2, p_indicator IN VARCHAR2, p_year IN NUMBER, p_value IN FLOAT);
END data_pipeline_pkg;
/

