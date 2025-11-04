-- ============================================
-- Migration: Add Check-in Tracking to Bookings
-- Date: 2025-11-05
-- Description: Add checkedintime and checkedinby columns to bookings table
-- ============================================

-- Step 1: Add checkedintime column (nullable timestamp)
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS checkedintime TIMESTAMP WITHOUT TIME ZONE;

-- Step 2: Add checkedinby column (nullable FK to User table)
ALTER TABLE bookings 
ADD COLUMN IF NOT EXISTS checkedinby INTEGER;

-- Step 3: Add foreign key constraint (Note: User table has capital U)
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'fk_bookings_checkedinby_users'
    ) THEN
        ALTER TABLE bookings 
        ADD CONSTRAINT fk_bookings_checkedinby_users 
        FOREIGN KEY (checkedinby) 
        REFERENCES "User"(userid) 
        ON DELETE SET NULL;
    END IF;
END $$;

-- Step 4: Add comments for documentation
COMMENT ON COLUMN bookings.checkedintime IS 'Timestamp when customer checked in at cinema counter';
COMMENT ON COLUMN bookings.checkedinby IS 'Staff user ID who performed the check-in';

-- Step 5: Create indexes for performance (if not exists)
CREATE INDEX IF NOT EXISTS idx_bookings_checkedintime ON bookings(checkedintime);
CREATE INDEX IF NOT EXISTS idx_bookings_checkedinby ON bookings(checkedinby);

-- ============================================
-- Verification Query
-- ============================================
-- Run this to verify the columns were added correctly:
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns 
WHERE table_name = 'bookings' 
AND column_name IN ('checkedintime', 'checkedinby')
ORDER BY column_name;

-- Verify foreign key constraint:
SELECT
    tc.constraint_name, 
    tc.table_name, 
    kcu.column_name, 
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name 
FROM information_schema.table_constraints AS tc 
JOIN information_schema.key_column_usage AS kcu
  ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
  ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY' 
AND tc.table_name = 'bookings'
AND kcu.column_name = 'checkedinby';
