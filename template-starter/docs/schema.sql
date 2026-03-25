-- Schema Template
-- Keep this as source of truth

CREATE TABLE IF NOT EXISTS sample_entities (
  id CHAR(36) PRIMARY KEY,
  name VARCHAR(100) NOT NULL,
  created_at_utc DATETIME NOT NULL
);
