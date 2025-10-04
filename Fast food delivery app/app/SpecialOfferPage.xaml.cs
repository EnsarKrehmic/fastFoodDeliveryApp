using SQLite;
using app.Models;

namespace app;

public partial class SpecialOfferPage : ContentPage
{
    private SQLiteConnection _database;
    private SpecialOffer _specialOffer;

    // Konstruktor koji prima specijalnu ponudu i postavlja BindingContext
    public SpecialOfferPage(SpecialOffer specialOffer)
    {
        InitializeComponent();
        _specialOffer = specialOffer;
        BindingContext = _specialOffer; // Postavljanje BindingContext na specijalnu ponudu
        InitializeDatabase(); // Inicijalizacija baze podataka
    }

    // Funkcija za inicijalizaciju baze podataka
    private void InitializeDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
        _database = new SQLiteConnection(dbPath);
        _database.CreateTable<CartItem>(); // Kreiranje tabele za stavke u korpi
    }

    // Funkcija koja se poziva kada korisnik klikne na dugme "Dodaj u korpu"
    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        // Provjera da li je već stavka u korpi
        var existingCartItem = _database.Table<CartItem>().FirstOrDefault(c => c.Name == _specialOffer.Name);
        if (existingCartItem != null)
        {
            // Ako stavka već postoji u korpi, povećaj količinu za 1
            existingCartItem.Quantity += 1;
            _database.Update(existingCartItem); // Ažuriraj postojeću stavku u bazi
        }
        else
        {
            // Ako stavka nije u korpi, dodaj novu
            var cartItem = new CartItem
            {
                Name = _specialOffer.Name,
                Price = _specialOffer.DiscountedPrice,
                Quantity = 1
            };
            _database.Insert(cartItem); // Dodaj novu stavku u bazu
        }

        // Prikazivanje obavještenja korisniku da je proizvod uspješno dodan u korpu
        await DisplayAlert("Uspješno!", "Proizvod je dodan u korpu.", "OK");
    }
}