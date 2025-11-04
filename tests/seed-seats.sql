-- Script to seed seats for auditorium testing
-- Creates 50 seats for Auditorium ID 1
-- Layout: 5 rows (A-E) x 10 seats per row
-- Seat types: Standard, VIP

-- IMPORTANT: Column names are case-sensitive in PostgreSQL
-- Schema uses: "Row" and "Number" with capital letters
-- PostgreSQL converts to lowercase without quotes

-- Delete existing seats for auditorium 1 (optional - uncomment if needed)
-- DELETE FROM bookingseats WHERE seatid IN (SELECT seatid FROM seats WHERE auditoriumid = 1);
-- DELETE FROM seats WHERE auditoriumid = 1;

-- Insert seats for Auditorium 1
-- Note: Row is CHAR(2) type, so 'A' becomes 'A ' (with trailing space)
-- Row A: All Standard
INSERT INTO Seats (AuditoriumId, "Row", "Number", Type) VALUES
(1, 'A', 1, 'Standard'),
(1, 'A', 2, 'Standard'),
(1, 'A', 3, 'Standard'),
(1, 'A', 4, 'Standard'),
(1, 'A', 5, 'Standard'),
(1, 'A', 6, 'Standard'),
(1, 'A', 7, 'Standard'),
(1, 'A', 8, 'Standard'),
(1, 'A', 9, 'Standard'),
(1, 'A', 10, 'Standard');

-- Row B: All Standard
INSERT INTO Seats (AuditoriumId, "Row", "Number", Type) VALUES
(1, 'B', 1, 'Standard'),
(1, 'B', 2, 'Standard'),
(1, 'B', 3, 'Standard'),
(1, 'B', 4, 'Standard'),
(1, 'B', 5, 'Standard'),
(1, 'B', 6, 'Standard'),
(1, 'B', 7, 'Standard'),
(1, 'B', 8, 'Standard'),
(1, 'B', 9, 'Standard'),
(1, 'B', 10, 'Standard');

-- Row C: All Standard
INSERT INTO Seats (AuditoriumId, "Row", "Number", Type) VALUES
(1, 'C', 1, 'Standard'),
(1, 'C', 2, 'Standard'),
(1, 'C', 3, 'Standard'),
(1, 'C', 4, 'Standard'),
(1, 'C', 5, 'Standard'),
(1, 'C', 6, 'Standard'),
(1, 'C', 7, 'Standard'),
(1, 'C', 8, 'Standard'),
(1, 'C', 9, 'Standard'),
(1, 'C', 10, 'Standard');

-- Row D: Mix Standard and VIP (VIP in middle)
INSERT INTO Seats (AuditoriumId, "Row", "Number", Type) VALUES
(1, 'D', 1, 'Standard'),
(1, 'D', 2, 'Standard'),
(1, 'D', 3, 'VIP'),
(1, 'D', 4, 'VIP'),
(1, 'D', 5, 'VIP'),
(1, 'D', 6, 'VIP'),
(1, 'D', 7, 'VIP'),
(1, 'D', 8, 'VIP'),
(1, 'D', 9, 'Standard'),
(1, 'D', 10, 'Standard');

-- Row E: All VIP (Premium row)
INSERT INTO Seats (AuditoriumId, "Row", "Number", Type) VALUES
(1, 'E', 1, 'VIP'),
(1, 'E', 2, 'VIP'),
(1, 'E', 3, 'VIP'),
(1, 'E', 4, 'VIP'),
(1, 'E', 5, 'VIP'),
(1, 'E', 6, 'VIP'),
(1, 'E', 7, 'VIP'),
(1, 'E', 8, 'VIP'),
(1, 'E', 9, 'VIP'),
(1, 'E', 10, 'VIP');



-- Verify seat count
SELECT 
    AuditoriumId,
    COUNT(*) as total_seats,
    COUNT(CASE WHEN Type = 'Standard' THEN 1 END) as standard_seats,
    COUNT(CASE WHEN Type = 'VIP' THEN 1 END) as vip_seats
FROM Seats 
WHERE AuditoriumId = 1
GROUP BY AuditoriumId;

-- Expected result:
-- auditoriumid: 1
-- total_seats: 50
-- standard_seats: 34 (rows A, B, C: 30 + D sides: 4)
-- vip_seats: 16 (row D center: 6 + row E: 10)
