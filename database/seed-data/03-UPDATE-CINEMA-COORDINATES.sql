-- Update Cinema Coordinates for TP.HCM locations
-- Date: 2025-11-06
-- Description: Update existing cinema records with actual GPS coordinates
-- Source: Google Maps

-- Cinema 1: Movie 88 - Nguyễn Trãi
-- Location: 123 Nguyễn Trãi, P. Nguyễn Cư Trinh, Quận 1
UPDATE cinemas 
SET 
    latitude = 10.7626,
    longitude = 106.6827
WHERE cinemaid = 1 AND name LIKE '%Nguyễn Trãi%';

-- Cinema 2: Movie 88 - Sư Vạn Hạnh  
-- Location: 10 Sư Vạn Hạnh, P. 12, Quận 10
UPDATE cinemas 
SET 
    latitude = 10.7717,
    longitude = 106.6657
WHERE cinemaid = 2 AND name LIKE '%Sư Vạn Hạnh%';

-- Cinema 3: Movie 88 - Lý Chính Thắng
-- Location: 45 Lý Chính Thắng, P. Võ Thị Sáu, Quận 3
UPDATE cinemas 
SET 
    latitude = 10.7823,
    longitude = 106.6917
WHERE cinemaid = 3 AND name LIKE '%Lý Chính Thắng%';

-- Cinema 4: Movie 88 - Trần Hưng Đạo
-- Location: 89 Trần Hưng Đạo, P. Nguyễn Thái Bình, Quận 1  
UPDATE cinemas 
SET 
    latitude = 10.7688,
    longitude = 106.6954
WHERE cinemaid = 4 AND name LIKE '%Trần Hưng Đạo%';

-- Cinema 5: Movie 88 - Nam Kỳ Khởi Nghĩa
-- Location: 234 Nam Kỳ Khởi Nghĩa, P. 8, Quận 3
UPDATE cinemas 
SET 
    latitude = 10.7809,
    longitude = 106.6905
WHERE cinemaid = 5 AND name LIKE '%Nam Kỳ Khởi Nghĩa%';

-- Verify all cinemas have coordinates
SELECT 
    cinemaid,
    name,
    address,
    city,
    ROUND(latitude::numeric, 6) as lat,
    ROUND(longitude::numeric, 6) as lng,
    CASE 
        WHEN latitude IS NOT NULL AND longitude IS NOT NULL 
        THEN '✅ Ready for map display'
        ELSE '⚠️ Missing coordinates - please update'
    END as status
FROM cinemas
ORDER BY cinemaid;

-- Calculate distance between cinemas (example query for future use)
-- Distance in kilometers using Haversine formula
-- SELECT 
--     c1.name as cinema1,
--     c2.name as cinema2,
--     ROUND(
--         6371 * acos(
--             cos(radians(c1.latitude)) * cos(radians(c2.latitude)) * 
--             cos(radians(c2.longitude) - radians(c1.longitude)) + 
--             sin(radians(c1.latitude)) * sin(radians(c2.latitude))
--         )::numeric, 
--     2) as distance_km
-- FROM cinemas c1, cinemas c2
-- WHERE c1.cinemaid < c2.cinemaid
--   AND c1.latitude IS NOT NULL 
--   AND c2.latitude IS NOT NULL
-- ORDER BY distance_km;
