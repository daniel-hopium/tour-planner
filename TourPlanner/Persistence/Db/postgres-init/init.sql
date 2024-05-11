CREATE TABLE IF NOT EXISTS addresses (
    id                  SERIAL          PRIMARY KEY,
    street              VARCHAR(255)    NOT NULL,
    housenumber         VARCHAR(255),
    zip                 INT             NOT NULL,
    city                VARCHAR(255)    NOT NULL,
    created             TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tours (
    id                  SERIAL          PRIMARY KEY,
    name                VARCHAR(255)    NOT NULL,
    description         VARCHAR(255)    NOT NULL,
    from_address_fk     INT             NOT NULL    references addresses(id),
    to_address_fk       INT             NOT NULL    references addresses(id),
    transport_type      VARCHAR(255)    NOT NULL,            
    distance            FLOAT           NOT NULL,
    est_time            INT             NOT NULL,
    image               VARCHAR(255)    NOT NULL,
    popularity          INT             NOT NULL DEFAULT 0,
    child_friendliness  INT             NOT NULL DEFAULT 0,
    created             TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tourlogs (
    id                  SERIAL          PRIMARY KEY,
    tour_date           VARCHAR(255)    NOT NULL,
    comment             VARCHAR(255),
    tour_id_fk          INT             NOT NULL    references tours(id),
    difficulty          INT             NOT NULL,
    distance            FLOAT,
    total_time          INT             NOT NULL,
    rating              INT             NOT NULL,
    created             TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Test- Seed

INSERT INTO addresses (street, housenumber, zip, city, created)
VALUES ('Strasse', '1a', 1200, 'Vienna', NOW()),
       ('Hauptstrasse', '4', 1190, 'Vienna', NOW());

-- Fügen Sie Daten in die Tour-Tabelle ein
INSERT INTO tours (name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created)
VALUES ('Tour 1', 'Description for Tour 1', 1, 1, 'hike', 1.2, 90, '/Persistence/Images/image1.png', 3, 5, NOW()),
       ('Tour 2', 'Description for Tour 2', 1, 2, 'hike', 9.2, 270, '/Persistence/Images/image2.png', 1, 3, NOW());