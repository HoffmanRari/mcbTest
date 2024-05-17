CREATE DATABASE macroeconomic_analysis;

CREATE TABLE world_development_indicators (
    country VARCHAR2(100),
    indicator VARCHAR2(255),
    year NUMBER,
    value FLOAT
);

CREATE TABLE corruption_perception_index (
    country VARCHAR2(100),
    year NUMBER,
    cpi_score FLOAT
);
