using SQLite;
using app.Models;

namespace app;

public partial class AllCategoriesPage : ContentPage
{
    private SQLiteConnection _database; // Veza sa bazom podataka
    private List<Category> _categories; // Originalna lista kategorija
    private readonly User _currentUser; // Trenutni korisnik

    // Konstruktor koji prima korisnika
    public AllCategoriesPage(User currentUser)
    {
        _currentUser = currentUser; // Postavljanje trenutnog korisnika
        InitializeComponent();
        InitializeDatabase(); // Inicijalizacija baze podataka
        LoadCategories(); // Učitavanje kategorija
    }

    private void InitializeDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3"); // Putanja do baze
        _database = new SQLiteConnection(dbPath); // Kreiranje konekcije
        _database.CreateTable<Category>(); // Kreiranje tabele (ako ne postoji)
    }

    private void LoadCategories()
    {
        // Učitavanje svih kategorija iz baze
        _categories = _database.Table<Category>().ToList();
        // Postavljanje izvora podataka za CollectionView
        CategoriesCollectionView.ItemsSource = _categories;
    }

    // Metoda za filtriranje kategorija
    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? string.Empty;
        // Filtriranje kategorija na osnovu unosa
        CategoriesCollectionView.ItemsSource = _categories
            .Where(c => c.Name.ToLower().Contains(searchText) ||
                        c.Description.ToLower().Contains(searchText))
            .ToList();
    }

    // Navigacija na stranicu kategorije
    private async void OnCategorySelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
        if (selectedCategory != null)
        {
            await Navigation.PushAsync(new CategoryPage(selectedCategory, _currentUser)); // Navigacija
        }
        CategoriesCollectionView.SelectedItem = null; // Resetovanje selekcije
    }
}