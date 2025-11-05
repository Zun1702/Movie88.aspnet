-- ============================================
-- SEED BOOKINGS DATA
-- ============================================
-- This script creates 20 test bookings with:
-- - Customer IDs: 1, 2, 5, 6, 7, 11, 22, 28, 29
-- - Payment Method ID: 4
-- - All booking statuses: Pending, CheckedIn, Confirmed, Cancelled, Completed
-- - Each booking has 2-4 seats
-- - Realistic booking patterns for past and future showtimes
-- ============================================

-- Step 1: Insert Bookings
INSERT INTO bookings (bookingid, customerid, showtimeid, voucherid, bookingcode, bookingtime, totalamount, status, checkedintime, checkedinby)
VALUES
    -- Completed bookings (past showtimes, checked in)
    (1, 1, 1, NULL, 'BK001', NOW() - INTERVAL '3 days', 160000, 'Completed', NOW() - INTERVAL '3 days' + INTERVAL '2 hours', NULL),
    (2, 2, 2, NULL, 'BK002', NOW() - INTERVAL '3 days', 240000, 'Completed', NOW() - INTERVAL '3 days' + INTERVAL '2 hours', NULL),
    (3, 5, 5, NULL, 'BK003', NOW() - INTERVAL '2 days', 240000, 'Completed', NOW() - INTERVAL '2 days' + INTERVAL '3 hours', NULL),
    (4, 6, 8, NULL, 'BK004', NOW() - INTERVAL '2 days', 320000, 'Completed', NOW() - INTERVAL '2 days' + INTERVAL '4 hours', NULL),
    
    -- CheckedIn bookings (today's showtimes, already checked in)
    (5, 7, 15, NULL, 'BK005', NOW() - INTERVAL '2 hours', 160000, 'CheckedIn', NOW() - INTERVAL '30 minutes', NULL),
    (6, 11, 18, NULL, 'BK006', NOW() - INTERVAL '3 hours', 240000, 'CheckedIn', NOW() - INTERVAL '1 hour', NULL),
    (7, 22, 20, NULL, 'BK007', NOW() - INTERVAL '1 hour', 160000, 'CheckedIn', NOW() - INTERVAL '15 minutes', NULL),
    
    -- Confirmed bookings (upcoming showtimes, paid)
    (8, 28, 25, NULL, 'BK008', NOW() - INTERVAL '1 day', 240000, 'Confirmed', NULL, NULL),
    (9, 29, 28, NULL, 'BK009', NOW() - INTERVAL '1 day', 320000, 'Confirmed', NULL, NULL),
    (10, 1, 32, NULL, 'BK010', NOW() - INTERVAL '12 hours', 160000, 'Confirmed', NULL, NULL),
    (11, 2, 35, NULL, 'BK011', NOW() - INTERVAL '6 hours', 240000, 'Confirmed', NULL, NULL),
    (12, 5, 40, NULL, 'BK012', NOW() - INTERVAL '3 hours', 240000, 'Confirmed', NULL, NULL),
    (13, 6, 45, NULL, 'BK013', NOW(), 160000, 'Confirmed', NULL, NULL),
    
    -- Pending bookings (future showtimes, not yet paid)
    (14, 7, 50, NULL, 'BK014', NOW(), 240000, 'Pending', NULL, NULL),
    (15, 11, 55, NULL, 'BK015', NOW(), 320000, 'Pending', NULL, NULL),
    (16, 22, 60, NULL, 'BK016', NOW() - INTERVAL '30 minutes', 160000, 'Pending', NULL, NULL),
    
    -- Cancelled bookings (various reasons)
    (17, 28, 65, NULL, 'BK017', NOW() - INTERVAL '2 days', 240000, 'Cancelled', NULL, NULL),
    (18, 29, 70, NULL, 'BK018', NOW() - INTERVAL '1 day', 160000, 'Cancelled', NULL, NULL),
    (19, 1, 75, NULL, 'BK019', NOW() - INTERVAL '5 hours', 240000, 'Cancelled', NULL, NULL),
    (20, 2, 80, NULL, 'BK020', NOW() - INTERVAL '2 hours', 320000, 'Cancelled', NULL, NULL)
ON CONFLICT (bookingid) DO UPDATE SET
    customerid = EXCLUDED.customerid,
    showtimeid = EXCLUDED.showtimeid,
    bookingcode = EXCLUDED.bookingcode,
    bookingtime = EXCLUDED.bookingtime,
    totalamount = EXCLUDED.totalamount,
    status = EXCLUDED.status,
    checkedintime = EXCLUDED.checkedintime;

-- Step 2: Insert Booking Seats (2-4 seats per booking)
INSERT INTO bookingseats (bookingseatid, bookingid, showtimeid, seatid, seatprice)
VALUES
    -- Booking 1: 2 seats (2 × 80,000 = 160,000)
    (1, 1, 1, 1, 80000), (2, 1, 1, 2, 80000),
    
    -- Booking 2: 3 seats (3 × 80,000 = 240,000)
    (3, 2, 2, 11, 80000), (4, 2, 2, 12, 80000), (5, 2, 2, 13, 80000),
    
    -- Booking 3: 3 seats (3 × 80,000 = 240,000)
    (6, 3, 5, 31, 80000), (7, 3, 5, 32, 80000), (8, 3, 5, 33, 80000),
    
    -- Booking 4: 4 seats (4 × 80,000 = 320,000)
    (9, 4, 8, 51, 80000), (10, 4, 8, 52, 80000), (11, 4, 8, 53, 80000), (12, 4, 8, 54, 80000),
    
    -- Booking 5: 2 seats (2 × 80,000 = 160,000)
    (13, 5, 15, 101, 80000), (14, 5, 15, 102, 80000),
    
    -- Booking 6: 3 seats (3 × 80,000 = 240,000)
    (15, 6, 18, 121, 80000), (16, 6, 18, 122, 80000), (17, 6, 18, 123, 80000),
    
    -- Booking 7: 2 seats (2 × 80,000 = 160,000)
    (18, 7, 20, 141, 80000), (19, 7, 20, 142, 80000),
    
    -- Booking 8: 3 seats (3 × 80,000 = 240,000)
    (20, 8, 25, 171, 80000), (21, 8, 25, 172, 80000), (22, 8, 25, 173, 80000),
    
    -- Booking 9: 4 seats (4 × 80,000 = 320,000)
    (23, 9, 28, 191, 80000), (24, 9, 28, 192, 80000), (25, 9, 28, 193, 80000), (26, 9, 28, 194, 80000),
    
    -- Booking 10: 2 seats (2 × 80,000 = 160,000)
    (27, 10, 32, 221, 80000), (28, 10, 32, 222, 80000),
    
    -- Booking 11: 3 seats (3 × 80,000 = 240,000)
    (29, 11, 35, 241, 80000), (30, 11, 35, 242, 80000), (31, 11, 35, 243, 80000),
    
    -- Booking 12: 3 seats (3 × 80,000 = 240,000)
    (32, 12, 40, 271, 80000), (33, 12, 40, 272, 80000), (34, 12, 40, 273, 80000),
    
    -- Booking 13: 2 seats (2 × 80,000 = 160,000)
    (35, 13, 45, 301, 80000), (36, 13, 45, 302, 80000),
    
    -- Booking 14: 3 seats (3 × 80,000 = 240,000)
    (37, 14, 50, 331, 80000), (38, 14, 50, 332, 80000), (39, 14, 50, 333, 80000),
    
    -- Booking 15: 4 seats (4 × 80,000 = 320,000)
    (40, 15, 55, 361, 80000), (41, 15, 55, 362, 80000), (42, 15, 55, 363, 80000), (43, 15, 55, 364, 80000),
    
    -- Booking 16: 2 seats (2 × 80,000 = 160,000)
    (44, 16, 60, 391, 80000), (45, 16, 60, 392, 80000),
    
    -- Booking 17: 3 seats (3 × 80,000 = 240,000)
    (46, 17, 65, 421, 80000), (47, 17, 65, 422, 80000), (48, 17, 65, 423, 80000),
    
    -- Booking 18: 2 seats (2 × 80,000 = 160,000)
    (49, 18, 70, 451, 80000), (50, 18, 70, 452, 80000),
    
    -- Booking 19: 3 seats (3 × 80,000 = 240,000)
    (51, 19, 75, 481, 80000), (52, 19, 75, 482, 80000), (53, 19, 75, 483, 80000),
    
    -- Booking 20: 4 seats (4 × 80,000 = 320,000)
    (54, 20, 80, 511, 80000), (55, 20, 80, 512, 80000), (56, 20, 80, 513, 80000), (57, 20, 80, 514, 80000)
ON CONFLICT (bookingseatid) DO UPDATE SET
    bookingid = EXCLUDED.bookingid,
    showtimeid = EXCLUDED.showtimeid,
    seatid = EXCLUDED.seatid,
    seatprice = EXCLUDED.seatprice;

-- Step 3: Insert Payments (only for Confirmed, CheckedIn, Completed bookings)
INSERT INTO payments (paymentid, bookingid, customerid, methodid, amount, status, transactioncode, paymenttime)
VALUES
    -- Completed bookings (status: Success)
    (1, 1, 1, 4, 160000, 'Success', 'TXN001-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '3 days'),
    (2, 2, 2, 4, 240000, 'Success', 'TXN002-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '3 days'),
    (3, 3, 5, 4, 240000, 'Success', 'TXN003-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '2 days'),
    (4, 4, 6, 4, 320000, 'Success', 'TXN004-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '2 days'),
    
    -- CheckedIn bookings (status: Success)
    (5, 5, 7, 4, 160000, 'Success', 'TXN005-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '2 hours'),
    (6, 6, 11, 4, 240000, 'Success', 'TXN006-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '3 hours'),
    (7, 7, 22, 4, 160000, 'Success', 'TXN007-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '1 hour'),
    
    -- Confirmed bookings (status: Success)
    (8, 8, 28, 4, 240000, 'Success', 'TXN008-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '1 day'),
    (9, 9, 29, 4, 320000, 'Success', 'TXN009-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '1 day'),
    (10, 10, 1, 4, 160000, 'Success', 'TXN010-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '12 hours'),
    (11, 11, 2, 4, 240000, 'Success', 'TXN011-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '6 hours'),
    (12, 12, 5, 4, 240000, 'Success', 'TXN012-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '3 hours'),
    (13, 13, 6, 4, 160000, 'Success', 'TXN013-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW()),
    
    -- Cancelled bookings (status: Refunded)
    (14, 17, 28, 4, 240000, 'Refunded', 'TXN014-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '2 days'),
    (15, 18, 29, 4, 160000, 'Refunded', 'TXN015-' || EXTRACT(EPOCH FROM NOW())::BIGINT, NOW() - INTERVAL '1 day')
ON CONFLICT (paymentid) DO UPDATE SET
    bookingid = EXCLUDED.bookingid,
    customerid = EXCLUDED.customerid,
    methodid = EXCLUDED.methodid,
    amount = EXCLUDED.amount,
    status = EXCLUDED.status,
    transactioncode = EXCLUDED.transactioncode,
    paymenttime = EXCLUDED.paymenttime;

-- Step 4: Reset sequences (if needed)
SELECT setval('bookings_bookingid_seq', (SELECT MAX(bookingid) FROM bookings));
SELECT setval('bookingseats_bookingseatid_seq', (SELECT MAX(bookingseatid) FROM bookingseats));
SELECT setval('payments_paymentid_seq', (SELECT MAX(paymentid) FROM payments));

-- Step 5: Verification
SELECT 
    '=== BOOKINGS BY STATUS ===' AS summary;

SELECT 
    status,
    COUNT(*) AS count
FROM bookings
WHERE bookingid BETWEEN 1 AND 20
GROUP BY status
ORDER BY 
    CASE status
        WHEN 'Pending' THEN 1
        WHEN 'Confirmed' THEN 2
        WHEN 'CheckedIn' THEN 3
        WHEN 'Completed' THEN 4
        WHEN 'Cancelled' THEN 5
    END;

SELECT 
    '=== TOTAL SUMMARY ===' AS summary;

SELECT 
    (SELECT COUNT(*) FROM bookings WHERE bookingid BETWEEN 1 AND 20) AS total_bookings,
    (SELECT COUNT(*) FROM bookingseats WHERE bookingid BETWEEN 1 AND 20) AS total_bookingseats,
    (SELECT COUNT(*) FROM payments WHERE bookingid BETWEEN 1 AND 20) AS total_payments,
    (SELECT SUM(totalamount) FROM bookings WHERE bookingid BETWEEN 1 AND 20) AS total_revenue;

SELECT 
    '=== SAMPLE BOOKINGS ===' AS summary;

SELECT 
    b.bookingid,
    b.bookingcode,
    b.customerid,
    b.status,
    b.totalamount,
    COUNT(bs.bookingseatid) AS seat_count,
    p.status AS payment_status
FROM bookings b
LEFT JOIN bookingseats bs ON b.bookingid = bs.bookingid
LEFT JOIN payments p ON b.bookingid = p.bookingid
WHERE b.bookingid BETWEEN 1 AND 20
GROUP BY b.bookingid, b.bookingcode, b.customerid, b.status, b.totalamount, p.status
ORDER BY b.bookingid
LIMIT 10;
