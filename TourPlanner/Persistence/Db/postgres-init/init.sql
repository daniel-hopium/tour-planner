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
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (1, 'Austria', 'Höchstädtplatz', '6', 1200, 'Wien', '2024-06-26 15:38:45.730379');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (2, 'Austria', 'Adalbert-Stifter-Straße', '73', 1200, 'Wien', '2024-06-26 15:38:45.730379');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (3, 'Austria', '', '', 0, 'Salzburg', '2024-06-26 15:38:45.730379');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (4, '', 'Wallgasse', '18', 1060, 'Wien', '2024-06-26 18:57:07.828532');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (5, '', 'Thorenbergstrasse', '49', 6014, 'Luzern', '2024-06-26 18:57:07.860746');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (6, '', 'Badgasse', '42', 2405, 'Bad Deutsch-Altenburg', '2024-06-26 19:50:09.122109');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (7, '', 'Ottenstein', '1', 3532, 'Ottenstein', '2024-06-26 19:50:09.159635');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (8, '', 'Prinzlstraße', '22', 3390, 'Melk', '2024-06-26 19:54:16.728010');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (9, '', 'Innsbrucker Bundesstraße', '136', 5020, 'Salzburg', '2024-06-26 19:54:16.739915');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (10, '', 'Wöhrlehenstraße', '41', 5324, 'Anger', '2024-06-26 19:56:25.888832');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (11, '', 'Kirchenweg', '7', 5324, 'Faistenau', '2024-06-26 19:56:25.894323');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (12, '', 'Vilpianer Straße', '', 39010, 'Nals', '2024-06-26 19:58:33.337744');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (13, '', 'Zwingenburgweg', '3', 39010, 'Prissian', '2024-06-26 19:58:33.342667');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (14, '', 'Loambauern', '1', 39050, 'San Genesio Atesino BZ', '2024-06-26 20:01:44.578156');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (15, '', 'Möltnerstraße', '3', 39010, 'Meltina Autonome Provinz Bozen', '2024-06-26 20:01:44.583347');
INSERT INTO public.addresses (id, country, street, housenumber, zip, city, created) VALUES (16, '', 'Austria', '', 0, 'Salzburg', '2024-06-26 20:02:44.981692');


-- F�gen Sie Daten in die Tour-Tabelle ein
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (8, 'St Jenesien', 'St. Jenesienwanderung', 14, 15, 'hike', 10.557, 126, 'hike_11,330559_46,536671_11,254361_46,587068.png', 5, 0, '2024-06-26 20:01:44.586465');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (7, 'Südtirol', 'Wandertour', 12, 13, 'hike', 5.347, 74, 'hike_11,21285_46,540547_11,172681_46,553283.png', 5, 3, '2024-06-26 19:58:33.345526');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (1, 'Autotour', 'Description for Tour 1', 1, 16, 'car', 299.996, 182, 'car_16,378317_48,238992_12,995288_47,82287.png', 1, 5, '2024-06-26 15:38:45.748631');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (2, 'Wienspaziergang', 'Description for Tour 2', 1, 2, 'running', 0.654, 7, 'running_16,378317_48,238992_16,377598_48,244099.png', 2, 4, '2024-06-26 15:38:45.748631');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (3, 'Testtour', 'Testroute', 4, 5, 'bike', 838.273, 2863, 'bike_16,339738_48,192093_8,257608_47,058027.png', 3, 1, '2024-06-26 18:57:07.885224');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (4, 'Internationale Radtour', 'Radtour von Österreich nach Deutschland', 6, 7, 'bike', 166.4991, 564, 'bike_16,902952_48,115389_15,3357_48,595947.png', 4, 4, '2024-06-26 19:50:09.186124');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (5, 'Autotour', 'Autotour', 8, 9, 'car', 219.252, 124, 'car_15,332986_48,21428_13,007241_47,798377.png', 5, 3, '2024-06-26 19:54:16.743059');
INSERT INTO public.tours (id, name, description, from_address_fk, to_address_fk, transport_type, distance, est_time, image, popularity, child_friendliness, created) VALUES (6, 'Wandertour', 'Wandertour', 10, 11, 'hike', 952.905, 11448, 'hike_13,238239_47,764884_5,221634_51,756378.png', 5, 3, '2024-06-26 19:56:25.897627');

-- Insert data into the tourlogs table
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36191, '2024-06-26', 'Was nice', 2, 2, 5, 30, 4, '2024-06-26 20:31:57.542485');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36192, '2024-06-26', 'Was pretty Cool', 3, 4, 5, 50, 3, '2024-06-26 20:32:33.872439');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36193, '2024-06-26', 'Decent Cycletour', 4, 4, 1, 23, 2, '2024-06-26 20:33:01.862543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36194, '2024-06-26', 'Loved it', 5, 2, 213, 33, 1, '2024-06-26 20:33:24.014497');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36195, '2024-06-19', 'Not so good', 6, 1, 213, 213, 5, '2024-06-26 20:33:52.371621');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36196, '2024-06-26', 'Decent Tour', 8, 3, 1111, 123, 4, '2024-06-26 20:34:20.920131');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (36197, '2024-06-26', 'War der hammer', 7, 4, 12, 3, 5, '2024-06-26 20:34:56.253687');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35368, '2024-06-27', 'Tour 1 - Log 1', 1, 3, 10.5, 60, 4, '2024-06-26 20:21:32.703876');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35369, '2024-06-27', 'Tour 2 - Log 1', 2, 2, 5, 30, 5, '2024-06-26 20:21:32.723871');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35370, '2024-06-28', 'Tour 2 - Log 2', 2, 3, 6, 40, 4, '2024-06-26 20:21:32.723871');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35371, '2024-06-27', 'Tour 3 - Log 1', 3, 4, 7, 50, 3, '2024-06-26 20:21:32.739491');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35372, '2024-06-28', 'Tour 3 - Log 2', 3, 3, 8, 60, 4, '2024-06-26 20:21:32.739491');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35373, '2024-06-29', 'Tour 3 - Log 3', 3, 2, 9, 70, 5, '2024-06-26 20:21:32.739491');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35374, '2024-06-30', 'Tour 3 - Log 4', 3, 5, 10, 80, 2, '2024-06-26 20:21:32.739491');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35375, '2024-06-27', 'Tour 4 - Log 1', 4, 1, 2, 20, 5, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35376, '2024-06-28', 'Tour 4 - Log 2', 4, 2, 3, 30, 4, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35377, '2024-06-29', 'Tour 4 - Log 3', 4, 3, 4, 40, 3, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35378, '2024-06-30', 'Tour 4 - Log 4', 4, 4, 5, 50, 2, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35379, '2024-07-01', 'Tour 4 - Log 5', 4, 5, 6, 60, 1, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35380, '2024-07-02', 'Tour 4 - Log 6', 4, 1, 7, 70, 5, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35381, '2024-07-03', 'Tour 4 - Log 7', 4, 2, 8, 80, 4, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35382, '2024-07-04', 'Tour 4 - Log 8', 4, 3, 9, 90, 3, '2024-06-26 20:21:32.759383');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35383, '2024-06-27', 'Tour 5 - Log 1', 5, 5, 8.61, 116, 5, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35384, '2024-06-28', 'Tour 5 - Log 2', 5, 1, 5.27, 49, 6, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35385, '2024-06-29', 'Tour 5 - Log 3', 5, 6, 1.46, 69, 5, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35386, '2024-06-30', 'Tour 5 - Log 4', 5, 1, 1.35, 94, 5, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35387, '2024-07-01', 'Tour 5 - Log 5', 5, 5, 10.46, 32, 1, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35388, '2024-07-02', 'Tour 5 - Log 6', 5, 3, 8.1, 86, 4, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35389, '2024-07-03', 'Tour 5 - Log 7', 5, 2, 9.1, 71, 2, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35390, '2024-07-04', 'Tour 5 - Log 8', 5, 2, 7.1, 54, 4, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35391, '2024-07-05', 'Tour 5 - Log 9', 5, 4, 10.86, 69, 6, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35392, '2024-07-06', 'Tour 5 - Log 10', 5, 4, 4.77, 67, 2, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35393, '2024-07-07', 'Tour 5 - Log 11', 5, 5, 5.74, 66, 5, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35394, '2024-07-08', 'Tour 5 - Log 12', 5, 5, 7.55, 43, 5, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35395, '2024-07-09', 'Tour 5 - Log 13', 5, 6, 8.59, 120, 3, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35396, '2024-07-10', 'Tour 5 - Log 14', 5, 2, 10.47, 88, 3, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35397, '2024-07-11', 'Tour 5 - Log 15', 5, 3, 1.89, 105, 4, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35398, '2024-07-12', 'Tour 5 - Log 16', 5, 2, 4.11, 90, 6, '2024-06-26 20:21:32.792543');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35639, '2024-06-27', 'Tour 6 - Log 1', 6, 6, 4.26, 117, 1, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35640, '2024-06-28', 'Tour 6 - Log 2', 6, 3, 5.99, 103, 5, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35641, '2024-06-29', 'Tour 6 - Log 3', 6, 1, 2.08, 39, 4, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35642, '2024-06-30', 'Tour 6 - Log 4', 6, 5, 3.06, 43, 5, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35643, '2024-07-01', 'Tour 6 - Log 5', 6, 4, 10.6, 121, 3, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35644, '2024-07-02', 'Tour 6 - Log 6', 6, 1, 4.75, 40, 6, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35645, '2024-07-03', 'Tour 6 - Log 7', 6, 5, 2.08, 60, 5, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35646, '2024-07-04', 'Tour 6 - Log 8', 6, 5, 1.55, 84, 4, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35647, '2024-07-05', 'Tour 6 - Log 9', 6, 2, 5.81, 51, 4, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35738, '2024-07-06', 'Tour 6 - Log 10', 6, 5, 4.29, 74, 1, '2024-06-26 20:21:32.823246');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35739, '2024-06-27', 'Tour 7 - Log 1', 7, 3, 9.32, 125, 5, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35740, '2024-06-28', 'Tour 7 - Log 2', 7, 4, 7.66, 88, 5, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35741, '2024-06-29', 'Tour 7 - Log 3', 7, 3, 7.43, 101, 4, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35742, '2024-06-30', 'Tour 7 - Log 4', 7, 3, 6.07, 81, 1, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35743, '2024-07-01', 'Tour 7 - Log 5', 7, 2, 1.17, 55, 5, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35744, '2024-07-02', 'Tour 7 - Log 6', 7, 3, 1.4, 49, 3, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35745, '2024-07-03', 'Tour 7 - Log 7', 7, 3, 9.5, 107, 2, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35746, '2024-07-04', 'Tour 7 - Log 8', 7, 5, 4.52, 115, 6, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35747, '2024-07-05', 'Tour 7 - Log 9', 7, 5, 2.57, 44, 6, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35748, '2024-07-06', 'Tour 7 - Log 10', 7, 2, 8.11, 79, 5, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35749, '2024-07-07', 'Tour 7 - Log 11', 7, 2, 3.68, 123, 3, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35750, '2024-07-08', 'Tour 7 - Log 12', 7, 2, 5.32, 95, 6, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35751, '2024-07-09', 'Tour 7 - Log 13', 7, 1, 2.32, 56, 1, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35752, '2024-07-10', 'Tour 7 - Log 14', 7, 4, 8.19, 121, 2, '2024-06-26 20:21:32.847631');
INSERT INTO public.tourlogs (id, tour_date, comment, tour_id_fk, difficulty, distance, total_time, rating, created) VALUES (35935, '2024-06-27', 'Tour 8 - Log 1', 8, 4, 3, 104, 4, '2024-06-26 20:21:32.872005');
