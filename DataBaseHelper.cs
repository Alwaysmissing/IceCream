using IceCream.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCream.Data
{
    public class DataBaseHelper
    {
        string dbPath = System.AppDomain.CurrentDomain.BaseDirectory + "IceCreamDb.db";

        public DataBaseHelper()
        {
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            string ConnectionString = $"Data Source={dbPath};";
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS MachineState (
                    Id INTEGER PRIMARY KEY,
                    ChocolateCount INTEGER,
                    StrawberryCount INTEGER,
                    IceCreamCount INTEGER,
                    Remain INTEGER,
                    IsMakingIceCream INTEGER,
                    IsReloading INTEGER,
                    CurrentActivity TEXT
                )";

                using (var command = new SQLiteCommand(createTableQuery, conn))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveState(MachineState state)
        {
            string ConnectionString = $"Data Source={dbPath};";

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertOrUpdateQuery = @"
                INSERT OR REPLACE INTO MachineState (Id, ChocolateCount, StrawberryCount, IceCreamCount, Remain, IsMakingIceCream, IsReloading, CurrentActivity)
                VALUES (1, @ChocolateCount, @StrawberryCount, @IceCreamCount, @Remain, @IsMakingIceCream, @IsReloading, @CurrentActivity)";
                using (var command = new SQLiteCommand(insertOrUpdateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ChocolateCount", state.ChocolateCount);
                    command.Parameters.AddWithValue("@StrawberryCount", state.StrawberryCount);
                    command.Parameters.AddWithValue("@IceCreamCount", state.IceCreamCount);
                    command.Parameters.AddWithValue("@Remain", state.Remain);
                    command.Parameters.AddWithValue("@IsMakingIceCream", state.IsMakingIceCream ? 1 : 0);
                    command.Parameters.AddWithValue("@IsReloading", state.IsReloading ? 1 : 0);
                    command.Parameters.AddWithValue("@CurrentActivity", state.CurrentActivity);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void LoadState(MachineState state)
        {
            string ConnectionString = $"Data Source={dbPath};";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM MachineState WHERE Id = 1";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            state.ChocolateCount = Convert.ToInt32(reader["ChocolateCount"]);
                            state.StrawberryCount = Convert.ToInt32(reader["StrawberryCount"]);
                            state.IceCreamCount = Convert.ToInt32(reader["IceCreamCount"]);
                            state.Remain = Convert.ToInt32(reader["Remain"]);
                            state.IsMakingIceCream = Convert.ToInt32(reader["IsMakingIceCream"]) == 1;
                            state.IsReloading = Convert.ToInt32(reader["IsReloading"]) == 1;
                            state.CurrentActivity = reader["CurrentActivity"].ToString();
                        }
                    }
                }
            }
        }
    }
}
