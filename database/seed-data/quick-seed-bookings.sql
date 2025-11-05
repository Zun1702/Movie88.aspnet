-- Quick seed bookings (20 bookings with all statuses)
-- Customer IDs: 1,2,5,6,7,11,22,28,29 | Payment Method: 4

-- Bookings
INSERT INTO bookings (bookingid, customerid, showtimeid, bookingcode, bookingtime, totalamount, status, checkedintime)
VALUES
    (1,1,1,'BK001',NOW()-INTERVAL '3 days',160000,'Completed',NOW()-INTERVAL '3 days'+INTERVAL '2 hours'),
    (2,2,2,'BK002',NOW()-INTERVAL '3 days',240000,'Completed',NOW()-INTERVAL '3 days'+INTERVAL '2 hours'),
    (3,5,5,'BK003',NOW()-INTERVAL '2 days',240000,'Completed',NOW()-INTERVAL '2 days'+INTERVAL '3 hours'),
    (4,6,8,'BK004',NOW()-INTERVAL '2 days',320000,'Completed',NOW()-INTERVAL '2 days'+INTERVAL '4 hours'),
    (5,7,15,'BK005',NOW()-INTERVAL '2 hours',160000,'CheckedIn',NOW()-INTERVAL '30 minutes'),
    (6,11,18,'BK006',NOW()-INTERVAL '3 hours',240000,'CheckedIn',NOW()-INTERVAL '1 hour'),
    (7,22,20,'BK007',NOW()-INTERVAL '1 hour',160000,'CheckedIn',NOW()-INTERVAL '15 minutes'),
    (8,28,25,'BK008',NOW()-INTERVAL '1 day',240000,'Confirmed',NULL),
    (9,29,28,'BK009',NOW()-INTERVAL '1 day',320000,'Confirmed',NULL),
    (10,1,32,'BK010',NOW()-INTERVAL '12 hours',160000,'Confirmed',NULL),
    (11,2,35,'BK011',NOW()-INTERVAL '6 hours',240000,'Confirmed',NULL),
    (12,5,40,'BK012',NOW()-INTERVAL '3 hours',240000,'Confirmed',NULL),
    (13,6,45,'BK013',NOW(),160000,'Confirmed',NULL),
    (14,7,50,'BK014',NOW(),240000,'Pending',NULL),
    (15,11,55,'BK015',NOW(),320000,'Pending',NULL),
    (16,22,60,'BK016',NOW()-INTERVAL '30 minutes',160000,'Pending',NULL),
    (17,28,65,'BK017',NOW()-INTERVAL '2 days',240000,'Cancelled',NULL),
    (18,29,70,'BK018',NOW()-INTERVAL '1 day',160000,'Cancelled',NULL),
    (19,1,75,'BK019',NOW()-INTERVAL '5 hours',240000,'Cancelled',NULL),
    (20,2,80,'BK020',NOW()-INTERVAL '2 hours',320000,'Cancelled',NULL)
ON CONFLICT (bookingid) DO NOTHING;

-- Booking Seats (2-4 seats per booking)
INSERT INTO bookingseats (bookingseatid, bookingid, showtimeid, seatid, seatprice) VALUES
(1,1,1,1,80000),(2,1,1,2,80000),
(3,2,2,11,80000),(4,2,2,12,80000),(5,2,2,13,80000),
(6,3,5,31,80000),(7,3,5,32,80000),(8,3,5,33,80000),
(9,4,8,51,80000),(10,4,8,52,80000),(11,4,8,53,80000),(12,4,8,54,80000),
(13,5,15,101,80000),(14,5,15,102,80000),
(15,6,18,121,80000),(16,6,18,122,80000),(17,6,18,123,80000),
(18,7,20,141,80000),(19,7,20,142,80000),
(20,8,25,171,80000),(21,8,25,172,80000),(22,8,25,173,80000),
(23,9,28,191,80000),(24,9,28,192,80000),(25,9,28,193,80000),(26,9,28,194,80000),
(27,10,32,221,80000),(28,10,32,222,80000),
(29,11,35,241,80000),(30,11,35,242,80000),(31,11,35,243,80000),
(32,12,40,271,80000),(33,12,40,272,80000),(34,12,40,273,80000),
(35,13,45,301,80000),(36,13,45,302,80000),
(37,14,50,331,80000),(38,14,50,332,80000),(39,14,50,333,80000),
(40,15,55,361,80000),(41,15,55,362,80000),(42,15,55,363,80000),(43,15,55,364,80000),
(44,16,60,391,80000),(45,16,60,392,80000),
(46,17,65,421,80000),(47,17,65,422,80000),(48,17,65,423,80000),
(49,18,70,451,80000),(50,18,70,452,80000),
(51,19,75,481,80000),(52,19,75,482,80000),(53,19,75,483,80000),
(54,20,80,511,80000),(55,20,80,512,80000),(56,20,80,513,80000),(57,20,80,514,80000)
ON CONFLICT (bookingseatid) DO NOTHING;

-- Payments (only for paid bookings)
INSERT INTO payments (paymentid, bookingid, customerid, methodid, amount, status, transactioncode, paymenttime) VALUES
(1,1,1,4,160000,'Success','TXN001-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '3 days'),
(2,2,2,4,240000,'Success','TXN002-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '3 days'),
(3,3,5,4,240000,'Success','TXN003-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '2 days'),
(4,4,6,4,320000,'Success','TXN004-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '2 days'),
(5,5,7,4,160000,'Success','TXN005-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '2 hours'),
(6,6,11,4,240000,'Success','TXN006-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '3 hours'),
(7,7,22,4,160000,'Success','TXN007-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '1 hour'),
(8,8,28,4,240000,'Success','TXN008-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '1 day'),
(9,9,29,4,320000,'Success','TXN009-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '1 day'),
(10,10,1,4,160000,'Success','TXN010-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '12 hours'),
(11,11,2,4,240000,'Success','TXN011-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '6 hours'),
(12,12,5,4,240000,'Success','TXN012-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '3 hours'),
(13,13,6,4,160000,'Success','TXN013-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()),
(14,17,28,4,240000,'Refunded','TXN014-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '2 days'),
(15,18,29,4,160000,'Refunded','TXN015-'||EXTRACT(EPOCH FROM NOW())::BIGINT,NOW()-INTERVAL '1 day')
ON CONFLICT (paymentid) DO NOTHING;

-- Verification
SELECT status, COUNT(*) FROM bookings WHERE bookingid BETWEEN 1 AND 20 GROUP BY status;
