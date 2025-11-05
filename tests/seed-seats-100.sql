-- Script to seed 100 seats for large auditorium testing
-- Creates 100 seats for Auditorium ID 2 (or change auditoriumid as needed)
-- Layout: 10 rows (A-J) x 10 seats per row
-- Seat types: Standard, VIP, Couple, Deluxe

-- Change @auditorium_id if you want to seed a different auditorium
DO $$
DECLARE
    v_auditorium_id INT := 1; -- Change this to your auditorium ID
    v_rows TEXT[] := ARRAY['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];
    v_row TEXT;
    v_seat_number INT;
    v_seat_type TEXT;
BEGIN
    -- Optional: Delete existing seats first
    -- DELETE FROM seats WHERE auditoriumid = v_auditorium_id;

    -- Loop through rows
    FOREACH v_row IN ARRAY v_rows
    LOOP
        -- Loop through seat numbers 1-10
        FOR v_seat_number IN 1..10 LOOP
            -- Determine seat type based on row and position
            CASE 
                -- Front rows (A-C): Standard
                WHEN v_row IN ('A', 'B', 'C') THEN
                    v_seat_type := 'Standard';
                
                -- Middle rows (D-F): VIP in center (seats 3-8), Standard on sides
                WHEN v_row IN ('D', 'E', 'F') THEN
                    IF v_seat_number BETWEEN 3 AND 8 THEN
                        v_seat_type := 'VIP';
                    ELSE
                        v_seat_type := 'Standard';
                    END IF;
                
                -- Premium row (G): All Deluxe
                WHEN v_row = 'G' THEN
                    v_seat_type := 'Deluxe';
                
                -- Back rows (H-J): Couple seats
                ELSE
                    v_seat_type := 'Couple';
            END CASE;

            -- Insert seat
            INSERT INTO seats (auditoriumid, "Row", "Number", "Type", isavailable)
            VALUES (v_auditorium_id, v_row, v_seat_number, v_seat_type, true);
        END LOOP;
    END LOOP;

    -- Print summary
    RAISE NOTICE 'Successfully inserted 100 seats for auditorium %', v_auditorium_id;
END $$;

-- Verify the inserted seats
SELECT 
    auditoriumid,
    COUNT(*) as total_seats,
    COUNT(CASE WHEN "Type" = 'Standard' THEN 1 END) as standard_seats,
    COUNT(CASE WHEN "Type" = 'VIP' THEN 1 END) as vip_seats,
    COUNT(CASE WHEN "Type" = 'Deluxe' THEN 1 END) as deluxe_seats,
    COUNT(CASE WHEN "Type" = 'Couple' THEN 1 END) as couple_seats
FROM seats 
WHERE auditoriumid = 1  -- Change to match your auditorium ID
GROUP BY auditoriumid;

-- View seat layout by row
SELECT 
    "Row",
    COUNT(*) as seats_per_row,
    STRING_AGG("Type", ', ' ORDER BY "Number") as seat_types
FROM seats 
WHERE auditoriumid = 1  -- Change to match your auditorium ID
GROUP BY "Row"
ORDER BY "Row";

-- Expected result:
-- total_seats: 100
-- standard_seats: 36 (rows A-C: 3 rows x 10 seats + D-F sides: 3 rows x 4 seats)
-- vip_seats: 18 (rows D-F center: 3 rows x 6 seats)
-- deluxe_seats: 10 (row G: 1 row x 10 seats)
-- couple_seats: 30 (rows H-J: 3 rows x 10 seats)
