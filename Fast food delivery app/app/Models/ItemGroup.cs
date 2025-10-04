using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace app.Models
{
    public class ItemGroup : List<Item>
    {
        public string Category { get; set; }

        public ItemGroup(string category, List<Item> items) : base(items)
        {
            Category = category;
        }
    }
}
