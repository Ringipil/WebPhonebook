using Microsoft.Data.SqlClient;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

public class SqlDatabaseHandler : IDatabaseHandler
{
    private const string connectionString = "Server=.\\SQLEXPRESS;Database=PeopleDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false";
    private readonly PeopleDbContext _dbContext;

    public SqlDatabaseHandler(PeopleDbContext dbContext)
    {
        _dbContext = dbContext;
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
//####
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
//####
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
}
