-- Add Status column to ServiceOrderItems table
ALTER TABLE ServiceOrderItems ADD COLUMN Status TEXT NOT NULL DEFAULT 'Pending';

-- Update existing records to have 'Pending' status
UPDATE ServiceOrderItems SET Status = 'Pending' WHERE Status IS NULL OR Status = '';
