using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using app.Models;

namespace app
{
    public partial class OrderHistoryPage : ContentPage
    {
        private SQLiteConnection _database;

        public OrderHistoryPage()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadOrderHistory();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadOrderHistory();
        }

        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Order>();
            _database.CreateTable<OrderItem>();
        }

        private void LoadOrderHistory(string statusFilter = "All")
        {
            var orders = _database.Table<Order>().ToList();
            if (statusFilter != "All")
                orders = orders.Where(o => o.Status == statusFilter).ToList();

            var orderHistory = orders.Select(order => new OrderHistory
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderItems = _database.Table<OrderItem>().Where(oi => oi.OrderId == order.Id).ToList()
            }).ToList();

            OrdersCollectionView.ItemsSource = orderHistory;
        }

        private void OnStatusPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var selectedStatus = picker.SelectedItem?.ToString();
            LoadOrderHistory(selectedStatus);
        }

        private async void OnOrderSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedOrder = e.CurrentSelection.FirstOrDefault() as OrderHistory;
            if (selectedOrder != null)
                await Navigation.PushAsync(new OrderDetailPage(selectedOrder.Id));
        }

        private async void OnRemoveOrderClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var orderToRemove = button?.CommandParameter as OrderHistory;
            if (orderToRemove != null)
            {
                bool confirm = await DisplayAlert("Potvrda brisanja", $"Da li ste sigurni?", "Da", "Ne");
                if (confirm)
                {
                    var items = _database.Table<OrderItem>().Where(oi => oi.OrderId == orderToRemove.Id).ToList();
                    foreach (var item in items)
                        _database.Delete(item);
                    _database.Delete<Order>(orderToRemove.Id);
                    LoadOrderHistory();
                }
            }
        }

        private async void OnUpdateOrderStatusClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var orderToUpdate = button?.CommandParameter as OrderHistory;
            if (orderToUpdate != null)
            {
                var newStatus = await DisplayActionSheet("Ažuriraj status", "Odustani", null,
                    "Narudžba je u procesu pakovanja", "Narudžba je poslana", "Narudžba je dostavljena",
                    "Narudžba je otkazana", "Narudžba je preuzeta");

                if (!string.IsNullOrEmpty(newStatus) && newStatus != "Odustani")
                {
                    var order = _database.Table<Order>().FirstOrDefault(o => o.Id == orderToUpdate.Id);
                    if (order != null)
                    {
                        order.Status = newStatus;
                        _database.Update(order);
                        LoadOrderHistory();
                    }
                }
            }
        }

        private async void OnMarkOrderAsArrivedClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var orderToMark = button?.CommandParameter as OrderHistory;
            if (orderToMark != null)
            {
                var order = _database.Table<Order>().FirstOrDefault(o => o.Id == orderToMark.Id);
                if (order != null)
                {
                    order.Status = "Dostavljena";
                    _database.Update(order);
                    LoadOrderHistory();
                }
            }
        }
    }

// Klasa koja predstavlja istoriju narudžbi
public class OrderHistory
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}