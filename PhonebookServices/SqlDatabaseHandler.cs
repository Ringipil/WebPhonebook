using Microsoft.Data.SqlClient;
using WebPhonebook;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

public class SqlDatabaseHandler : IDatabaseHandler
{
    private const string connectionString = "Server=.\\SQLEXPRESS;Database=PeopleDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false";
    //private readonly PeopleDbContext _dbContext;

    public SqlDatabaseHandler(/*PeopleDbContext dbContext*/)
    {
        //_dbContext = dbContext;
    }

    public void InitializeDatabase()
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string createTableQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='People' AND xtype='U')
            BEGIN
                CREATE TABLE People (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(255) NOT NULL,
                    PhoneNumber NVARCHAR(50) NOT NULL,
                    Email NVARCHAR(255) NOT NULL
                );

                CREATE INDEX IX_People_Name ON People (Name);
            END";

            using (var command = new SqlCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public List<Person> LoadPeople(string filterByName = "", string filterByContact = "")
    {
        var peopleList = new List<Person>();
        using (var connection = new SqlConnection(connectionString))
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

            using (var command = new SqlCommand(query, connection))
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
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO People (Name, PhoneNumber, Email) VALUES (@Name, @PhoneNumber, @Email)";

            using (var command = new SqlCommand(query, connection))
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
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE People SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email WHERE Id = @Id";
            using (var command = new SqlCommand(query, connection))
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
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "DELETE FROM People WHERE Id = @Id";
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }


    public async Task GenerateUniqueNames(List<string> firstNames, List<string> middleNames, List<string> lastNames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration)
    {
        using (var connection = new SqlConnection(connectionString))
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
                        using (var checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@Name", fullName);
                            int existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                            if (existingCount == 0)
                            {
                                string insertQuery = "INSERT INTO People (Name, PhoneNumber, Email) VALUES (@Name, '', '')";
                                using (var insertCommand = new SqlCommand(insertQuery, connection))
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

