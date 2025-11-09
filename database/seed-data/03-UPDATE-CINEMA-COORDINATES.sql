-- Update Cinema Coordinates for TP.HCM locations (CORRECTED)
-- Date: 2025-11-07
-- Description: Update existing cinema records with actual GPS coordinates based on real addresses
-- Source: Google Maps

-- Cinema 1: Movie 88 - Nguyễn Du
-- Address: 116 Nguyễn Du, Phường Bến Thành, Quận 1
UPDATE cinemas 
SET 
    latitude = 10.77280000,
    longitude = 106.69570000
WHERE cinemaid = 1;

-- Cinema 2: Movie 88 - Kinh Dương Vương
-- Address: 718bis Đ. Kinh Dương Vương, Phường 13, Quận 6
UPDATE cinemas 
SET 
    latitude = 10.74850000,
    longitude = 106.63450000
WHERE cinemaid = 2;

-- Cinema 3: Movie 88 - Quang Trung
-- Address: 304A Đ. Quang Trung, Phường 11, Gò Vấp
UPDATE cinemas 
SET 
    latitude = 10.83920000,
    longitude = 106.66230000
WHERE cinemaid = 3;

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
