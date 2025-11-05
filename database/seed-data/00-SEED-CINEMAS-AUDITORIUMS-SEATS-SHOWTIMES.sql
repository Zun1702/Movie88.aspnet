-- ============================================
-- 🎬 MOVIE88 - SEED DATA SCRIPT
-- ============================================
-- Purpose: Insert test data for cinemas, auditoriums, seats, and showtimes
-- Author: System
-- Date: 2025-11-06
-- Movies: Using existing movieids (1,2,3,40-51)
-- ⚠️ WARNING: This will DELETE ALL existing cinema-related data first!
-- ============================================

-- ============================================
-- 0. DROP EXISTING DATA (in correct order)
-- ============================================

DO $$
BEGIN
    RAISE NOTICE '🗑️  Step 0: Deleting existing data...';
    
    -- Delete in correct order to avoid FK violations
    DELETE FROM bookingseats;
    RAISE NOTICE '  ✅ Deleted bookingseats';
    
    DELETE FROM bookingcombos;
    RAISE NOTICE '  ✅ Deleted bookingcombos';
    
    DELETE FROM bookingpromotions;
    RAISE NOTICE '  ✅ Deleted bookingpromotions';
    
    DELETE FROM payments;
    RAISE NOTICE '  ✅ Deleted payments';
    
    DELETE FROM bookings;
    RAISE NOTICE '  ✅ Deleted bookings';
    
    DELETE FROM showtimes;
    RAISE NOTICE '  ✅ Deleted showtimes';
    
    DELETE FROM seats;
    RAISE NOTICE '  ✅ Deleted seats';
    
    DELETE FROM auditoriums;
    RAISE NOTICE '  ✅ Deleted auditoriums';
    
    DELETE FROM cinemas;
    RAISE NOTICE '  ✅ Deleted cinemas';
    
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
    RAISE NOTICE '  ✅ Reset all sequences';
    
    RAISE NOTICE '🗑️  Step 0: Completed!';
    RAISE NOTICE '';
END $$;

-- ============================================
-- 1. CINEMAS (3 cinemas in Ho Chi Minh City)
-- ============================================

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

-- ============================================
-- 2. AUDITORIUMS
-- ============================================
-- Cinema 1: 4 auditoriums (120-180 seats each)
-- Cinema 2: 4 auditoriums (120-180 seats each)
-- Cinema 3: 5 auditoriums (100-200 seats each)

INSERT INTO auditoriums (auditoriumid, cinemaid, name, seatscount)
VALUES
    -- Cinema 1: Movie 88 - Nguyễn Huệ (4 phòng)
    (1, 1, 'Phòng 1', 70),
    (2, 1, 'Phòng 2', 60),
    (3, 1, 'Phòng 3', 80),
    (4, 1, 'Phòng 4', 70),
    
    -- Cinema 2: Movie 88 - Sư Vạn Hạnh (4 phòng)
    (5, 2, 'Phòng 1', 70),
    (6, 2, 'Phòng 2', 60),
    (7, 2, 'Phòng 3', 80),
    (8, 2, 'Phòng 4', 70),
    
    -- Cinema 3: Movie 88 - Võ Văn Ngân (5 phòng)
    (9, 3, 'Phòng 1', 70),
    (10, 3, 'Phòng 2', 60),
    (11, 3, 'Phòng 3', 80),
    (12, 3, 'Phòng 4', 80),
    (13, 3, 'Phòng 5', 60)
ON CONFLICT (auditoriumid) DO UPDATE SET
    name = EXCLUDED.name,
    seatscount = EXCLUDED.seatscount;

-- ============================================
-- 3. SEATS
-- ============================================
-- Layout chuẩn rạp chiếu phim Việt Nam (phòng nhỏ-vừa):
-- - Seatscount: 60-80 (chia hết cho 10)
-- - Rows: 6-8 hàng (A-F cho 60 ghế, A-G cho 70 ghế, A-H cho 80 ghế)
-- - Seats per row: 10 ghế
-- - VIP rows: Hàng giữa (D cho phòng 60-70 ghế, D-E cho phòng 80 ghế)
-- - Standard rows: Các hàng còn lại
-- - All seats isavailable = true (không có ghế hỏng)
-- ============================================

-- Function để generate seats
DO $$
DECLARE
    aud_id INT;
    total_seats INT;
    row_letter TEXT;
    seat_num INT;
    seat_type TEXT;
    seats_per_row INT;
    num_rows INT;
    start_seat_id INT := 1;
BEGIN
    -- Loop through all auditoriums
    FOR aud_id IN 1..13 LOOP
        -- Get seatscount for this auditorium
        SELECT seatscount INTO total_seats FROM auditoriums WHERE auditoriumid = aud_id;
        
        -- Determine layout based on seatscount (60-80, divisible by 10)
        IF total_seats = 60 THEN
            -- 6 rows x 10 seats = 60
            num_rows := 6;
            seats_per_row := 10;
        ELSIF total_seats = 70 THEN
            -- 7 rows x 10 seats = 70
            num_rows := 7;
            seats_per_row := 10;
        ELSIF total_seats = 80 THEN
            -- 8 rows x 10 seats = 80
            num_rows := 8;
            seats_per_row := 10;
        ELSE
            -- Default fallback
            num_rows := 6;
            seats_per_row := 10;
        END IF;
        
        -- Generate seats for this auditorium
        FOR i IN 1..num_rows LOOP
            -- Convert row number to letter (1=A, 2=B, ..., 8=H)
            row_letter := CHR(64 + i);
            
            -- Determine seat type based on auditorium size
            -- 6 rows (A-F): VIP = D (row 4, middle)
            -- 7 rows (A-G): VIP = D (row 4, middle)
            -- 8 rows (A-H): VIP = D, E (rows 4-5, middle)
            IF (num_rows = 6 AND row_letter = 'D') OR
               (num_rows = 7 AND row_letter = 'D') OR
               (num_rows = 8 AND row_letter IN ('D', 'E')) THEN
                seat_type := 'VIP';
            ELSE
                seat_type := 'Standard';
            END IF;
            
            -- Generate seats in this row
            FOR seat_num IN 1..seats_per_row LOOP
                INSERT INTO seats (auditoriumid, "Row", "Number", type, isavailable)
                VALUES (aud_id, row_letter, seat_num, seat_type, true)
                ON CONFLICT (auditoriumid, "Row", "Number") DO NOTHING;
            END LOOP;
        END LOOP;
        
        RAISE NOTICE 'Created seats for auditorium % with % rows x % seats', aud_id, num_rows, seats_per_row;
    END LOOP;
END $$;

-- ============================================
-- 4. SHOWTIMES (10 DAYS)
-- ============================================
-- Strategy:
-- - Hot movies (1, 2, 40, 41, 42): 5-7 showtimes/day
-- - Normal movies (3, 43-48): 3-4 showtimes/day
-- - Less popular (49-51): 1-2 showtimes/day
-- - Time slots: 09:00, 11:30, 14:00, 16:30, 19:00, 21:30, 23:00
-- - Format: 2D (70%), 3D (30%)
-- - Language: Phụ đề (80%), Lồng tiếng (20%)
-- - Price: 2D = 80,000đ, 3D = 120,000đ
-- - Timezone: UTC+7 (Vietnam)
-- ============================================

-- Clear existing showtimes (optional)
-- DELETE FROM showtimes WHERE starttime >= NOW();

-- Function to generate showtimes
DO $$
DECLARE
    movie_id INT;
    movie_popularity TEXT; -- 'hot', 'normal', 'low'
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
    duration_mins INT;
    cinema_auds INT[];
BEGIN
    -- Define movie popularity
    -- Hot movies
    FOREACH movie_id IN ARRAY ARRAY[1, 2, 40, 41, 42] LOOP
        showtimes_per_day := 6; -- 6 showtimes/day for hot movies
        
        -- Generate for 10 days
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            
            -- Select random auditoriums for this movie (spread across cinemas)
            cinema_auds := ARRAY[
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 1 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 2 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 3 ORDER BY RANDOM() LIMIT 1)
            ];
            
            -- Select random time slots
            selected_times := (SELECT ARRAY_AGG(elem) FROM (
                SELECT elem FROM UNNEST(time_slots) AS elem 
                ORDER BY RANDOM() LIMIT showtimes_per_day
            ) sub);
            
            -- Create showtimes
            FOREACH show_time IN ARRAY selected_times LOOP
                -- Random format (70% 2D, 30% 3D)
                IF RANDOM() < 0.7 THEN
                    format_type := '2D';
                    price_val := 80000;
                ELSE
                    format_type := '3D';
                    price_val := 120000;
                END IF;
                
                -- Random language (80% subtitle, 20% dubbed)
                IF RANDOM() < 0.8 THEN
                    lang_type := 'Phụ đề';
                ELSE
                    lang_type := 'Lồng tiếng';
                END IF;
                
                -- Get movie duration (assume 120 mins if not set)
                SELECT COALESCE(durationminutes, 120) INTO duration_mins 
                FROM movies WHERE movieid = movie_id;
                
                -- Calculate start and end times
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                
                -- Select random auditorium from available ones
                aud_id := cinema_auds[1 + FLOOR(RANDOM() * 3)];
                
                -- Insert showtime
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
        
        RAISE NOTICE 'Created % showtimes/day for HOT movie %', showtimes_per_day, movie_id;
    END LOOP;
    
    -- Normal movies
    FOREACH movie_id IN ARRAY ARRAY[3, 43, 44, 45, 46, 47, 48] LOOP
        showtimes_per_day := 4; -- 4 showtimes/day for normal movies
        
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            
            cinema_auds := ARRAY[
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 1 ORDER BY RANDOM() LIMIT 1),
                (SELECT auditoriumid FROM auditoriums WHERE cinemaid = 2 ORDER BY RANDOM() LIMIT 1)
            ];
            
            selected_times := (SELECT ARRAY_AGG(elem) FROM (
                SELECT elem FROM UNNEST(time_slots) AS elem 
                ORDER BY RANDOM() LIMIT showtimes_per_day
            ) sub);
            
            FOREACH show_time IN ARRAY selected_times LOOP
                IF RANDOM() < 0.7 THEN
                    format_type := '2D';
                    price_val := 80000;
                ELSE
                    format_type := '3D';
                    price_val := 120000;
                END IF;
                
                IF RANDOM() < 0.8 THEN
                    lang_type := 'Phụ đề';
                ELSE
                    lang_type := 'Lồng tiếng';
                END IF;
                
                SELECT COALESCE(durationminutes, 120) INTO duration_mins 
                FROM movies WHERE movieid = movie_id;
                
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                
                aud_id := cinema_auds[1 + FLOOR(RANDOM() * 2)];
                
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
        
        RAISE NOTICE 'Created % showtimes/day for NORMAL movie %', showtimes_per_day, movie_id;
    END LOOP;
    
    -- Less popular movies
    FOREACH movie_id IN ARRAY ARRAY[49, 50, 51] LOOP
        showtimes_per_day := 2; -- 2 showtimes/day for less popular
        
        FOR day_offset IN 0..9 LOOP
            show_date := CURRENT_DATE + day_offset;
            
            cinema_auds := ARRAY[
                (SELECT auditoriumid FROM auditoriums ORDER BY RANDOM() LIMIT 1)
            ];
            
            selected_times := (SELECT ARRAY_AGG(elem) FROM (
                SELECT elem FROM UNNEST(time_slots) AS elem 
                ORDER BY RANDOM() LIMIT showtimes_per_day
            ) sub);
            
            FOREACH show_time IN ARRAY selected_times LOOP
                format_type := '2D';
                price_val := 80000;
                
                IF RANDOM() < 0.8 THEN
                    lang_type := 'Phụ đề';
                ELSE
                    lang_type := 'Lồng tiếng';
                END IF;
                
                SELECT COALESCE(durationminutes, 120) INTO duration_mins 
                FROM movies WHERE movieid = movie_id;
                
                start_dt := (show_date::TEXT || ' ' || show_time::TEXT)::TIMESTAMP;
                end_dt := start_dt + (duration_mins || ' minutes')::INTERVAL;
                
                aud_id := cinema_auds[1];
                
                INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
                VALUES (movie_id, aud_id, start_dt, end_dt, price_val, format_type, lang_type);
            END LOOP;
        END LOOP;
        
        RAISE NOTICE 'Created % showtimes/day for LOW movie %', showtimes_per_day, movie_id;
    END LOOP;
END $$;

-- ============================================
-- 5. VERIFICATION QUERIES
-- ============================================

-- Count cinemas
SELECT COUNT(*) AS total_cinemas FROM cinemas;

-- Count auditoriums per cinema
SELECT c.name AS cinema, COUNT(a.auditoriumid) AS auditorium_count
FROM cinemas c
LEFT JOIN auditoriums a ON c.cinemaid = a.cinemaid
GROUP BY c.cinemaid, c.name
ORDER BY c.cinemaid;

-- Count seats per auditorium
SELECT 
    a.auditoriumid,
    c.name AS cinema,
    a.name AS auditorium,
    a.seatscount AS declared_seats,
    COUNT(s.seatid) AS actual_seats,
    COUNT(CASE WHEN s.type = 'Standard' THEN 1 END) AS standard_seats,
    COUNT(CASE WHEN s.type = 'VIP' THEN 1 END) AS vip_seats
FROM auditoriums a
LEFT JOIN cinemas c ON a.cinemaid = c.cinemaid
LEFT JOIN seats s ON a.auditoriumid = s.auditoriumid
GROUP BY a.auditoriumid, c.name, a.name, a.seatscount
ORDER BY a.auditoriumid;

-- Count showtimes per movie (next 10 days)
SELECT 
    m.movieid,
    m.title,
    COUNT(st.showtimeid) AS total_showtimes,
    MIN(st.starttime) AS first_showtime,
    MAX(st.starttime) AS last_showtime,
    COUNT(DISTINCT DATE(st.starttime)) AS days_showing
FROM movies m
LEFT JOIN showtimes st ON m.movieid = st.movieid 
    AND st.starttime >= CURRENT_DATE 
    AND st.starttime < CURRENT_DATE + INTERVAL '10 days'
WHERE m.movieid IN (1,2,3,40,41,42,43,44,45,46,47,48,49,50,51)
GROUP BY m.movieid, m.title
ORDER BY total_showtimes DESC;

-- Check showtimes distribution by date
SELECT 
    DATE(starttime) AS show_date,
    COUNT(*) AS total_showtimes,
    COUNT(DISTINCT movieid) AS movies_showing,
    COUNT(DISTINCT auditoriumid) AS auditoriums_used
FROM showtimes
WHERE starttime >= CURRENT_DATE AND starttime < CURRENT_DATE + INTERVAL '10 days'
GROUP BY DATE(starttime)
ORDER BY show_date;

-- Sample showtimes for today
SELECT 
    c.name AS cinema,
    a.name AS auditorium,
    m.title AS movie,
    st.starttime,
    st.format,
    st.languagetype,
    st.price
FROM showtimes st
JOIN auditoriums a ON st.auditoriumid = a.auditoriumid
JOIN cinemas c ON a.cinemaid = c.cinemaid
JOIN movies m ON st.movieid = m.movieid
WHERE DATE(st.starttime) = CURRENT_DATE
ORDER BY st.starttime, c.name
LIMIT 20;

-- ============================================
-- 6. SUMMARY
-- ============================================

DO $$
DECLARE
    total_cinemas INT;
    total_auditoriums INT;
    total_seats INT;
    total_showtimes INT;
BEGIN
    SELECT COUNT(*) INTO total_cinemas FROM cinemas;
    SELECT COUNT(*) INTO total_auditoriums FROM auditoriums;
    SELECT COUNT(*) INTO total_seats FROM seats;
    SELECT COUNT(*) INTO total_showtimes FROM showtimes 
    WHERE starttime >= CURRENT_DATE AND starttime < CURRENT_DATE + INTERVAL '10 days';
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE '🎬 MOVIE88 SEED DATA SUMMARY';
    RAISE NOTICE '========================================';
    RAISE NOTICE '✅ Cinemas: %', total_cinemas;
    RAISE NOTICE '✅ Auditoriums: %', total_auditoriums;
    RAISE NOTICE '✅ Seats: %', total_seats;
    RAISE NOTICE '✅ Showtimes (next 10 days): %', total_showtimes;
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;
