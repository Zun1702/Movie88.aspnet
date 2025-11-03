-- Migration: Add bookingcode column to bookings table
-- Date: 2025-11-03
-- Description: Add bookingcode field for QR code scanning and booking verification

-- Add bookingcode column (nullable first for existing data)
ALTER TABLE public.bookings
ADD COLUMN IF NOT EXISTS bookingcode VARCHAR(20);

-- Create unique index
CREATE UNIQUE INDEX IF NOT EXISTS IX_Bookings_BookingCode 
ON public.bookings(bookingcode);

-- Update existing bookings with generated booking codes
-- Format: BK-YYYYMMDD-XXXX (e.g., BK-20251103-0001)
DO $$
DECLARE
    booking_record RECORD;
    new_code VARCHAR(20);
    counter INT := 1;
BEGIN
    FOR booking_record IN 
        SELECT bookingid, bookingtime 
        FROM public.bookings 
        WHERE bookingcode IS NULL
        ORDER BY bookingid
    LOOP
        -- Generate booking code
        new_code := 'BK-' || 
                    TO_CHAR(COALESCE(booking_record.bookingtime, NOW()), 'YYYYMMDD') || 
                    '-' || 
                    LPAD(counter::TEXT, 4, '0');
        
        -- Update booking
        UPDATE public.bookings 
        SET bookingcode = new_code 
        WHERE bookingid = booking_record.bookingid;
        
        counter := counter + 1;
    END LOOP;
END $$;

-- Now make bookingcode NOT NULL
ALTER TABLE public.bookings
ALTER COLUMN bookingcode SET NOT NULL;

-- Verify changes
SELECT 
    COUNT(*) as total_bookings,
    COUNT(bookingcode) as bookings_with_code,
    COUNT(DISTINCT bookingcode) as unique_codes
FROM public.bookings;

-- Show sample booking codes
SELECT bookingid, bookingcode, bookingtime, status
FROM public.bookings
ORDER BY bookingid
LIMIT 10;
