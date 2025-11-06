-- =============================================
-- 02-SEED-PROMOTIONS.sql
-- Purpose: Seed data for promotions table
-- Created: November 6, 2025
-- =============================================

-- Delete existing promotions (if any)
DELETE FROM bookingpromotions;
DELETE FROM promotions;

-- Reset sequence
ALTER SEQUENCE promotions_promotionid_seq RESTART WITH 1;

-- =============================================
-- INSERT PROMOTIONS
-- =============================================

-- 1. Khuyến Mãi Tháng 11 (November 2025)
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'Khuyến Mãi Tháng 11',
    'Giảm 20% cho tất cả vé trong tháng 11/2025',
    '2025-11-01',
    '2025-11-30',
    'Percent',
    20
);

-- 2. Black Friday Cinema
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'Black Friday Cinema',
    'Giảm 30% đặc biệt ngày Black Friday 29/11',
    '2025-11-29',
    '2025-11-29',
    'Percent',
    30
);

-- 3. Opening Week Special
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'Opening Week Special',
    'Giảm 50,000đ cho tuần đầu tháng 11',
    '2025-11-01',
    '2025-11-07',
    'Fixed',
    50000
);

-- 4. Cyber Monday Deal
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'Cyber Monday Deal',
    'Giảm 25% ngày Cyber Monday',
    '2025-12-02',
    '2025-12-02',
    'Percent',
    25
);

-- 5. End of Year Celebration
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'End of Year Celebration',
    'Giảm 15% cho tháng 12',
    '2025-12-01',
    '2025-12-31',
    'Percent',
    15
);

-- 6. Student Special (Example for fixed discount)
INSERT INTO promotions (name, description, startdate, enddate, discounttype, discountvalue)
VALUES (
    'Student Special',
    'Giảm 30,000đ cho sinh viên (cả năm 2025)',
    '2025-01-01',
    '2025-12-31',
    'Fixed',
    30000
);

-- =============================================
-- VERIFY DATA
-- =============================================

-- Check all promotions
SELECT 
    promotionid,
    name,
    description,
    TO_CHAR(startdate, 'DD/MM/YYYY') as startdate,
    TO_CHAR(enddate, 'DD/MM/YYYY') as enddate,
    discounttype,
    discountvalue,
    CASE 
        WHEN CURRENT_DATE BETWEEN startdate AND enddate THEN 'ACTIVE ✅'
        WHEN CURRENT_DATE < startdate THEN 'UPCOMING 📅'
        ELSE 'EXPIRED ❌'
    END as status
FROM promotions
ORDER BY startdate DESC;

-- Show active promotions only
SELECT 
    promotionid,
    name,
    description,
    discounttype,
    discountvalue
FROM promotions
WHERE CURRENT_DATE BETWEEN startdate AND enddate
ORDER BY promotionid;

-- =============================================
-- EXPECTED OUTPUT
-- =============================================
/*
promotionid | name                     | discounttype | discountvalue | status
------------|--------------------------|--------------|---------------|----------
1           | Khuyến Mãi Tháng 11      | percentage   | 20.00         | ACTIVE ✅
2           | Black Friday Cinema      | percentage   | 30.00         | UPCOMING 📅
3           | Opening Week Special     | fixed        | 50000.00      | ACTIVE ✅
4           | Cyber Monday Deal        | percentage   | 25.00         | UPCOMING 📅
5           | End of Year Celebration  | percentage   | 15.00         | UPCOMING 📅
6           | Student Special          | fixed        | 30000.00      | ACTIVE ✅
*/
