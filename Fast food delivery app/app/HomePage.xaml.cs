using SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using app.Models;
using app.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using app.Helpers;


namespace app
{
    public partial class HomePage : TabbedPage, INotifyPropertyChanged
    {
       



        private SQLiteConnection _database; // Veza sa bazom podataka
        private readonly User _currentUser; // Trenutni korisnik
        public bool IsAdmin { get; set; } // Da li je korisnik admin
        public bool IsLoggedUser { get; set; } // Da li je korisnik prijavljen
        public bool IsGuest { get; set; } // Da li je korisnik gost

        // Konstruktor stranice koji inicijalizuje stranicu sa ulogom korisnika i trenutnim korisnikom
        public HomePage(string userRole, User currentUser)
        {
            InitializeComponent();
            InitializeDatabase(); // Inicijalizacija baze podataka
            SetUserRole(userRole); // Postavljanje uloge korisnika
            LoadCategories(); // Učitavanje kategorija
            LoadSpecialOffers(); // Učitavanje specijalnih ponuda
            LoadItems(); // Učitavanje artikala

            _currentUser = currentUser;

            // Inicijalizacija ViewModel-a i postavljanje BindingContext-a
            var viewModel = new HomePageViewModel(userRole);
            BindingContext = viewModel;

            // Logovanje inicijalizacije
            Console.WriteLine($"HomePage inicijalizovana sa userRole: {userRole}, IsAdmin: {viewModel.IsAdmin}, IsLoggedUser: {viewModel.IsLoggedUser}");
        }

        // Metoda koja inicijalizuje vezu sa bazom podataka
        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Item>(); // Kreira tabelu za artikle
            _database.CreateTable<CartItem>(); // Kreira tabelu za artikle u korpi
            _database.CreateTable<Category>(); // Kreira tabelu za kategorije
            _database.CreateTable<SpecialOffer>(); // Kreira tabelu za specijalne ponude
            _database.CreateTable<Order>(); // Kreira tabelu za porudžbine
            _database.CreateTable<OrderItem>(); // Kreira tabelu za stavke porudžbine
        }

        // Metoda za postavljanje uloge korisnika
        private void SetUserRole(string userRole)
        {
            IsAdmin = userRole == "Admin"; // Ako je korisnik admin
            IsLoggedUser = userRole == "LoggedUser"; // Ako je korisnik prijavljen
            IsGuest = userRole == "Guest"; // Ako je korisnik gost
            OnPropertyChanged(nameof(IsAdmin)); // Obavještava o promjeni uloge
            OnPropertyChanged(nameof(IsLoggedUser));
            OnPropertyChanged(nameof(IsGuest));
        }

        // Metoda za učitavanje kategorija iz baze podataka
        private void LoadCategories()
        {
            var categories = _database.Table<Category>().ToList(); // Učitavanje svih kategorija iz baze
            CategoriesCollectionView.ItemsSource = categories; // Postavljanje kolekcije kao izvor podataka
        }

        // Metoda za učitavanje specijalnih ponuda iz baze podataka
        private void LoadSpecialOffers()
        {
            var specialOffers = DatabaseHelper.GetSpecialOffers();
            if (specialOffers != null && specialOffers.Count > 0)
            {
                SpecialOffersCollectionView.ItemsSource = specialOffers; // Povezivanje sa CollectionView
            }
            else
            {
                DisplayAlert("Nema specijalnih ponuda!", "Trenutno nema dostupnih specijalnih ponuda.", "OK");
            }
        }

        // Metoda za učitavanje artikala iz baze podataka
        private void LoadItems()
        {
            var items = _database.Table<Item>().ToList(); // Učitavanje svih artikala
            ItemsCollectionView.ItemsSource = items; // Postavljanje izvora podataka za CollectionView
        }

        // Metoda koja se poziva kada korisnik pritisne dugme za pretragu
        private void OnSearchBarButtonPressed(object sender, EventArgs e)
        {
            var searchText = SearchBar.Text?.ToLower(); // Dohvatanje unesenog teksta u pretragu

            if (string.IsNullOrWhiteSpace(searchText)) // Ako je tekst prazan, učitaj sve artikle
            {
                LoadItems();
            }
            else
            {
                // Filtriranje artikala na osnovu unesenog teksta
                var filteredItems = _database.Table<Item>().Where(i => i.Name.ToLower().Contains(searchText)).ToList();
                ItemsCollectionView.ItemsSource = filteredItems; // Postavljanje filtriranih artikala
            }
        }

        // Metoda koja se poziva kada korisnik klikne na meni
        private async void OnMenuClicked(object sender, EventArgs e)
        {
            // Prikazivanje akcija u meniju
            var action = await DisplayActionSheet("Meni", "Otkaži", null, "Historija narudžbi", "Odjava");

            // Obrada odabrane opcije iz menija
            switch (action)
            {
                case "Historija narudžbi":
                    await Navigation.PushAsync(new OrderHistoryPage()); // Navigacija na stranicu historije narudžbe
                    break;
                case "Odjava":
                    await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
                    break;
            }
        }

        // Metoda koja se poziva kada korisnik selektuje kategoriju
        private async void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            // Dohvatanje selektovane kategorije
            var selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
            if (selectedCategory != null)
            {
                // Navigacija na stranicu sa detaljima kategorije
                await Navigation.PushAsync(new CategoryPage(selectedCategory, _currentUser));
            }
            // Resetovanje selekcije da bi korisnik mogao ponovo da bira istu kategoriju
            CategoriesCollectionView.SelectedItem = null;
        }

        // Metoda koja se poziva kada korisnik selektuje specijalnu ponudu
        private async void OnSpecialOfferSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedOffer = e.CurrentSelection.FirstOrDefault() as SpecialOffer;
            if (selectedOffer != null)
            {
                Console.WriteLine($"Izabrana specijalna ponuda: {selectedOffer.Name}");
                await Navigation.PushAsync(new SpecialOfferPage(selectedOffer)); // Navigacija na stranicu specijalne ponude
            }
            // Resetovanje selekcije da bi korisnik mogao ponovo da bira istu ponudu
            CategoriesCollectionView.SelectedItem = null;
        }

        // Metoda koja se poziva kada korisnik selektuje proizvod
        private async void OnItemDetailClicked(object sender, EventArgs e)
        {
            var button = sender as Button; // Preuzimanje dugmeta koje je kliknuto
            var item = button?.CommandParameter as Item; // Preuzimanje artikla iz CommandParameter

            if (item != null)
            {
                Console.WriteLine($"Odabrani proizvod: {item.Name}"); // Test log
                await Navigation.PushAsync(new ItemDetailPage(item, _currentUser)); // Navigacija na stranicu sa detaljima
            }
            // Resetovanje selekcije da bi korisnik mogao ponovo da bira isti proizvod
            CategoriesCollectionView.SelectedItem = null;
        }

        // Metoda koja se poziva kada korisnik klikne na dugme za brisanje artikla
        private async void OnDeleteItemClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var itemToDelete = button?.CommandParameter as Item; // Dohvatanje artikla za brisanje
            if (itemToDelete != null)
            {
                // Potvrda brisanja artikla
                bool confirm = await DisplayAlert("Potvrda brisanja", $"Jeste li sigurni da želite obrisati {itemToDelete.Name}?", "Da", "Ne");
                if (confirm)
                {
                    _database.Delete(itemToDelete); // Brisanje artikla iz baze
                    LoadItems(); // Ponovno učitavanje artikala
                    await DisplayAlert("Uspješno", "Artikal je uspješno obrisan.", "OK"); // Obavještenje korisniku
                }
            }
        }

        /// Metoda za brisanje kategorije
        private async void OnDeleteCategoryClicked(object sender, EventArgs e)
        {
            var button = sender as Button; // Preuzimanje dugmeta koje je pokrenulo događaj
            var categoryToDelete = button?.CommandParameter as Category; // Dobijanje kategorije koja se treba obrisati

            if (categoryToDelete != null)
            {
                // Potvrda korisnika za brisanje kategorije
                bool confirm = await DisplayAlert("Potvrda brisanja", $"Jeste li sigurni da želite obrisati kategoriju {categoryToDelete.Name}?", "Da", "Ne");
                if (confirm)
                {
                    _database.Delete(categoryToDelete); // Brisanje kategorije iz baze
                    LoadCategories(); // Učitavanje preostalih kategorija
                    await DisplayAlert("Uspješno", "Kategorija je uspješno obrisana.", "OK");
                }
            }
        }

        // Metoda za brisanje specijalne ponude
        private async void OnDeleteSpecialOfferClicked(object sender, EventArgs e)
        {
            var button = sender as Button; // Preuzimanje dugmeta koje je pokrenulo događaj
            var specialOfferToDelete = button?.CommandParameter as SpecialOffer; // Dobijanje specijalne ponude koja se treba obrisati

            if (specialOfferToDelete != null)
            {
                // Potvrda korisnika za brisanje specijalne ponude
                bool confirm = await DisplayAlert("Potvrda brisanja", $"Jeste li sigurni da želite obrisati specijalnu ponudu {specialOfferToDelete.Name}?", "Da", "Ne");
                if (confirm)
                {
                    _database.Delete(specialOfferToDelete); // Brisanje specijalne ponude iz baze
                    LoadSpecialOffers(); // Učitavanje preostalih specijalnih ponuda
                    await DisplayAlert("Uspješno", "Specijalna ponuda je uspješno obrisana.", "OK");
                }
            }
        }

        // Metoda za navigaciju do stranice korpe
        private async void OnViewCartClicked(object sender, EventArgs e)
        {
            if (IsGuest)
            {
                bool goToLogin = await DisplayAlert("Nema pristup", "Nemate pristup korpi. Želite li se prijaviti?", "Da", "Ne");
                if (goToLogin)
                {
                    await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
                }
            }
            else
            {
                await Navigation.PushAsync(new CartPage()); // Navigacija do stranice za prikaz korpe
            }
        }



        // Metoda za navigaciju do stranice sa kategorijama
        private async void OnGoToCategoriesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AllCategoriesPage(_currentUser)); // Navigacija na stranicu Kategorije
        }


        // Metoda za dodavanje proizvoda u korpu
        private void OnAddToCartTapped(object sender, EventArgs e)
        {
            var tappedItem = (sender as Image)?.BindingContext as Item;
            if (tappedItem != null)
            {
                // Dodavanje proizvoda u korpu (implementirajte svoju logiku)
                DisplayAlert("Dodano u korpu", $"{tappedItem.Name} je dodan u vašu korpu.", "OK");
            }
        }

        // Metoda za prikaz detalja o proizvodu
        private async void OnItemDetailTapped(object sender, EventArgs e)
        {
            var tappedItem = (sender as Image)?.BindingContext as Item;
            if (tappedItem != null)
            {
                // Navigacija na stranicu sa detaljima o proizvodu
                await Navigation.PushAsync(new ItemDetailPage(tappedItem, _currentUser)); // Prosleđivanje tappedItem i currentUser
            }
        }


        // Metoda za dodavanje proizvoda u korpu
        private async void OnAddToCartClicked(object sender, EventArgs e)
        {
            if (IsGuest)
            {
                bool goToLogin = await DisplayAlert("Nema pristup", "Nemate pristup dodavanju u korpu. Želite li se prijaviti?", "Da", "Ne");
                if (goToLogin)
                {
                    await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
                }
            }
            else
            {
                var button = sender as Button; // Preuzimanje dugmeta koje je pokrenulo događaj
                var itemToAdd = button?.CommandParameter as Item; // Dobijanje proizvoda koji se treba dodati u korpu

                if (itemToAdd != null)
                {
                    // Provjera da li proizvod već postoji u korpi
                    var existingCartItem = _database.Table<CartItem>().FirstOrDefault(c => c.ItemId == itemToAdd.Id);
                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += 1; // Ako postoji, povećava količinu
                        _database.Update(existingCartItem);
                    }
                    else
                    {
                        // Ako ne postoji, dodaje novi proizvod u korpu
                        var cartItem = new CartItem
                        {
                            ItemId = itemToAdd.Id,
                            Name = itemToAdd.Name,
                            Price = itemToAdd.Price,
                            Quantity = 1,
                            ImageUrl = itemToAdd.ImageUrl
                        };
                        _database.Insert(cartItem);
                    }

                    await DisplayAlert("Uspješno", $"{itemToAdd.Name} je dodan u korpu.", "OK");
                }
            }
        }

        // Metoda za dodavanje specijalnog proizvoda u korpu
        private async void OnAddToCartClickedSpecialOffer(object sender, EventArgs e)
        {
            if (IsGuest)
            {
                bool goToLogin = await DisplayAlert("Nema pristup", "Nemate pristup dodavanju u korpu. Želite li se prijaviti?", "Da", "Ne");
                if (goToLogin)
                {
                    await Navigation.PushAsync(new LoginPage());
                }
                return;
            }

            var button = sender as Button;
            var specialItemToAdd = button?.CommandParameter as SpecialOffer;

            if (specialItemToAdd != null)
            {
                var existingCartItem = _database.Table<CartItem>().FirstOrDefault(c => c.SpecialOfferId == specialItemToAdd.Id);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += 1;
                    _database.Update(existingCartItem);
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        SpecialOfferId = specialItemToAdd.Id,
                        Name = specialItemToAdd.Name,
                        Price = specialItemToAdd.OriginalPrice,
                        DiscountedPrice = specialItemToAdd.DiscountedPrice,
                        Quantity = 1,
                        ImageUrl = specialItemToAdd.ImageUrl
                    };
                    _database.Insert(cartItem);
                }

                await DisplayAlert("Uspješno", $"{specialItemToAdd.Name} je dodan u korpu.", "OK");
            }
        }

        // Metoda za navigaciju do stranice historije narudžbi
        private async void OnOrderHistoryClicked(object sender, EventArgs e)
        {
            if (IsGuest)
            {
                bool goToLogin = await DisplayAlert("Nema pristup", "Nemate pristup historiji narudžbi. Želite li se prijaviti?", "Da", "Ne");
                if (goToLogin)
                {
                    await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
                }
            }
            else
            {
                await Navigation.PushAsync(new OrderHistoryPage()); // Navigacija do stranice historije narudžbi
            }
        }

        // Metoda za navigaciju do administratorske kontrolne ploče
        private async void OnGoToAdminDashboardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HomePageWithCRUD("Admin", _currentUser)); // Navigacija do administratorske kontrolne ploče
        }

        // Metoda za navigaciju do stranice korisničkog profila
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (IsGuest)
            {
                bool goToLogin = await DisplayAlert("Nema pristup", "Nemate pristup profilu. Želite li se prijaviti?", "Da", "Ne");
                if (goToLogin)
                {
                    await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
                }
            }
            else
            {
                await Navigation.PushAsync(new ProfilePage(_currentUser)); // Navigacija do stranice profila korisnika
            }
        }

        // Metoda koja se poziva kada se tab korpe prikaže
        private void OnCartTabAppearing(object sender, EventArgs e)
        {
            RefreshCartItems(); // Osvježavanje stavki u korpi
        }

        // Metoda za osvježavanje stavki u korpi
        private void RefreshCartItems()
        {
            var cartItems = _database.Table<CartItem>().ToList(); // Dobijanje svih stavki iz korpe
        }

        // Event za obavještavanje o promjenama svojstava
        public new event PropertyChangedEventHandler PropertyChanged;

        // Metoda za podizanje događaja kada se svojstvo promijeni
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}