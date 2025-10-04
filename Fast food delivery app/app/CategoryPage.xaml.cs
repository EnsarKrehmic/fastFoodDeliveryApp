using SQLite;
using app.Models;

namespace app;

public partial class CategoryPage : ContentPage
{
    private SQLiteConnection _database; // Veza sa bazom podataka
    private Category _category; // Kategorija čiji će se artikli prikazivati
    private readonly User _currentUser; // Trenutni korisnik koji je prijavljen

    // Konstruktor koji inicijalizuje stranicu sa odabranom kategorijom i trenutnim korisnikom
    public CategoryPage(Category category, User currentUser)
    {
        _category = category; // Postavljanje kategorije
        _currentUser = currentUser; // Postavljanje trenutnog korisnika
        BindingContext = _category; // Postavljanje BindingContext-a za kategoriju
        InitializeComponent();
        InitializeDatabase(); // Inicijalizacija baze podataka
        LoadCategories();
        LoadCategoryItems(); // Učitavanje artikala u kategoriji
    }

    // Metoda koja inicijalizuje vezu sa bazom podataka
    private void InitializeDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3"); // Putanja do baze podataka
        _database = new SQLiteConnection(dbPath); // Otvaranje veze sa bazom
        _database.CreateTable<Item>(); // Kreiranje tabele za artikle ako već ne postoji
    }

    private void LoadCategories()
    {
        var categories = _database.Table<Category>().ToList(); // Učitavanje svih kategorija
        CategoryItemsCollectionView.ItemsSource = categories; // Postavljanje izvora podataka za CollectionView
    }

    // Metoda koja učitava artikle iz odabrane kategorije
    private void LoadCategoryItems()
    {
        // Učitavanje svih artikala koji pripadaju odabranoj kategoriji
        var items = _database.Table<Item>().Where(i => i.Category == _category.Name).ToList();
        // Postavljanje liste artikala kao izvor podataka za CollectionView
        CategoryItemsCollectionView.ItemsSource = items;
    }

    // Metoda koja se poziva kada korisnik selektuje neki artikal
    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        // Dohvatanje selektovanog artikla
        var selectedItem = e.CurrentSelection.FirstOrDefault() as Item;
        if (selectedItem != null)
        {
            // Navigacija ka stranici za detalje o artiklu, prosljeđivanje odabranog artikla i trenutnog korisnika
            await Navigation.PushAsync(new ItemDetailPage(selectedItem, _currentUser));
        }
    }
}