-- Script to create test bookings for seat availability testing
-- This will help test the isAvailable functionality in GET /api/auditoriums/{id}/seats?showtimeId={id}

-- Prerequisites:
-- 1. Run seed-seats.sql first to create 50 seats
-- 2. Make sure you have valid customerid, showtimeid
-- 3. Update the variables below with your actual IDs

DO $$
DECLARE
    v_customer_id INT := 1; -- Change to your test customer ID
    v_showtime_id INT := 1; -- Change to your test showtime ID
    v_booking_id INT;
    v_seat_price DECIMAL := 100000; -- 100k VND per seat
    v_total_amount DECIMAL;
BEGIN
    -- Booking 1: Book seats A1, A2, A3 (front row - Standard)
    v_total_amount := v_seat_price * 3;
    
    INSERT INTO bookings (customerid, showtimeid, bookingcode, bookingtime, totalamount, status)
    VALUES (v_customer_id, v_showtime_id, 'BK-20251105-0001', NOW(), v_total_amount, 'confirmed')
    RETURNING bookingid INTO v_booking_id;
    
    INSERT INTO bookingseats (bookingid, seatid, showtimeid, seatprice)
    VALUES 
        (v_booking_id, 1, v_showtime_id, v_seat_price),  -- A1
        (v_booking_id, 2, v_showtime_id, v_seat_price),  -- A2
        (v_booking_id, 3, v_showtime_id, v_seat_price);  -- A3
    
    RAISE NOTICE 'Created booking % with seats A1, A2, A3', v_booking_id;

    -- Booking 2: Book seats D2, D3, D4 (middle row - VIP)
    v_total_amount := v_seat_price * 3 * 1.5; -- VIP seats are 1.5x price
    
    INSERT INTO bookings (customerid, showtimeid, bookingcode, bookingtime, totalamount, status)
    VALUES (v_customer_id, v_showtime_id, 'BK-20251105-0002', NOW(), v_total_amount, 'confirmed')
    RETURNING bookingid INTO v_booking_id;
    
    INSERT INTO bookingseats (bookingid, seatid, showtimeid, seatprice)
    VALUES 
        (v_booking_id, 17, v_showtime_id, v_seat_price * 1.5),  -- D2
        (v_booking_id, 18, v_showtime_id, v_seat_price * 1.5),  -- D3
        (v_booking_id, 19, v_showtime_id, v_seat_price * 1.5);  -- D4
    
    RAISE NOTICE 'Created booking % with seats D2, D3, D4', v_booking_id;

    -- Booking 3: Book couple seats H1, H2 (back row - Couple)
    v_total_amount := v_seat_price * 2 * 1.3; -- Couple seats are 1.3x price
    
    INSERT INTO bookings (customerid, showtimeid, bookingcode, bookingtime, totalamount, status)
    VALUES (v_customer_id, v_showtime_id, 'BK-20251105-0003', NOW(), v_total_amount, 'confirmed')
    RETURNING bookingid INTO v_booking_id;
    
    INSERT INTO bookingseats (bookingid, seatid, showtimeid, seatprice)
    VALUES 
        (v_booking_id, 36, v_showtime_id, v_seat_price * 1.3),  -- H1
        (v_booking_id, 37, v_showtime_id, v_seat_price * 1.3);  -- H2
    
    RAISE NOTICE 'Created booking % with seats H1, H2', v_booking_id;

    -- Booking 4: Cancelled booking (should NOT affect availability)
    INSERT INTO bookings (customerid, showtimeid, bookingcode, bookingtime, totalamount, status)
    VALUES (v_customer_id, v_showtime_id, 'BK-20251105-0004', NOW(), v_seat_price, 'cancelled')
    RETURNING bookingid INTO v_booking_id;
    
    INSERT INTO bookingseats (bookingid, seatid, showtimeid, seatprice)
    VALUES (v_booking_id, 11, v_showtime_id, v_seat_price);  -- C1 (cancelled, should be available)
    
    RAISE NOTICE 'Created cancelled booking % with seat C1 (should be available)', v_booking_id;

END $$;

-- Verify bookings created
SELECT 
    b.bookingid,
    b.bookingcode,
    b.status,
    b.totalamount,
    COUNT(bs.seatid) as seat_count,
    STRING_AGG(CONCAT(s."Row", s."Number"), ', ' ORDER BY s."Row", s."Number") as seats
FROM bookings b
JOIN bookingseats bs ON b.bookingid = bs.bookingid
JOIN seats s ON bs.seatid = s.seatid
WHERE b.showtimeid = 1  -- Change to your showtime ID
GROUP BY b.bookingid, b.bookingcode, b.status, b.totalamount
ORDER BY b.bookingid;

-- Check seat availability summary
SELECT 
    s."Row",
    COUNT(*) as total_seats,
    COUNT(CASE WHEN bs.bookingseatid IS NULL THEN 1 END) as available_seats,
    COUNT(CASE WHEN bs.bookingseatid IS NOT NULL AND b.status != 'cancelled' THEN 1 END) as booked_seats
FROM seats s
LEFT JOIN bookingseats bs ON s.seatid = bs.seatid AND bs.showtimeid = 1  -- Change to your showtime ID
LEFT JOIN bookings b ON bs.bookingid = b.bookingid
WHERE s.auditoriumid = 1  -- Change to your auditorium ID
GROUP BY s."Row"
ORDER BY s."Row";

-- Expected booked seats for showtime 1:
-- A1, A2, A3 (booking 1 - confirmed)
-- D2, D3, D4 (booking 2 - confirmed)
-- H1, H2 (booking 3 - confirmed)
-- C1 should be AVAILABLE (booking 4 - cancelled)
-- Total booked: 8 seats
-- Total available: 42 seats (50 - 8)
