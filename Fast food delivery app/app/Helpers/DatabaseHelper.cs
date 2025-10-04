using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using app.Models;

namespace app.Helpers
{
    public static class DatabaseHelper
    {
        private static SQLiteConnection _database;

        public static SQLiteConnection Database
        {
            get
            {
                if (_database == null)
                {
                    var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db3");
                    Console.WriteLine($"Lokacija baze podataka: {dbPath}");
                    _database = new SQLiteConnection(dbPath);
                    _database.CreateTable<Item>();
                    _database.CreateTable<SpecialOffer>();
                    _database.CreateTable<Category>();
                    _database.CreateTable<User>();
                    _database.CreateTable<CartItem>();
                    _database.CreateTable<Order>();
                    _database.CreateTable<OrderItem>();

                    InitializeAdminUser(); // Inicijalizacija admin korisnika
                }
                return _database;
            }
        }

        //Kategorije

       


        public static void InitializeAdminUser()
        {
            // Provjera da li već postoji admin korisnik
            var adminUser = Database.Table<User>().FirstOrDefault(u => u.Username == "admin@fastfood.com");
            if (adminUser == null)
            {
                Database.Insert(new User
                {
                    Username = "admin@fastfood.com",
                    Password = "admin123",
                    Role = "Admin"
                });
            }
            //Kategorije tabela --test
            

        }

        // Dohvat specijalnih ponuda
        public static List<SpecialOffer> GetSpecialOffers()
        {
            return Database.Table<SpecialOffer>().ToList();
        }

        // Metode za unos, ažuriranje i brisanje podataka za proizvode
        public static int AddFoodItem(Item item) => Database.Insert(item);
        public static int UpdateFoodItem(Item item) => Database.Update(item);
        public static int DeleteFoodItem(int id) => Database.Delete<Item>(id);
        public static Item GetFoodItem(int id) => Database.Table<Item>().FirstOrDefault(f => f.Id == id);
        public static List<Item> GetAllFoodItems() => Database.Table<Item>().ToList();

        // Metode za unos, ažuriranje i brisanje specijalnih ponuda
        public static int AddSpecialOffer(SpecialOffer offer) => Database.Insert(offer);
        public static int UpdateSpecialOffer(SpecialOffer offer) => Database.Update(offer);
        public static int DeleteSpecialOffer(int id) => Database.Delete<SpecialOffer>(id);
        public static SpecialOffer GetSpecialOffer(int id) => Database.Table<SpecialOffer>().FirstOrDefault(o => o.Id == id);
        public static List<SpecialOffer> GetAllSpecialOffers() => Database.Table<SpecialOffer>().ToList();

        // Metode za unos, ažuriranje i brisanje kategorija
        public static int AddCategory(Category category) => Database.Insert(category);
        public static int UpdateCategory(Category category) => Database.Update(category);
        public static int DeleteCategory(int id) => Database.Delete<Category>(id);
        public static Category GetCategory(int id) => Database.Table<Category>().FirstOrDefault(c => c.Id == id);
        public static List<Category> GetAllCategories() => Database.Table<Category>().ToList();

        // Metode za unos, ažuriranje i brisanje korisnika
        public static int AddUser(User user) => Database.Insert(user);
        public static int UpdateUser(User user) => Database.Update(user);
        public static int DeleteUser(int id) => Database.Delete<User>(id);
        public static User GetUser(int id) => Database.Table<User>().FirstOrDefault(u => u.Id == id);
        public static List<User> GetAllUsers() => Database.Table<User>().ToList();

        // Dobijanje putanje do baze
        public static string GetDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db3");
        }
    }
}