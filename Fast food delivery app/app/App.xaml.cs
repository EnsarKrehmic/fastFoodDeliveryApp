using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using app.Helpers;
using app.Models;

namespace app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Inicijalizacija baze i kreacija potrebnih tabela
            var db = DatabaseHelper.Database;

            // Dodavanje početnih podataka (u slučaju da baza nema podataka)
            SeedData();

            // Postavljanje početne stranice aplikacije
            MainPage = new NavigationPage(new LoginPage());
        }

        // Metoda za dodavanje početnih podataka u bazu
        private void SeedData()
        {
            var foodItems = DatabaseHelper.GetAllFoodItems();
            if (foodItems == null || foodItems.Count == 0)
            {
                var items = new List<Item>
                {
                    new Item { Name = "Pepperoni Pizza", Description = "Delicious pizza with pepperoni", Price = 8.99M, ImageUrl = "pizza.jpg", Category ="lunch" },
                    new Item { Name = "Sushi Deluxe", Description = "Fresh sushi rolls", Price = 15.99M, ImageUrl = "sushi.jpg", Category = "deluxe" },
                    new Item { Name = "Burger Express", Description = "Juicy burger with cheese", Price = 6.99M, ImageUrl = "burger.jpg", Category = "grill"},
                    new Item { Name = "Chicken", Description = "Perfect food for short time!", Price = 5.00M, ImageUrl = "chicken.jpg", Category = "grill"},
                    new Item { Name = "Cake", Description = "Sweet chocolate cake", Price = 3.50M, ImageUrl = "desert1.jpg", Category = "desert"},
                    new Item { Name = "Ice cake", Description = "Cake with delicious ice cream", Price = 5.99M, ImageUrl = "desert.jpg", Category = "desert"},
                    new Item { Name = "Breakfast special", Description = "Fresh made eggs with bacon", Price = 4.99M, ImageUrl = "dorucak.jpg", Category = "lunch"},
                    new Item{ Name = "Pasta deluxe", Description = "Deluxe version of pasta, with cheese!", Price = 8.00M, ImageUrl = "pasta1", Category = "deluxe"}
                };

                foreach (var item in items)
                {
                    DatabaseHelper.AddFoodItem(item);
                }
            }

            // Dodavanje testnih podataka za specijalne ponude
            var specialOffers = DatabaseHelper.GetAllSpecialOffers();
            if (specialOffers == null || specialOffers.Count == 0)
            {
                var offers = new List<SpecialOffer>
                {
                    new SpecialOffer { Name = "Pizza Special", ImageUrl = "pizza1.jpg", DiscountedPrice = 6.99M, OriginalPrice = 8.99M },
                    new SpecialOffer { Name = "Burger Deal", ImageUrl = "burger1.jpg", DiscountedPrice = 5.99M, OriginalPrice = 7.99M }
                };

                foreach (var offer in offers)
                {
                    DatabaseHelper.AddSpecialOffer(offer);
                }
            }

            // Dodavanje testnih podataka za kategorije
            var categories = DatabaseHelper.GetAllCategories();
            if (categories == null || categories.Count == 0)
            {
                var categoryList = new List<Category>
                {
                    new Category { Name = "Lunch", ImageUrl = "dorucak.jpg", Description = "Delicious lunch items" },
                    new Category { Name = "Deluxe", ImageUrl = "chicken.jpg",Description = "Premium quality items" },
                    new Category { Name = "Grill", ImageUrl = "sushi.jpg",Description = "Grilled to perfection" },
                    new Category { Name = "Desert", ImageUrl = "desert1.jpg",Description = "Sweet treats" }
                };

                foreach (var category in categoryList)
                {
                    DatabaseHelper.AddCategory(category);
                }
            }
        }
    }
}