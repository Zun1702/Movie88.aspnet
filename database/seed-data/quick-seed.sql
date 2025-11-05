-- Quick seed script without comments (for fast execution)
-- ⚠️ WARNING: This will DELETE ALL existing cinema-related data first!

-- ============================================
-- 1. DROP EXISTING DATA (in correct order to avoid FK violations)
-- ============================================
DELETE FROM bookingseats;
DELETE FROM bookingcombos;
DELETE FROM bookingpromotions;
DELETE FROM payments;
DELETE FROM bookings;
DELETE FROM showtimes;
DELETE FROM seats;
DELETE FROM auditoriums;
DELETE FROM cinemas;

-- Reset sequences
ALTER SEQUENCE IF EXISTS bookingseats_bookingseatid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS bookingcombos_bookingcomboid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS bookingpromotions_bookingpromotionid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS payments_paymentid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS bookings_bookingid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS showtimes_showtimeid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS seats_seatid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS auditoriums_auditoriumid_seq RESTART WITH 1;
ALTER SEQUENCE IF EXISTS cinemas_cinemaid_seq RESTART WITH 1;

-- ============================================
-- 2. INSERT FRESH DATA
-- ============================================

-- Cinemas
INSERT INTO cinemas (cinemaid, name, address, phone, city, createdat)
VALUES
    (1, 'Movie 88 - Nguyễn Huệ', '1 Nguyễn Huệ, P. Bến Nghé, Quận 1', '02811112222', 'TP.HCM', NOW()),
    (2, 'Movie 88 - Sư Vạn Hạnh', '10 Sư Vạn Hạnh, P. 12, Quận 10', '02833334444', 'TP.HCM', NOW()),
    (3, 'Movie 88 - Võ Văn Ngân', '2 Võ Văn Ngân, P. Linh Chiểu, Thủ Đức', '02855556666', 'TP.HCM', NOW())
ON CONFLICT (cinemaid) DO UPDATE SET
    name = EXCLUDED.name,
    address = EXCLUDED.address,
    phone = EXCLUDED.phone,
    city = EXCLUDED.city;

-- Auditoriums
INSERT INTO auditoriums (auditoriumid, cinemaid, name, seatscount)
VALUES
    (1, 1, 'Phòng 1', 70), (2, 1, 'Phòng 2', 60), (3, 1, 'Phòng 3', 80), (4, 1, 'Phòng 4', 70),
    (5, 2, 'Phòng 1', 70), (6, 2, 'Phòng 2', 60), (7, 2, 'Phòng 3', 80), (8, 2, 'Phòng 4', 70),
    (9, 3, 'Phòng 1', 70), (10, 3, 'Phòng 2', 60), (11, 3, 'Phòng 3', 80), (12, 3, 'Phòng 4', 80), (13, 3, 'Phòng 5', 60)
ON CONFLICT (auditoriumid) DO UPDATE SET
    name = EXCLUDED.name,
    seatscount = EXCLUDED.seatscount;

-- Generate Seats
DO $$
DECLARE
    aud_id INT;
    total_seats INT;
    row_letter TEXT;
    seat_num INT;
    seat_type TEXT;
    seats_per_row INT;
    num_rows INT;
BEGIN
    FOR aud_id IN 1..13 LOOP
        SELECT seatscount INTO total_seats FROM auditoriums WHERE auditoriumid = aud_id;
        
        -- Layout based on seatscount (60-80, divisible by 10)
        IF total_seats = 60 THEN
            num_rows := 6; seats_per_row := 10;  -- 6 rows x 10 seats = 60
        ELSIF total_seats = 70 THEN
            num_rows := 7; seats_per_row := 10;  -- 7 rows x 10 seats = 70
        ELSIF total_seats = 80 THEN
            num_rows := 8; seats_per_row := 10;  -- 8 rows x 10 seats = 80
        ELSE
            num_rows := 6; seats_per_row := 10;  -- default
        END IF;
        
        FOR i IN 1..num_rows LOOP
            row_letter := CHR(64 + i);  -- A, B, C, D, E, F, G, H
            
            -- VIP seats: middle rows (row D for 6-row, row D for 7-row, row D-E for 8-row)
            IF (num_rows = 6 AND row_letter = 'D') OR
               (num_rows = 7 AND row_letter = 'D') OR
               (num_rows = 8 AND row_letter IN ('D', 'E')) THEN
                seat_type := 'VIP';
            ELSE
                seat_type := 'Standard';
            END IF;
            
            FOR seat_num IN 1..seats_per_row LOOP
                INSERT INTO seats (auditoriumid, "Row", "Number", type, isavailable)
                VALUES (aud_id, row_letter, seat_num, seat_type, true)
                ON CONFLICT (auditoriumid, "Row", "Number") DO NOTHING;
            END LOOP;
        END LOOP;
    END LOOP;
END $$;

-- Generate Showtimes
DO $$
DECLARE
    movie_id INT;
    showtimes_per_day INT;
    day_offset INT;
    show_date DATE;
    show_time TIME;
    time_slots TIME[] := ARRAY['09:00', '11:30', '14:00', '16:30', '19:00', '21:30', '23:00'];
    selected_times TIME[];
    aud_id INT;
    format_type TEXT;
    lang_type TEXT;
    price_val DECIMAL(10,2);
    start_dt TIMESTAMP;
    end_dt TIMESTAMP;
    duration_mins INT := 120;
    cinema_auds INT[];
BEGIN
    -- Hot movies: 6 showtimes/day
    FOREACH movie_id IN ARRAY ARRAY[1, 2, 40, 41, 42] LOOP
        showtimes_per_day := 6;
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            cinema_auds := ARRAY[
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 1 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 2 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 3 ORDER BY RANDOM() LIMIT 1)
            ];
            selected_times := (SELECT ARRAY_AGG(elem) FROM (SELECT elem FROM UNNEST(time_slots) AS elem ORDER BY RANDOM() LIMIT showtimes_per_day) sub);
            FOREACH show_time IN ARRAY selected_times LOOP
                IF RANDOM() < 0.7 THEN format_type := '2D'; price_val := 80000; ELSE format_type := '3D'; price_val := 120000; END IF;
                IF RANDOM() < 0.8 THEN lang_type := 'Phụ đề'; ELSE lang_type := 'Lồng tiếng'; END IF;
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                aud_id := cinema_auds[1 + FLOOR(RANDOM() * 3)];
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
    END LOOP;
    
    -- Normal movies: 4 showtimes/day
    FOREACH movie_id IN ARRAY ARRAY[3, 43, 44, 45, 46, 47, 48] LOOP
        showtimes_per_day := 4;
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            cinema_auds := ARRAY[
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 1 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 2 ORDER BY RANDOM() LIMIT 1)
            ];
            selected_times := (SELECT ARRAY_AGG(elem) FROM (SELECT elem FROM UNNEST(time_slots) AS elem ORDER BY RANDOM() LIMIT showtimes_per_day) sub);
            FOREACH show_time IN ARRAY selected_times LOOP
                IF RANDOM() < 0.7 THEN format_type := '2D'; price_val := 80000; ELSE format_type := '3D'; price_val := 120000; END IF;
                IF RANDOM() < 0.8 THEN lang_type := 'Phụ đề'; ELSE lang_type := 'Lồng tiếng'; END IF;
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                aud_id := cinema_auds[1 + FLOOR(RANDOM() * 2)];
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
    END LOOP;
    
    -- Less popular: 2 showtimes/day
    FOREACH movie_id IN ARRAY ARRAY[49, 50, 51] LOOP
        showtimes_per_day := 2;
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            cinema_auds := ARRAY[(SELECT auditoriumid FROM auditoriums ORDER BY RANDOM() LIMIT 1)];
            selected_times := (SELECT ARRAY_AGG(elem) FROM (SELECT elem FROM UNNEST(time_slots) AS elem ORDER BY RANDOM() LIMIT showtimes_per_day) sub);
            FOREACH show_time IN ARRAY selected_times LOOP
                format_type := '2D'; price_val := 80000;
                IF RANDOM() < 0.8 THEN lang_type := 'Phụ đề'; ELSE lang_type := 'Lồng tiếng'; END IF;
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                aud_id := cinema_auds[1];
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
    END LOOP;
END $$;

-- Summary
SELECT 
    (SELECT COUNT(*) FROM cinemas) AS cinemas,
    (SELECT COUNT(*) FROM auditoriums) AS auditoriums,
    (SELECT COUNT(*) FROM seats) AS seats,
    (SELECT COUNT(*) FROM showtimes WHERE starttime >= CURRENT_DATE AND starttime < CURRENT_DATE + INTERVAL '10 days') AS showtimes;
