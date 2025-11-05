-- Test script to verify seat generation logic
-- This creates a small sample without affecting main database

-- Test seat generation for 1 auditorium
DO $$
DECLARE
    row_letter TEXT;
    seat_num INT;
    seat_type TEXT;
BEGIN
    -- Test layout: 10 rows x 12 seats
    FOR i IN 1..10 LOOP
        row_letter := CHR(64 + i);
        
        -- VIP rows: G, H, I
        IF row_letter IN ('G', 'H', 'I') THEN
            seat_type := 'VIP';
        ELSE
            seat_type := 'Standard';
        END IF;
        
        RAISE NOTICE 'Row %: Type = %', row_letter, seat_type;
    END LOOP;
END $$;

-- Expected output:
-- Row A: Type = Standard
-- Row B: Type = Standard
-- Row C: Type = Standard
-- Row D: Type = Standard
-- Row E: Type = Standard
-- Row F: Type = Standard
-- Row G: Type = VIP ⭐
-- Row H: Type = VIP ⭐
-- Row I: Type = VIP ⭐
-- Row J: Type = Standard

-- Summary:
-- Standard: 7 rows (A-F, J)
-- VIP: 3 rows (G, H, I)
-- Ratio: 70% Standard / 30% VIP ✅
