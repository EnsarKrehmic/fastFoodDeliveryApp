using SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using app.Models;

namespace app
{
    public partial class CartPage : ContentPage
    {
        // Privatna varijabla za SQLite konekciju
        private SQLiteConnection _database;

        // Konstruktor: Inicijalizacija stranice, baze podataka i učitavanje artikala u korpi
        public CartPage()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadCartItems();
        }

        // Metoda koja se poziva kada stranica postane vidljiva
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadCartItems(); // Ponovno učitavanje artikala iz baze
        }

        // Metoda za inicijalizaciju baze podataka
        private void InitializeDatabase()
        {
            // Definisanje puta za bazu podataka
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");

            // Konekcija s bazom i kreiranje tabela ako ne postoje
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<CartItem>();
            _database.CreateTable<Order>();
            _database.CreateTable<OrderItem>();
        }

        // Metoda za učitavanje artikala iz korpe
        private void LoadCartItems()
        {
            // Učitavanje artikala iz baze
            var cartItems = _database.Table<CartItem>().ToList();
            CartItemsCollectionView.ItemsSource = cartItems;

            // Računanje ukupne količine i ukupne cijene
            int totalQuantity = cartItems.Sum(item => item.Quantity);
            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

            // Ažuriranje korisničkog interfejsa
            TotalQuantityLabel.Text = totalQuantity.ToString();
            TotalPriceLabel.Text = totalPrice.ToString("C");
        }

        // Metoda koja se poziva kada korisnik ukloni artikal iz korpe
        private async void OnRemoveItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itemToRemove = button?.CommandParameter as CartItem;

            if (itemToRemove != null)
            {
                // Prikaz poruke za potvrdu uklanjanja artikla
                bool confirm = await DisplayAlert("Potvrda uklanjanja", $"Da li ste sigurni da želite ukloniti {itemToRemove.Name}?", "Da", "Ne");
                if (confirm)
                {
                    _database.Delete(itemToRemove); // Brisanje artikla iz baze
                    LoadCartItems(); // Ažuriranje liste artikala
                    await DisplayAlert("Uspjeh", "Artikal je uspješno uklonjen.", "OK");
                }
            }
        }

        // Metoda za proces checkout-a
        private async void OnCheckoutClicked(object sender, EventArgs e)
        {
            bool confirm = await ShowCartSummaryAsync(); // Prikaz rezimea korpe
            if (confirm)
            {
                await CheckoutAsync(); // Pokretanje procesa checkout-a
            }
        }

        // Metoda za prikaz rezimea korpe
        private async Task<bool> ShowCartSummaryAsync()
        {
            var cartItems = _database.Table<CartItem>().ToList();
            int totalQuantity = cartItems.Sum(item => item.Quantity);
            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

            string message = $"Ukupna količina: {totalQuantity}\nUkupna cijena: {totalPrice:C}\n\nDa li želite nastaviti s narudžbom?";
            bool confirm = await DisplayAlert("Rezime korpe", message, "Da", "Ne");
            return confirm;
        }

        // Metoda za izvršavanje checkout-a
        private async Task CheckoutAsync()
        {
            var cartItems = _database.Table<CartItem>().ToList();
            decimal totalPrice = cartItems.Sum(item => item.Price * item.Quantity);

            // Kreiranje nove narudžbe
            var order = new Order
            {
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice
            };
            _database.Insert(order);

            // Spremanje artikala narudžbe
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ItemId = cartItem.ItemId,
                    Name = cartItem.Name,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price
                };
                _database.Insert(orderItem);
            }

            // Brisanje artikala iz korpe
            _database.DeleteAll<CartItem>();
            LoadCartItems(); // Ažuriranje liste artikala

            // Poruka o uspješno izvršenoj narudžbi
            await DisplayAlert("Narudžba", "Vaša narudžba je uspješno zaprimljena.", "OK");

            // Navigacija na stranicu historije narudžbi
            await Navigation.PushAsync(new OrderHistoryPage());
        }
    }
}