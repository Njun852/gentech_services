using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;
using System;

namespace gentech_services
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Run database migration on startup
            MigrateDatabase();
        }

        private void MigrateDatabase()
        {
            try
            {
                using var connection = new SqliteConnection("Data Source=gentech.db");
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "ALTER TABLE ServiceOrderItems ADD COLUMN Status TEXT NOT NULL DEFAULT 'Pending';";

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex) when (ex.Message.Contains("duplicate column"))
                {
                    // Column already exists, that's fine
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database migration error: {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

}
