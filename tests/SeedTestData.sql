-- ===================================================
-- Movie88 - Seed Test Data Script
-- Purpose: Add more movies and bookings for testing
-- Target: UserId=11, CustomerId=5
-- ===================================================

-- ===================================================
-- 1. INSERT MOVIES (Add 10 more movies for testing)
-- ===================================================

-- Movie 4: The Dark Knight
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'The Dark Knight',
    'When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.',
    152,
    'Christopher Nolan',
    'https://www.youtube.com/watch?v=EXeTwQWrcwY',
    '2008-07-18',
    'https://image.tmdb.org/t/p/original/qJ2tW6WMUDux911r6m7haRef0WH.jpg',
    'USA',
    'PG-13',
    'Action, Crime, Drama',
    CURRENT_TIMESTAMP
);

-- Movie 5: Inception
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Inception',
    'A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.',
    148,
    'Christopher Nolan',
    'https://www.youtube.com/watch?v=YoHD9XEInc0',
    '2010-07-16',
    'https://image.tmdb.org/t/p/original/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg',
    'USA',
    'PG-13',
    'Action, Sci-Fi, Thriller',
    CURRENT_TIMESTAMP
);

-- Movie 6: Interstellar
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Interstellar',
    'A team of explorers travel through a wormhole in space in an attempt to ensure humanity''s survival.',
    169,
    'Christopher Nolan',
    'https://www.youtube.com/watch?v=zSWdZVtXT7E',
    '2014-11-07',
    'https://image.tmdb.org/t/p/original/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg',
    'USA',
    'PG-13',
    'Adventure, Drama, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 7: The Shawshank Redemption
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'The Shawshank Redemption',
    'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.',
    142,
    'Frank Darabont',
    'https://www.youtube.com/watch?v=6hB3S9bIaco',
    '1994-09-23',
    'https://image.tmdb.org/t/p/original/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg',
    'USA',
    'R',
    'Drama',
    CURRENT_TIMESTAMP
);

-- Movie 8: Pulp Fiction
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Pulp Fiction',
    'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.',
    154,
    'Quentin Tarantino',
    'https://www.youtube.com/watch?v=s7EdQ4FqbhY',
    '1994-10-14',
    'https://image.tmdb.org/t/p/original/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg',
    'USA',
    'R',
    'Crime, Drama',
    CURRENT_TIMESTAMP
);

-- Movie 9: The Matrix
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'The Matrix',
    'A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.',
    136,
    'The Wachowskis',
    'https://www.youtube.com/watch?v=vKQi3bBA1y8',
    '1999-03-31',
    'https://image.tmdb.org/t/p/original/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg',
    'USA',
    'R',
    'Action, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 10: Forrest Gump
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Forrest Gump',
    'The presidencies of Kennedy and Johnson, the Vietnam War, and other historical events unfold from the perspective of an Alabama man.',
    142,
    'Robert Zemeckis',
    'https://www.youtube.com/watch?v=bLvqoHBptjg',
    '1994-07-06',
    'https://image.tmdb.org/t/p/original/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg',
    'USA',
    'PG-13',
    'Drama, Romance',
    CURRENT_TIMESTAMP
);

-- Movie 11: Avatar (Now Showing)
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Avatar',
    'A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.',
    162,
    'James Cameron',
    'https://www.youtube.com/watch?v=5PSNL1qE6VY',
    '2024-01-15',
    'https://image.tmdb.org/t/p/original/6EiRUJpuoeQPghrs3YNd6E2-TU.jpg',
    'USA',
    'PG-13',
    'Action, Adventure, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 12: Spider-Man: No Way Home (Now Showing)
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Spider-Man: No Way Home',
    'With Spider-Man''s identity now revealed, Peter asks Doctor Strange for help. When a spell goes wrong, dangerous foes from other worlds start to appear.',
    148,
    'Jon Watts',
    'https://www.youtube.com/watch?v=JfVOs4VSpmA',
    '2024-02-20',
    'https://image.tmdb.org/t/p/original/1g0dhYtq4irTY1GPXvft6k4YLjm.jpg',
    'USA',
    'PG-13',
    'Action, Adventure, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 13: Dune: Part Three (Coming Soon)
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Dune: Part Three',
    'Paul Atreides unites with Chani and the Fremen while seeking revenge against the conspirators who destroyed his family.',
    166,
    'Denis Villeneuve',
    'https://www.youtube.com/watch?v=Way9Dexny3w',
    '2026-03-15',
    'https://image.tmdb.org/t/p/original/d5NXSklXo0qyIYkgV94XAgMIckC.jpg',
    'USA',
    'PG-13',
    'Action, Adventure, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 14: Avengers: Secret Wars (Coming Soon)
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Avengers: Secret Wars',
    'The Avengers and their allies must be willing to sacrifice all in an attempt to defeat the powerful Thanos.',
    180,
    'Russo Brothers',
    'https://www.youtube.com/watch?v=TcMBFSGVi1c',
    '2027-05-07',
    'https://image.tmdb.org/t/p/original/7WsyChQLEftFiDOVTGkv3hFpyyt.jpg',
    'USA',
    'PG-13',
    'Action, Adventure, Sci-Fi',
    CURRENT_TIMESTAMP
);

-- Movie 15: Oppenheimer
INSERT INTO movies (title, description, durationminutes, director, trailerurl, releasedate, posterurl, country, rating, genre, createdat)
VALUES (
    'Oppenheimer',
    'The story of American scientist J. Robert Oppenheimer and his role in the development of the atomic bomb.',
    180,
    'Christopher Nolan',
    'https://www.youtube.com/watch?v=uYPbbksJxIg',
    '2023-07-21',
    'https://image.tmdb.org/t/p/original/8Gxv8gSFCU0XGDykEGv7zR1n2ua.jpg',
    'USA',
    'R',
    'Biography, Drama, History',
    CURRENT_TIMESTAMP
);

-- ===================================================
-- 2. ADD SHOWTIMES FOR NOW-SHOWING MOVIES
-- ===================================================

-- Note: Showtimes table structure:
-- - auditoriumid (not cinemaid)
-- - movieid, starttime, endtime, price, format, languagetype
-- - No showdate column (use DATE from starttime)

-- Avatar showtimes (Movie 11)
DO $$
DECLARE
    avatar_movie_id INT;
    auditorium1_id INT;
    auditorium2_id INT;
    current_date_val DATE := CURRENT_DATE;
BEGIN
    SELECT movieid INTO avatar_movie_id FROM movies WHERE title = 'Avatar' ORDER BY movieid DESC LIMIT 1;
    
    -- Get auditorium IDs (assuming auditoriums exist)
    SELECT auditoriumid INTO auditorium1_id FROM auditoriums ORDER BY auditoriumid LIMIT 1 OFFSET 0;
    SELECT auditoriumid INTO auditorium2_id FROM auditoriums ORDER BY auditoriumid LIMIT 1 OFFSET 1;
    
    -- Add showtimes for next 7 days, 3 times per day
    FOR i IN 0..6 LOOP
        -- Morning show: 10:00 AM (Auditorium 1)
        INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
        VALUES (
            avatar_movie_id,
            auditorium1_id,
            (current_date_val + i + TIME '10:00:00')::timestamp,
            (current_date_val + i + TIME '12:42:00')::timestamp,
            125000,
            '2D',
            'Subtitled'
        );
        
        -- Afternoon show: 2:30 PM (Auditorium 1)
        INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
        VALUES (
            avatar_movie_id,
            auditorium1_id,
            (current_date_val + i + TIME '14:30:00')::timestamp,
            (current_date_val + i + TIME '17:12:00')::timestamp,
            125000,
            '3D',
            'Subtitled'
        );
        
        -- Evening show: 7:00 PM (Auditorium 2)
        INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
        VALUES (
            avatar_movie_id,
            auditorium2_id,
            (current_date_val + i + TIME '19:00:00')::timestamp,
            (current_date_val + i + TIME '21:42:00')::timestamp,
            150000,
            'IMAX 3D',
            'Subtitled'
        );
    END LOOP;
END $$;

-- Spider-Man showtimes (Movie 12)
DO $$
DECLARE
    spiderman_movie_id INT;
    auditorium2_id INT;
    auditorium3_id INT;
    current_date_val DATE := CURRENT_DATE;
BEGIN
    SELECT movieid INTO spiderman_movie_id FROM movies WHERE title = 'Spider-Man: No Way Home' ORDER BY movieid DESC LIMIT 1;
    
    -- Get auditorium IDs
    SELECT auditoriumid INTO auditorium2_id FROM auditoriums ORDER BY auditoriumid LIMIT 1 OFFSET 1;
    SELECT auditoriumid INTO auditorium3_id FROM auditoriums ORDER BY auditoriumid LIMIT 1 OFFSET 2;
    
    FOR i IN 0..6 LOOP
        -- Morning show (Auditorium 2)
        INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
        VALUES (
            spiderman_movie_id,
            auditorium2_id,
            (current_date_val + i + TIME '11:00:00')::timestamp,
            (current_date_val + i + TIME '13:28:00')::timestamp,
            125000,
            '2D',
            'Dubbed'
        );
        
        -- Evening show (Auditorium 3)
        INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
        VALUES (
            spiderman_movie_id,
            auditorium3_id,
            (current_date_val + i + TIME '20:00:00')::timestamp,
            (current_date_val + i + TIME '22:28:00')::timestamp,
            125000,
            '2D',
            'Subtitled'
        );
    END LOOP;
END $$;

-- ===================================================
-- 3. CREATE BOOKINGS FOR USERID=11, CUSTOMERID=5
-- ===================================================

-- Note: Actual schema columns:
-- bookings: customerid, showtimeid, voucherid, bookingcode, bookingtime, totalamount, status
-- bookingseats: bookingid, showtimeid, seatid, seatprice (NOT price!)
-- bookingcombos: bookingid, comboid, quantity, comboprice (NOT totalprice!)
-- payments: bookingid, customerid, methodid, amount, status, transactioncode, paymenttime

-- Booking 1: Avatar - Completed
DO $$
DECLARE
    v_movieid INT;
    v_showtimeid INT;
    v_booking_id INT;
    v_seatid1 INT;
    v_seatid2 INT;
    v_methodid INT;
BEGIN
    -- Get Avatar movie and showtime
    SELECT movieid INTO v_movieid FROM movies WHERE title = 'Avatar' ORDER BY movieid DESC LIMIT 1;
    SELECT showtimeid INTO v_showtimeid FROM showtimes WHERE movieid = v_movieid ORDER BY showtimeid LIMIT 1;
    
    -- Get some seat IDs
    SELECT seatid INTO v_seatid1 FROM seats ORDER BY seatid LIMIT 1 OFFSET 0;
    SELECT seatid INTO v_seatid2 FROM seats ORDER BY seatid LIMIT 1 OFFSET 1;
    
    -- Get payment method ID (assume first one exists)
    SELECT methodid INTO v_methodid FROM paymentmethods ORDER BY methodid LIMIT 1;
    
    -- Insert booking
    INSERT INTO bookings (customerid, showtimeid, bookingtime, totalamount, status, bookingcode)
    VALUES (
        5, -- customerid
        v_showtimeid,
        CURRENT_TIMESTAMP - INTERVAL '2 days',
        250000, -- 2 tickets * 125000
        'completed',
        'BK' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDD') || '001'
    )
    RETURNING bookingid INTO v_booking_id;
    
    -- Insert booking seats
    IF v_seatid1 IS NOT NULL AND v_seatid2 IS NOT NULL AND v_showtimeid IS NOT NULL THEN
        INSERT INTO bookingseats (bookingid, showtimeid, seatid, seatprice)
        VALUES 
            (v_booking_id, v_showtimeid, v_seatid1, 125000),
            (v_booking_id, v_showtimeid, v_seatid2, 125000);
    END IF;
    
    -- Insert payment
    IF v_methodid IS NOT NULL THEN
        INSERT INTO payments (bookingid, customerid, methodid, transactioncode, paymenttime, status, amount)
        VALUES (
            v_booking_id,
            5,
            v_methodid,
            'VNP' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDDHH24MISS') || '001',
            CURRENT_TIMESTAMP - INTERVAL '2 days',
            'completed',
            250000
        );
    END IF;
END $$;

-- Booking 2: Spider-Man - Confirmed with Combo
DO $$
DECLARE
    v_movieid INT;
    v_showtimeid INT;
    v_booking_id INT;
    v_comboid INT;
    v_seatid1 INT;
    v_seatid2 INT;
    v_methodid INT;
BEGIN
    SELECT movieid INTO v_movieid FROM movies WHERE title = 'Spider-Man: No Way Home' ORDER BY movieid DESC LIMIT 1;
    SELECT showtimeid INTO v_showtimeid FROM showtimes WHERE movieid = v_movieid ORDER BY showtimeid LIMIT 1;
    SELECT comboid INTO v_comboid FROM combos ORDER BY comboid LIMIT 1;
    SELECT seatid INTO v_seatid1 FROM seats ORDER BY seatid LIMIT 1 OFFSET 4;
    SELECT seatid INTO v_seatid2 FROM seats ORDER BY seatid LIMIT 1 OFFSET 5;
    SELECT methodid INTO v_methodid FROM paymentmethods ORDER BY methodid LIMIT 1;
    
    INSERT INTO bookings (customerid, showtimeid, bookingtime, totalamount, status, bookingcode)
    VALUES (
        5,
        v_showtimeid,
        CURRENT_TIMESTAMP - INTERVAL '1 day',
        450000,
        'confirmed',
        'BK' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDD') || '002'
    )
    RETURNING bookingid INTO v_booking_id;
    
    IF v_seatid1 IS NOT NULL AND v_seatid2 IS NOT NULL AND v_showtimeid IS NOT NULL THEN
        INSERT INTO bookingseats (bookingid, showtimeid, seatid, seatprice)
        VALUES 
            (v_booking_id, v_showtimeid, v_seatid1, 125000),
            (v_booking_id, v_showtimeid, v_seatid2, 125000);
    END IF;
    
    IF v_comboid IS NOT NULL THEN
        INSERT INTO bookingcombos (bookingid, comboid, quantity, comboprice)
        VALUES (v_booking_id, v_comboid, 2, 200000);
    END IF;
    
    IF v_methodid IS NOT NULL THEN
        INSERT INTO payments (bookingid, customerid, methodid, transactioncode, paymenttime, status, amount)
        VALUES (
            v_booking_id,
            5,
            v_methodid,
            'MOMO' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDDHH24MISS') || '002',
            CURRENT_TIMESTAMP - INTERVAL '1 day',
            'completed',
            450000
        );
    END IF;
END $$;

-- Booking 3: Avatar - Pending (Future booking)
DO $$
DECLARE
    v_movieid INT;
    v_showtimeid INT;
    v_booking_id INT;
    v_seatid1 INT;
    v_seatid2 INT;
    v_seatid3 INT;
BEGIN
    SELECT movieid INTO v_movieid FROM movies WHERE title = 'Avatar' ORDER BY movieid DESC LIMIT 1;
    SELECT showtimeid INTO v_showtimeid FROM showtimes 
    WHERE movieid = v_movieid AND starttime > CURRENT_TIMESTAMP
    ORDER BY showtimeid LIMIT 1;
    SELECT seatid INTO v_seatid1 FROM seats ORDER BY seatid LIMIT 1 OFFSET 9;
    SELECT seatid INTO v_seatid2 FROM seats ORDER BY seatid LIMIT 1 OFFSET 10;
    SELECT seatid INTO v_seatid3 FROM seats ORDER BY seatid LIMIT 1 OFFSET 11;
    
    INSERT INTO bookings (customerid, showtimeid, bookingtime, totalamount, status, bookingcode)
    VALUES (
        5,
        v_showtimeid,
        CURRENT_TIMESTAMP,
        375000,
        'pending',
        'BK' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDD') || '003'
    )
    RETURNING bookingid INTO v_booking_id;
    
    IF v_seatid1 IS NOT NULL AND v_seatid2 IS NOT NULL AND v_seatid3 IS NOT NULL AND v_showtimeid IS NOT NULL THEN
        INSERT INTO bookingseats (bookingid, showtimeid, seatid, seatprice)
        VALUES 
            (v_booking_id, v_showtimeid, v_seatid1, 125000),
            (v_booking_id, v_showtimeid, v_seatid2, 125000),
            (v_booking_id, v_showtimeid, v_seatid3, 125000);
    END IF;
END $$;

-- Booking 4: The Dark Knight - Completed (Old movie)
DO $$
DECLARE
    v_movieid INT;
    v_showtimeid INT;
    v_booking_id INT;
    v_auditoriumid INT;
    v_seatid1 INT;
    v_seatid2 INT;
    v_methodid INT;
BEGIN
    SELECT movieid INTO v_movieid FROM movies WHERE title = 'The Dark Knight' ORDER BY movieid DESC LIMIT 1;
    SELECT auditoriumid INTO v_auditoriumid FROM auditoriums ORDER BY auditoriumid LIMIT 1;
    SELECT seatid INTO v_seatid1 FROM seats ORDER BY seatid LIMIT 1 OFFSET 19;
    SELECT seatid INTO v_seatid2 FROM seats ORDER BY seatid LIMIT 1 OFFSET 20;
    SELECT methodid INTO v_methodid FROM paymentmethods ORDER BY methodid LIMIT 1;
    
    -- Create a past showtime
    INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
    VALUES (
        v_movieid,
        v_auditoriumid,
        (CURRENT_DATE - 30 + TIME '19:00:00')::timestamp,
        (CURRENT_DATE - 30 + TIME '21:32:00')::timestamp,
        125000,
        '2D',
        'Subtitled'
    )
    RETURNING showtimeid INTO v_showtimeid;
    
    INSERT INTO bookings (customerid, showtimeid, bookingtime, totalamount, status, bookingcode)
    VALUES (
        5,
        v_showtimeid,
        CURRENT_TIMESTAMP - INTERVAL '30 days',
        250000,
        'completed',
        'BK' || TO_CHAR(CURRENT_TIMESTAMP - INTERVAL '30 days', 'YYYYMMDD') || '004'
    )
    RETURNING bookingid INTO v_booking_id;
    
    IF v_seatid1 IS NOT NULL AND v_seatid2 IS NOT NULL AND v_showtimeid IS NOT NULL THEN
        INSERT INTO bookingseats (bookingid, showtimeid, seatid, seatprice)
        VALUES 
            (v_booking_id, v_showtimeid, v_seatid1, 125000),
            (v_booking_id, v_showtimeid, v_seatid2, 125000);
    END IF;
    
    IF v_methodid IS NOT NULL THEN
        INSERT INTO payments (bookingid, customerid, methodid, transactioncode, paymenttime, status, amount)
        VALUES (
            v_booking_id,
            5,
            v_methodid,
            'CASH' || TO_CHAR(CURRENT_TIMESTAMP - INTERVAL '30 days', 'YYYYMMDDHH24MISS'),
            CURRENT_TIMESTAMP - INTERVAL '30 days',
            'completed',
            250000
        );
    END IF;
END $$;

-- Booking 5: Inception - Cancelled
DO $$
DECLARE
    v_movieid INT;
    v_showtimeid INT;
    v_booking_id INT;
    v_auditoriumid INT;
    v_seatid1 INT;
    v_seatid2 INT;
    v_seatid3 INT;
    v_seatid4 INT;
    v_methodid INT;
BEGIN
    SELECT movieid INTO v_movieid FROM movies WHERE title = 'Inception' ORDER BY movieid DESC LIMIT 1;
    SELECT auditoriumid INTO v_auditoriumid FROM auditoriums ORDER BY auditoriumid LIMIT 1 OFFSET 1;
    SELECT seatid INTO v_seatid1 FROM seats ORDER BY seatid LIMIT 1 OFFSET 29;
    SELECT seatid INTO v_seatid2 FROM seats ORDER BY seatid LIMIT 1 OFFSET 30;
    SELECT seatid INTO v_seatid3 FROM seats ORDER BY seatid LIMIT 1 OFFSET 31;
    SELECT seatid INTO v_seatid4 FROM seats ORDER BY seatid LIMIT 1 OFFSET 32;
    SELECT methodid INTO v_methodid FROM paymentmethods ORDER BY methodid LIMIT 1;
    
    -- Create a past showtime
    INSERT INTO showtimes (movieid, auditoriumid, starttime, endtime, price, format, languagetype)
    VALUES (
        v_movieid,
        v_auditoriumid,
        (CURRENT_DATE - 15 + TIME '20:00:00')::timestamp,
        (CURRENT_DATE - 15 + TIME '22:28:00')::timestamp,
        125000,
        '2D',
        'Subtitled'
    )
    RETURNING showtimeid INTO v_showtimeid;
    
    INSERT INTO bookings (customerid, showtimeid, bookingtime, totalamount, status, bookingcode)
    VALUES (
        5,
        v_showtimeid,
        CURRENT_TIMESTAMP - INTERVAL '15 days',
        500000,
        'cancelled',
        'BK' || TO_CHAR(CURRENT_TIMESTAMP - INTERVAL '15 days', 'YYYYMMDD') || '005'
    )
    RETURNING bookingid INTO v_booking_id;
    
    IF v_seatid1 IS NOT NULL AND v_seatid2 IS NOT NULL AND v_seatid3 IS NOT NULL AND v_seatid4 IS NOT NULL AND v_showtimeid IS NOT NULL THEN
        INSERT INTO bookingseats (bookingid, showtimeid, seatid, seatprice)
        VALUES 
            (v_booking_id, v_showtimeid, v_seatid1, 125000),
            (v_booking_id, v_showtimeid, v_seatid2, 125000),
            (v_booking_id, v_showtimeid, v_seatid3, 125000),
            (v_booking_id, v_showtimeid, v_seatid4, 125000);
    END IF;
    
    IF v_methodid IS NOT NULL THEN
        INSERT INTO payments (bookingid, customerid, methodid, transactioncode, paymenttime, status, amount)
        VALUES (
            v_booking_id,
            5,
            v_methodid,
            'VNP' || TO_CHAR(CURRENT_TIMESTAMP - INTERVAL '15 days', 'YYYYMMDDHH24MISS') || '005',
            CURRENT_TIMESTAMP - INTERVAL '15 days',
            'refunded',
            500000
        );
    END IF;
END $$;

-- ===================================================
-- VERIFICATION QUERIES
-- ===================================================

-- Check inserted movies
SELECT movieid, title, releasedate, genre, rating 
FROM movies 
ORDER BY movieid;

-- Check showtimes for now-showing movies
SELECT m.title, s.starttime, s.endtime, a.name as auditorium_name, c.name as cinema_name
FROM showtimes s
JOIN movies m ON s.movieid = m.movieid
JOIN auditoriums a ON s.auditoriumid = a.auditoriumid
JOIN cinemas c ON a.cinemaid = c.cinemaid
WHERE s.starttime >= CURRENT_TIMESTAMP
ORDER BY s.starttime;

-- Check bookings for customerid=5
SELECT 
    b.bookingid,
    b.bookingcode,
    m.title as movie_title,
    s.starttime,
    b.totalamount,
    b.status,
    pm.name as payment_method,
    p.status as payment_status
FROM bookings b
JOIN showtimes s ON b.showtimeid = s.showtimeid
JOIN movies m ON s.movieid = m.movieid
LEFT JOIN payments p ON b.bookingid = p.bookingid
LEFT JOIN paymentmethods pm ON p.methodid = pm.methodid
WHERE b.customerid = 5
ORDER BY b.bookingtime DESC;

-- Summary
SELECT 
    'Total Movies' as metric, 
    COUNT(*)::text as value 
FROM movies
UNION ALL
SELECT 
    'Active Showtimes', 
    COUNT(*)::text 
FROM showtimes 
WHERE starttime >= CURRENT_TIMESTAMP
UNION ALL
SELECT 
    'Customer 5 Bookings', 
    COUNT(*)::text 
FROM bookings 
WHERE customerid = 5;
