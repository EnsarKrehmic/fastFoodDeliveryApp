using SQLite;
using System.Collections.Generic;
using System.Linq;
using app.Models;

namespace app
{
    public partial class OrderDetailPage : ContentPage
    {
        private SQLiteConnection _database;
        private int _orderId;

        public OrderDetailPage(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            InitializeDatabase(); // Inicijalizacija baze podataka
            LoadOrderDetails(); // Učitavanje detalja narudžbe
        }

        // Funkcija za inicijalizaciju baze podataka i kreiranje potrebnih tabela
        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Order>(); // Kreiranje tabele narudžbi
            _database.CreateTable<OrderItem>(); // Kreiranje tabele stavki narudžbe
        }

        // Funkcija za učitavanje detalja narudžbe
        private void LoadOrderDetails()
        {
            var order = _database.Table<Order>().FirstOrDefault(o => o.Id == _orderId); // Pretraga narudžbe po ID-u
            if (order != null)
            {
                // Prikazivanje podataka o narudžbi
                OrderDateLabel.Text = $"Datum narudžbe: {order.OrderDate:dd.MM.yyyy}";
                TotalPriceLabel.Text = $"Ukupna cijena: {order.TotalPrice:C}";

                // Učitavanje stavki iz narudžbe
                var orderItems = _database.Table<OrderItem>().Where(oi => oi.OrderId == _orderId).ToList();
                OrderItemsCollectionView.ItemsSource = orderItems; // Postavljanje stavki za prikaz u listu
            }
            else
            {
                // Ako narudžba nije pronađena, prikazujemo poruku
                DisplayAlert("Greška", "Narudžba nije pronađena.", "OK");
            }
        }
    }
}