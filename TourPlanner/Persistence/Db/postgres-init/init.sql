CREATE TABLE IF NOT EXISTS addresses (
    id                  SERIAL          PRIMARY KEY,
    country             VARCHAR(255),
    street              VARCHAR(255),
    housenumber         VARCHAR(255),
    zip                 INT,
    city                VARCHAR(255)    NOT NULL,
    created             TIMESTAMP       DEFAULT CURRENT_TIMESTAMP
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
    child_friendliness  INT,
    created             TIMESTAMP       DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tourlogs (
    id                  SERIAL          PRIMARY KEY,
    tour_date           DATE            NOT NULL,
    comment             VARCHAR(255)    NOT NULL,
    tour_id_fk          INT             NOT NULL    references tours(id) ON DELETE CASCADE,
    difficulty          INT             NOT NULL,
    distance            FLOAT           NOT NULL,
    total_time          INT             NOT NULL,
    rating              INT             NOT NULL,
    created             TIMESTAMP       DEFAULT CURRENT_TIMESTAMP
);

-- Test- Seed

INSERT INTO addresses (country, street, housenumber, zip, city, created)
VALUES ('Austria', 'Höchstädtplatz', '6', 1200, 'Wien', NOW()),
       ('Austria', 'Adalbert-Stifter-Straße', '73', 1200, 'Wien', NOW()),
       ('Austria', '', '', 0, 'Salzburg', NOW());

-- F�gen Sie Daten in die Tour-Tabelle ein
INSERT INTO tours (name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created)
VALUES ('Tour 1', 'Description for Tour 1', 1, 3, 'car', 300, 182, 'car_16,378317_48,238992_12,995288_47,82287.png', 1, 5, NOW()),
       ('Tour 2', 'Description for Tour 2', 1, 2, 'running', 2.2, 26, 'running_16,378317_48,238992_16,377598_48,244099.png', 1, 3, NOW());

-- Insert data into the tourlogs table
INSERT INTO tourlogs (tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created)
VALUES ('2023-05-01', 'Nice weather, challenging paths', 1, 3, 1.2, 60, 5, NOW()),
       ('2023-05-02', 'Perfect day for a long hike', 1, 4, 1.5, 90, 4, NOW()),
       ('2023-05-03', 'Rainy and difficult', 2, 5, 9.2, 270, 3, NOW());
