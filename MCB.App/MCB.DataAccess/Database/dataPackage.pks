CREATE OR REPLACE PACKAGE data_package AS
    PROCEDURE load_or_update_corruption_data(
        p_country_name IN VARCHAR2,
        p_iso3 IN VARCHAR2,
        p_region IN VARCHAR2,
        p_year IN NUMBER,
        p_cpi_score IN NUMBER,
        p_rank IN NUMBER,
        p_sources IN NUMBER,
        p_standard_error IN NUMBER
    );

    PROCEDURE load_or_update_development_data(
        p_country_name IN VARCHAR2,
        p_country_code IN VARCHAR2,
        p_indicator_name IN VARCHAR2,
        p_indicator_code IN VARCHAR2,
        p_year IN NUMBER,
        p_indicator_value IN NUMBER
    );
END data_package;
/