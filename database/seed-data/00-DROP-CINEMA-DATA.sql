-- ============================================
-- 🗑️ DROP ALL DATA RELATED TO CINEMAS
-- ============================================
-- Purpose: Clear all data related to cinemas, auditoriums, seats, and showtimes
--          to prepare for fresh seed data
-- Author: System
-- Date: 2025-11-06
-- ============================================
-- ⚠️ WARNING: This will delete ALL bookings, payments, and related data!
-- Only use this in DEVELOPMENT/TEST environment, NOT in production!
-- ============================================

-- Disable foreign key checks temporarily (PostgreSQL doesn't have this, use CASCADE instead)

BEGIN;

-- ============================================
-- 1. DELETE CHILD TABLES (deepest dependencies first)
-- ============================================

-- Step 1: Delete booking-related junction tables
DELETE FROM bookingseats;
RAISE NOTICE '✅ Deleted all bookingseats';

DELETE FROM bookingcombos;
RAISE NOTICE '✅ Deleted all bookingcombos';

DELETE FROM bookingpromotions;
RAISE NOTICE '✅ Deleted all bookingpromotions';

-- Step 2: Delete payments (depends on bookings)
DELETE FROM payments;
RAISE NOTICE '✅ Deleted all payments';

-- Step 3: Delete bookings (depends on showtimes)
DELETE FROM bookings;
RAISE NOTICE '✅ Deleted all bookings';

-- ============================================
-- 2. DELETE CINEMA STRUCTURE (bottom to top)
-- ============================================

-- Step 4: Delete showtimes (depends on auditoriums and movies)
DELETE FROM showtimes;
RAISE NOTICE '✅ Deleted all showtimes';

-- Step 5: Delete seats (depends on auditoriums)
DELETE FROM seats;
RAISE NOTICE '✅ Deleted all seats';

-- Step 6: Delete auditoriums (depends on cinemas)
DELETE FROM auditoriums;
RAISE NOTICE '✅ Deleted all auditoriums';

-- Step 7: Delete cinemas (top level)
DELETE FROM cinemas;
RAISE NOTICE '✅ Deleted all cinemas';

-- ============================================
-- 3. RESET AUTO-INCREMENT SEQUENCES (OPTIONAL)
-- ============================================
-- This resets the ID counters to start from 1 again

ALTER SEQUENCE bookingseats_bookingseatid_seq RESTART WITH 1;
ALTER SEQUENCE bookingcombos_bookingcomboid_seq RESTART WITH 1;
ALTER SEQUENCE bookingpromotions_bookingpromotionid_seq RESTART WITH 1;
ALTER SEQUENCE payments_paymentid_seq RESTART WITH 1;
ALTER SEQUENCE bookings_bookingid_seq RESTART WITH 1;
ALTER SEQUENCE showtimes_showtimeid_seq RESTART WITH 1;
ALTER SEQUENCE seats_seatid_seq RESTART WITH 1;
ALTER SEQUENCE auditoriums_auditoriumid_seq RESTART WITH 1;
ALTER SEQUENCE cinemas_cinemaid_seq RESTART WITH 1;

RAISE NOTICE '✅ Reset all ID sequences to 1';

-- ============================================
-- 4. VERIFICATION
-- ============================================

DO $$
DECLARE
    count_bookingseats INT;
    count_bookings INT;
    count_payments INT;
    count_showtimes INT;
    count_seats INT;
    count_auditoriums INT;
    count_cinemas INT;
BEGIN
    SELECT COUNT(*) INTO count_bookingseats FROM bookingseats;
    SELECT COUNT(*) INTO count_bookings FROM bookings;
    SELECT COUNT(*) INTO count_payments FROM payments;
    SELECT COUNT(*) INTO count_showtimes FROM showtimes;
    SELECT COUNT(*) INTO count_seats FROM seats;
    SELECT COUNT(*) INTO count_auditoriums FROM auditoriums;
    SELECT COUNT(*) INTO count_cinemas FROM cinemas;
    
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE '🗑️  VERIFICATION - ALL TABLES EMPTY';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Bookingseats: %', count_bookingseats;
    RAISE NOTICE 'Bookings: %', count_bookings;
    RAISE NOTICE 'Payments: %', count_payments;
    RAISE NOTICE 'Showtimes: %', count_showtimes;
    RAISE NOTICE 'Seats: %', count_seats;
    RAISE NOTICE 'Auditoriums: %', count_auditoriums;
    RAISE NOTICE 'Cinemas: %', count_cinemas;
    RAISE NOTICE '========================================';
    
    IF count_bookingseats = 0 AND count_bookings = 0 AND count_payments = 0 
       AND count_showtimes = 0 AND count_seats = 0 AND count_auditoriums = 0 
       AND count_cinemas = 0 THEN
        RAISE NOTICE '✅ SUCCESS: All tables cleared successfully!';
        RAISE NOTICE '📝 Next step: Run seed script to insert fresh data';
    ELSE
        RAISE WARNING '⚠️  WARNING: Some tables still have data!';
    END IF;
    RAISE NOTICE '';
END $$;

COMMIT;

-- ============================================
-- 5. SUMMARY
-- ============================================
RAISE NOTICE '';
RAISE NOTICE '========================================';
RAISE NOTICE '🎬 MOVIE88 - DATA CLEANUP COMPLETED';
RAISE NOTICE '========================================';
RAISE NOTICE '✅ All cinema-related data has been deleted';
RAISE NOTICE '✅ All sequences have been reset';
RAISE NOTICE '✅ Database is ready for fresh seed data';
RAISE NOTICE '';
RAISE NOTICE '📌 Next Steps:';
RAISE NOTICE '   1. Run: quick-seed.sql (or full seed script)';
RAISE NOTICE '   2. Verify data with SELECT queries';
RAISE NOTICE '========================================';
RAISE NOTICE '';
