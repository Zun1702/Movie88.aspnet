-- Quick check data status after seed

-- 1. Check showtimes
SELECT 
    'Showtimes' as table_name,
    COUNT(*) as total_count,
    MIN(showtimeid) as min_id,
    MAX(showtimeid) as max_id
FROM showtimes;

-- 2. Check if showtimeid=1 exists
SELECT 
    s.showtimeid,
    s.movieid,
    s.auditoriumid,
    s.starttime,
    s.price,
    a.name as auditorium_name,
    c.name as cinema_name
FROM showtimes s
JOIN auditoriums a ON s.auditoriumid = a.auditoriumid
JOIN cinemas c ON a.cinemaid = c.cinemaid
WHERE s.showtimeid = 1;

-- 3. Check seats 3, 4
SELECT 
    seatid,
    auditoriumid,
    "Row",
    "Number",
    type,
    isavailable
FROM seats
WHERE seatid IN (3, 4);

-- 4. Check existing bookingseats for showtimeid=1
SELECT 
    bs.bookingseatid,
    bs.bookingid,
    bs.showtimeid,
    bs.seatid,
    s."Row",
    s."Number",
    b.status as booking_status,
    b.customerid
FROM bookingseats bs
JOIN seats s ON bs.seatid = s.seatid
JOIN bookings b ON bs.bookingid = b.bookingid
WHERE bs.showtimeid = 1
ORDER BY s."Row", s."Number";

-- 5. Check bookingseats for seats 3, 4 on any showtime
SELECT 
    bs.bookingseatid,
    bs.bookingid,
    bs.showtimeid,
    bs.seatid,
    s."Row",
    s."Number",
    s.auditoriumid,
    b.status as booking_status
FROM bookingseats bs
JOIN seats s ON bs.seatid = s.seatid
JOIN bookings b ON bs.bookingid = b.bookingid
WHERE bs.seatid IN (3, 4);

-- 6. Count total records
SELECT 
    (SELECT COUNT(*) FROM cinemas) as cinemas,
    (SELECT COUNT(*) FROM auditoriums) as auditoriums,
    (SELECT COUNT(*) FROM seats) as seats,
    (SELECT COUNT(*) FROM showtimes) as showtimes,
    (SELECT COUNT(*) FROM bookings) as bookings,
    (SELECT COUNT(*) FROM bookingseats) as bookingseats;
