namespace WebPhonebook.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public Person()
        {
        }

        public Person(int id, string? name, string? phoneNumber, string email)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Name}, {PhoneNumber}, {Email}";
        }
    }
}
