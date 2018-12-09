-- Based on insertScript.docx by paulhawit
-- Requires clean tables, will surely fail otherwise

USE jetson;

-- Plain Text Password for Tesing: jetson
INSERT INTO user (id, username, password, salt, is_administrator)
VALUES (1, 'laganier', '684C22D69EFEF24F3A1DFA2494BD7375', 'PMtQ6HdU', 1);

-- Plain Text API Key for Testing: IfpAF92lLGjU2PUUe3EicFSM513K0zDMVRi5SFwBhfF 
INSERT INTO jetson.api_key (user_id,api_key,salt) 
VALUES (1,'91C6236CC7EFCE5A419AAD80A673A295','O98Qpv2QUGeRy');

INSERT INTO location (id, user_id, location_name, address_line, city, state, zip)
VALUES  (1, 1, 'DMV Rockland', '120 Rockland Drive', 'Ottawa', 'Ontario', 'K1S5L5'), 
        (2, 1, 'Hospital Carling', '1053 Carling Avenue', 'Ottawa', 'Ontario', 'K1Y4E9');

INSERT INTO room (id, location_id, room_name)
VALUES  (1, 1, 'East Waiting Room'),
        (2, 1, 'West Waiting Room'),
        (3, 1, 'Written Test Room'),
        (4, 1, 'Front Entrance');

INSERT INTO camera (id, camera_name, camera_key, location_id, user_id, room_id)
VALUES  (1, 'East Waiting Room Camera', 'AFRJNILIJHRU', 1, 1, 1), 
        (2, 'West Waiting Room Camera', 'HGTIBNERMESD',1, 1, 2), 
        (3, 'Written Test Room Camera', 'EPOVHTRKMQZU',1, 1, 3), 
        (4, 'Front Entrance Camera', 'ZKYVWKJAQQIZ',1, 1, 4);
