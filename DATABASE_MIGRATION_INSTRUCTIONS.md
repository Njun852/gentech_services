# Database Migration Instructions

## ReturnedQuantity Column Migration

### What Changed
We've added a new `ReturnedQuantity` column to the `ProductOrderItems` table to track how many items have been returned from each product order.

### Automatic Migration
The application will **automatically** add the missing column when you start it. The migration happens in the `AuthenticationService.ApplySchemaMigrations()` method during database initialization.

You should see this message in the console output:
```
Adding ReturnedQuantity column to ProductOrderItems table...
ReturnedQuantity column added successfully.
```

### Manual Migration (If Needed)
If the automatic migration fails, you can manually run the SQL script:

1. Close the application
2. Open the database file: `gentech.db` using a SQLite browser or command line
3. Run the script located at: `Scripts/AddReturnedQuantityColumn.sql`

Or run this SQL command directly:
```sql
ALTER TABLE ProductOrderItems ADD COLUMN ReturnedQuantity INTEGER NOT NULL DEFAULT 0;
```

### Verification
To verify the column was added successfully, run:
```sql
PRAGMA table_info(ProductOrderItems);
```

You should see `ReturnedQuantity` in the list of columns.

### What This Enables
- Track how many items have been returned from each order
- Prevent returning more items than available
- Display returned quantities in order details: "10 (2 returned)"
- Calculate available quantities for returns accurately

### Default Value
All existing records will have `ReturnedQuantity = 0` by default, which is correct for orders that haven't had any returns processed yet.
