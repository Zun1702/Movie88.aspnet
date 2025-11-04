-- =====================================================
-- Migration: Add OTP Verification Feature
-- Created: 2025-11-04
-- Description: Add email verification with OTP support
-- =====================================================

-- Step 1: Add verification columns to User table (note: User not users)
ALTER TABLE public."User"
ADD COLUMN IF NOT EXISTS isverified BOOLEAN NOT NULL DEFAULT FALSE,
ADD COLUMN IF NOT EXISTS isactive BOOLEAN NOT NULL DEFAULT TRUE,
ADD COLUMN IF NOT EXISTS verifiedat TIMESTAMP NULL;

-- Add comments for documentation
COMMENT ON COLUMN public."User".isverified IS 'Email verification status via OTP';
COMMENT ON COLUMN public."User".isactive IS 'User account active status (for ban/suspend)';
COMMENT ON COLUMN public."User".verifiedat IS 'Timestamp when email was verified';

-- Step 2: Create OTP tokens table
CREATE TABLE IF NOT EXISTS public.otp_tokens (
    id SERIAL PRIMARY KEY,
    userid INTEGER NOT NULL,
    otpcode VARCHAR(6) NOT NULL,
    otptype VARCHAR(20) NOT NULL, -- 'EmailVerification', 'PasswordReset', 'Login'
    email VARCHAR(100) NOT NULL,
    createdat TIMESTAMP NOT NULL DEFAULT NOW(),
    expiresat TIMESTAMP NOT NULL,
    isused BOOLEAN NOT NULL DEFAULT FALSE,
    usedat TIMESTAMP NULL,
    ipaddress VARCHAR(45) NULL,
    useragent VARCHAR(500) NULL,
    
    -- Foreign key constraint (references User table)
    CONSTRAINT fk_otp_userid FOREIGN KEY (userid) 
        REFERENCES public."User"(userid) 
        ON DELETE CASCADE,
    
    -- Indexes for performance
    CONSTRAINT idx_otp_code_type UNIQUE (otpcode, otptype, email),
    CONSTRAINT chk_otp_code_length CHECK (LENGTH(otpcode) = 6),
    CONSTRAINT chk_otp_type CHECK (otptype IN ('EmailVerification', 'PasswordReset', 'Login'))
);

-- Add table comment
COMMENT ON TABLE public.otp_tokens IS 'OTP tokens for email verification and password reset';

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_otp_userid ON public.otp_tokens(userid);
CREATE INDEX IF NOT EXISTS idx_otp_email ON public.otp_tokens(email);
CREATE INDEX IF NOT EXISTS idx_otp_code ON public.otp_tokens(otpcode);
CREATE INDEX IF NOT EXISTS idx_otp_createdat ON public.otp_tokens(createdat DESC);
CREATE INDEX IF NOT EXISTS idx_otp_expiresat ON public.otp_tokens(expiresat);

-- Step 3: Update existing users to be verified (migration safety)
-- Set existing users as verified to avoid breaking existing accounts
UPDATE public."User"
SET isverified = TRUE, 
    verifiedat = NOW()
WHERE isverified = FALSE;

-- Step 4: Create function to auto-delete expired OTP tokens (cleanup)
CREATE OR REPLACE FUNCTION public.cleanup_expired_otp_tokens()
RETURNS void AS $$
BEGIN
    DELETE FROM public.otp_tokens
    WHERE expiresat < NOW() - INTERVAL '7 days'; -- Keep for 7 days for audit
END;
$$ LANGUAGE plpgsql;

-- Step 5: Create scheduled job to run cleanup (optional - if Supabase supports pg_cron)
-- You can manually run: SELECT public.cleanup_expired_otp_tokens();
-- Or set up a daily cron job in Supabase dashboard

-- Step 6: Add sample OTP for testing (remove in production)
-- Test user: customer@example.com with OTP: 123456
DO $$
DECLARE
    v_userid INTEGER;
BEGIN
    -- Get customer user id
    SELECT userid INTO v_userid 
    FROM public."User"
    WHERE email = 'customer@example.com' 
    LIMIT 1;
    
    -- Insert test OTP if user exists
    IF v_userid IS NOT NULL THEN
        INSERT INTO public.otp_tokens (
            userid, otpcode, otptype, email, createdat, expiresat, isused
        ) VALUES (
            v_userid, 
            '123456', 
            'EmailVerification', 
            'customer@example.com', 
            NOW(), 
            NOW() + INTERVAL '10 minutes',
            FALSE
        )
        ON CONFLICT (otpcode, otptype, email) DO NOTHING;
        
        RAISE NOTICE 'Test OTP created: 123456 for customer@example.com';
    END IF;
END $$;

-- Step 7: Verification queries for checking migration success
-- Run these to verify the migration worked correctly:

-- Check User table structure
SELECT 
    column_name, 
    data_type, 
    is_nullable, 
    column_default
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'User'
  AND column_name IN ('isverified', 'isactive', 'verifiedat')
ORDER BY ordinal_position;

-- Check otp_tokens table structure
SELECT 
    column_name, 
    data_type, 
    is_nullable
FROM information_schema.columns
WHERE table_schema = 'public' 
  AND table_name = 'otp_tokens'
ORDER BY ordinal_position;

-- Count verified users
SELECT 
    COUNT(*) as total_users,
    COUNT(CASE WHEN isverified = TRUE THEN 1 END) as verified_users,
    COUNT(CASE WHEN isverified = FALSE THEN 1 END) as unverified_users,
    COUNT(CASE WHEN isactive = FALSE THEN 1 END) as inactive_users
FROM public."User";

-- Check OTP tokens
SELECT 
    COUNT(*) as total_tokens,
    COUNT(CASE WHEN isused = TRUE THEN 1 END) as used_tokens,
    COUNT(CASE WHEN expiresat > NOW() THEN 1 END) as active_tokens
FROM public.otp_tokens;

-- =====================================================
-- ROLLBACK SCRIPT (if needed)
-- =====================================================
/*
-- Remove OTP tokens table
DROP TABLE IF EXISTS public.otp_tokens CASCADE;

-- Remove columns from User table
ALTER TABLE public."User"
DROP COLUMN IF EXISTS isverified,
DROP COLUMN IF EXISTS isactive,
DROP COLUMN IF EXISTS verifiedat;

-- Drop cleanup function
DROP FUNCTION IF EXISTS public.cleanup_expired_otp_tokens();
*/

-- =====================================================
-- END OF MIGRATION
-- =====================================================
