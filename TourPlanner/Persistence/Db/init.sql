CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS users (
                                     id              uuid            DEFAULT uuid_generate_v4() PRIMARY KEY ,
    username        VARCHAR(255)    NOT NULL UNIQUE,
    password        VARCHAR(255)    NOT NULL,
    name            VARCHAR(50)     ,
    image           VARCHAR(50)     ,
    bio             TEXT            ,
    coins           INTEGER         NOT NULL DEFAULT 20,
    created         TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP
    );