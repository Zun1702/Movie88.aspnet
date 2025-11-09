-- Migration: Add Latitude and Longitude to cinemas table
-- Date: 2025-11-07
-- Description: Add GPS coordinates for cinema locations

-- Step 1: Add latitude column
ALTER TABLE public.cinemas 
ADD COLUMN IF NOT EXISTS latitude NUMERIC(10, 8);

-- Step 2: Add longitude column
ALTER TABLE public.cinemas 
ADD COLUMN IF NOT EXISTS longitude NUMERIC(11, 8);

-- Step 3: Add comments for documentation
COMMENT ON COLUMN public.cinemas.latitude IS 'Latitude coordinate (GPS) - Range: -90 to 90, Precision: 8 decimal places (~1.1mm accuracy)';
COMMENT ON COLUMN public.cinemas.longitude IS 'Longitude coordinate (GPS) - Range: -180 to 180, Precision: 8 decimal places (~1.1mm accuracy)';

-- Step 4: Add check constraints for valid coordinate ranges
ALTER TABLE public.cinemas 
ADD CONSTRAINT check_latitude_range 
CHECK (latitude IS NULL OR (latitude >= -90 AND latitude <= 90));

ALTER TABLE public.cinemas 
ADD CONSTRAINT check_longitude_range 
CHECK (longitude IS NULL OR (longitude >= -180 AND longitude <= 180));

-- Verification query
SELECT 
    column_name, 
    data_type, 
    numeric_precision, 
    numeric_scale,
    is_nullable
FROM information_schema.columns 
WHERE table_name = 'cinemas' 
  AND column_name IN ('latitude', 'longitude');
