using SQLite;

namespace app.Models
{
    [Table("Users")]
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique]
        public string Username { get; set; }

        public string Password { get; set; }
        public string Role { get; set; }
    }
}
