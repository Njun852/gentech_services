-- Script to add ReturnedQuantity column to ProductOrderItems table
-- Run this script if you get "No column such as returned quantity" error

-- Add the ReturnedQuantity column with default value 0
ALTER TABLE ProductOrderItems ADD COLUMN ReturnedQuantity INTEGER NOT NULL DEFAULT 0;

-- Verify the column was added
PRAGMA table_info(ProductOrderItems);
