using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.Models
{
    public class Review
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string Username { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
