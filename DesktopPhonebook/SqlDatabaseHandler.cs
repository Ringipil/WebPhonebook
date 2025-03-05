using System.Data.SQLite;

namespace task0
{
    public class SqlDatabaseHandler : IDatabaseHandlerBase
    {
        private const string connectionString = "Data Source=people.db;Version=3;";

        public void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS People (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    Email TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS IX_People_Name ON People (Name);";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Person> LoadPeople(string filterByName = "", string filterByContact = "")
        {
            var peopleList = new List<Person>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, PhoneNumber, Email FROM People WHERE 1=1";

                if (!string.IsNullOrEmpty(filterByName))
                {
                    query += " AND Name LIKE @Name";
                }
                if (!string.IsNullOrEmpty(filterByContact))
                {
                    query += " AND (PhoneNumber LIKE @Contact OR Email LIKE @Contact)";
                }

                query += " LIMIT 10000";

                using (var command = new SQLiteCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(filterByName))
                    {
                        command.Parameters.AddWithValue("@Name", "%" + filterByName + "%");
                    }
                    if (!string.IsNullOrEmpty(filterByContact))
                    {
                        command.Parameters.AddWithValue("@Contact", "%" + filterByContact + "%");
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var person = new Person(
                                Convert.ToInt32(reader["Id"]),
                                reader["Name"].ToString(),
                                reader["PhoneNumber"].ToString(),
                                reader["Email"].ToString()
                            );
                            peopleList.Add(person);
                        }
                    }
                }
            }
            return peopleList;
        }

        public void AddPerson(Person person)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO People (Name, PhoneNumber, Email) VALUES (@Name, @PhoneNumber, @Email)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", person.Email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePerson(Person person)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE People SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", person.Email);
                    command.Parameters.AddWithValue("@Id", person.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePerson(int id)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM People WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task GenerateUniqueNames(List<string> firstNames, List<string> middleNames, List<string> lastNames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                DateTime startTime = DateTime.Now;
                int countAdded = 0;
                int countChecked = 0;
                bool stopRequested = false;
                int lastReportedCount = 0;
                int nextUpdateThreshold = 1000;

                foreach (var first in firstNames)
                {
                    foreach (var middle in middleNames)
                    {
                        foreach (var last in lastNames)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                stopRequested = true;
                                afterGeneration(countAdded, (DateTime.Now - startTime).TotalSeconds, stopRequested);
                                return;
                            }

                            string fullName = $"{first} {middle} {last}";
                            countChecked++;

                            string checkQuery = "SELECT COUNT(*) FROM People WHERE Name = @Name";
                            using (var checkCommand = new SQLiteCommand(checkQuery, connection))
                            {
                                checkCommand.Parameters.AddWithValue("@Name", fullName);
                                int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                                if (existingCount == 0)
                                {
                                    string insertQuery = "INSERT INTO People (Name, PhoneNumber, Email) VALUES (@Name, '', '')";
                                    using (var insertCommand = new SQLiteCommand(insertQuery, connection))
                                    {
                                        insertCommand.Parameters.AddWithValue("@Name", fullName);
                                        await insertCommand.ExecuteNonQueryAsync();
                                        countAdded++;
                                    }
                                }
                            }
                            if (countChecked >= nextUpdateThreshold)
                            {
                                updateStatus($"{countAdded} added, {countChecked} checked");
                                nextUpdateThreshold += 1000;
                            }
                        }
                    }
                }
                afterGeneration(countAdded, (DateTime.Now - startTime).TotalSeconds, stopRequested);
            }
        }
    }
}
