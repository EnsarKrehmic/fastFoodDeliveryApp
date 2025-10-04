using SQLite;
using System.IO;
using System.Linq;
using app.Models;

namespace app
{
    public partial class HomePageWithCRUD : ContentPage
    {
        private SQLiteConnection _database;
        private Item _itemBeingEdited;
        private readonly User _currentUser;

        // Konstruktor stranice koji inicijalizuje stranicu sa korisničkom ulogom i trenutnim korisnikom
        public HomePageWithCRUD(string userRole, User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            InitializeDatabase(); // Inicijalizacija baze podataka
            LoadItems(); // Učitavanje artikala
            SetUserRole(userRole); // Postavljanje korisničke uloge
        }

        // Metoda koja postavlja korisničku ulogu i redirektuje ako korisnik nije admin
        private void SetUserRole(string userRole)
        {
            if (userRole != "Admin")
            {
                Navigation.PushAsync(new HomePage(userRole, _currentUser)); // Navigacija na početnu stranicu za druge uloge
            }
        }

        // Metoda koja se poziva kada stranica postane vidljiva
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems(); // Ponovno učitavanje artikala
        }

        // Metoda koja inicijalizuje vezu sa bazom podataka i kreira potrebne tabele
        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Item>(); // Kreiranje tabele za artikle
            _database.CreateTable<SpecialOffer>();
        }

        // Metoda koja dodaje početne podatke u bazu ako artikli ne postoje
        private void SeedData()
        {
            AddItemIfNotExists("Margarita", "Italijanski stil", 8.00M, "pizza.jpg", "Pizza");
            AddItemIfNotExists("Grilled Chicken", "Grilovana piletina sa začinima", 10.00M, "chicken.jpg", "Piletina");
            AddItemIfNotExists("Cheese Burger", "Klasični cheeseburger sa zelenu salatom i paradajzom", 6.50M, "burger.jpg", "Burger");
            AddItemIfNotExists("Spaghetti Bolognese", "Tradicionalni spaghetti sa mesnim sosom", 12.00M, "pasta.jpg", "Pasta");
            AddItemIfNotExists("California Roll", "Sushi rolnice sa rakovima i avokadom", 15.00M, "sushi.jpg", "Sushi");
            AddItemIfNotExists("Pancakes", "Pahuljaste palačinke sa sirupom", 7.00M, "dorucak.jpg", "Doručak");
            AddItemIfNotExists("Cake", "Bogati čokoladni kolač sa kremom", 5.00M, "desert.jpg", "Desert");
        }

        // Metoda koja dodaje artikal u bazu ako već ne postoji
        private void AddItemIfNotExists(string name, string description, decimal price, string imageUrl, string category)
        {
            var existingItem = _database.Table<Item>().FirstOrDefault(i => i.Name == name);
            if (existingItem == null)
            {
                _database.Insert(new Item
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    ImageUrl = imageUrl,
                    Category = category
                });
            }
        }

        // Metoda koja se poziva kada korisnik klikne na dugme za brisanje artikla
        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itemToDelete = button?.CommandParameter as Item;

            if (itemToDelete != null)
            {
                bool confirm = await DisplayAlert("Potvrda brisanja", $"Da li ste sigurni da želite obrisati {itemToDelete.Name}?", "Da", "Ne");
                if (confirm)
                {
                    _database.Delete(itemToDelete); // Brisanje artikla iz baze
                    LoadItems(); // Učitavanje artikala
                    await DisplayAlert("Uspješno", "Artikal je uspješno obrisan.", "OK");
                }
            }
        }

        // Metoda koja učitava artikle iz baze
        private void LoadItems()
        {
            var items = _database.Table<Item>().ToList(); // Dohvatanje svih artikala
            var offers = _database.Table<SpecialOffer>().ToList();
            ItemsCollectionView.ItemsSource = items; // Postavljanje izvora podataka za CollectionView
        }

        // Metoda koja čisti unos u formama
        private void ClearFormFields()
        {
            ItemNameEntry.Text = string.Empty;
            ItemDescriptionEntry.Text = string.Empty;
            ItemPriceEntry.Text = string.Empty;
            ItemImageUrlEntry.Text = string.Empty;
            ItemCategoryEntry.Text = string.Empty;
        }

        // Metoda koja se poziva kada korisnik klikne na dugme za dodavanje ili ažuriranje artikla
        private async void AddOrUpdateItemButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string itemName = ItemNameEntry.Text;
                string itemDescription = ItemDescriptionEntry.Text;
                string itemPriceText = ItemPriceEntry.Text;
                string itemImageUrl = ItemImageUrlEntry.Text;
                string itemCategory = ItemCategoryEntry.Text;

                // Provjera da li su svi podaci uneseni
                if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(itemDescription) ||
                    string.IsNullOrWhiteSpace(itemPriceText) || string.IsNullOrWhiteSpace(itemImageUrl) ||
                    string.IsNullOrWhiteSpace(itemCategory))
                {
                    await DisplayAlert("Greška", "Molimo unesite sve informacije", "OK");
                    return;
                }

                // Provjera da li je cijena validna
                if (!decimal.TryParse(itemPriceText, out decimal itemPrice) || itemPrice <= 0)
                {
                    await DisplayAlert("Greška", "Molimo unesite validnu cijenu veću od 0", "OK");
                    return;
                }

                // Dodavanje novog ili ažuriranje postojećeg artikla
                if (_itemBeingEdited == null)
                {
                    var newItem = new Item
                    {
                        Name = itemName,
                        Description = itemDescription,
                        Price = itemPrice,
                        ImageUrl = itemImageUrl,
                        Category = itemCategory
                    };

                    _database.Insert(newItem); // Dodavanje novog artikla u bazu
                }
                else
                {
                    _itemBeingEdited.Name = itemName;
                    _itemBeingEdited.Description = itemDescription;
                    _itemBeingEdited.Price = itemPrice;
                    _itemBeingEdited.ImageUrl = itemImageUrl;
                    _itemBeingEdited.Category = itemCategory;

                    _database.Update(_itemBeingEdited); // Ažuriranje postojećeg artikla
                    _itemBeingEdited = null;
                }

                await DisplayAlert("Uspješno", "Artikal je uspješno sačuvan", "OK");
                LoadItems(); // Ponovno učitavanje artikala
                ClearFormFields(); // Čišćenje formi
            }
            catch (Exception ex)
            {
                await DisplayAlert("Greška", $"Došlo je do greške: {ex.Message}", "OK"); // Obavještenje u slučaju greške
            }
        }

        // Metoda koja se poziva kada korisnik klikne na dugme za ažuriranje artikla
        private void UpdateItemButton_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itemToUpdate = button?.CommandParameter as Item;

            if (itemToUpdate == null)
                return;

            // Popunjavanje polja sa selektovanim artiklom
            ItemNameEntry.Text = itemToUpdate.Name;
            ItemDescriptionEntry.Text = itemToUpdate.Description;
            ItemPriceEntry.Text = itemToUpdate.Price.ToString();
            ItemImageUrlEntry.Text = itemToUpdate.ImageUrl;
            ItemCategoryEntry.Text = itemToUpdate.Category;

            _itemBeingEdited = itemToUpdate; // Postavljanje artikla koji se uređuje
        }

        // Metoda za odjavu korisnika
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
        }

        // Metoda koja se poziva kada korisnik selektuje artikal iz liste
        private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection.FirstOrDefault() as Item;
            if (selectedItem != null)
            {
                await Navigation.PushAsync(new ItemDetailPage(selectedItem, _currentUser)); // Navigacija na stranicu za detalje artikla
            }
        }

        // Metoda koja se poziva kada korisnik klikne na dugme za povratak na početnu stranicu
        private async void OnGoToHomePageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePage("Admin", _currentUser)); // Navigacija na početnu stranicu
        }
    }
}