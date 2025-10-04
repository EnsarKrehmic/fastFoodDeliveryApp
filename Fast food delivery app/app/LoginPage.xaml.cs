using SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using app.Models;

namespace app;

public partial class LoginPage : ContentPage
{
    private SQLiteConnection _database;

    public LoginPage()
    {
        InitializeComponent();
        InitializeDatabase();
    }

    // Inicijalizacija baze podataka i kreiranje osnovnih korisnika ako ne postoje
    private void InitializeDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
        _database = new SQLiteConnection(dbPath);
        _database.CreateTable<User>();

        // Kreiraj admin korisnika ako ne postoji
        var adminUser = _database.Table<User>().FirstOrDefault(u => u.Username == "admin@fastfood.com");
        if (adminUser == null)
        {
            _database.Insert(new User
            {
                Username = "admin@fastfood.com",
                Password = "admin123",
                Role = "Admin"
            });
        }

        // Kreiraj korisnika ako ne postoji
        var user1 = _database.Table<User>().FirstOrDefault(u => u.Username == "ensar@fastfood.com");
        if (user1 == null)
        {
            _database.Insert(new User
            {
                Username = "ensar@fastfood.com",
                Password = "ensar123",
                Role = "LoggedUser"
            });
        }

        // Kreiraj korisnika ako ne postoji
        var user2 = _database.Table<User>().FirstOrDefault(u => u.Username == "tare@fastfood.com");
        if (user2 == null)
        {
            _database.Insert(new User
            {
                Username = "tare@fastfood.com",
                Password = "tare123",
                Role = "LoggedUser"
            });
        }
    }

    // Funkcija za prijavu korisnika
    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Pokrećemo indikator učitavanja
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            // Validacija unesenih podataka
            if (!ValidateInputs(username, password))
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                return;
            }

            // Provjera da li je baza podataka inicijalizovana
            if (_database == null)
            {
                await DisplayAlert("Greška", "Povezivanje s bazom podataka nije inicijalizovano.", "OK");
                return;
            }

            // Pronalazimo korisnika u bazi podataka
            var user = _database.Table<User>().FirstOrDefault(u => u.Username == username);

            // Provjera da li korisnik postoji u bazi
            if (user == null)
            {
                await DisplayAlert("Greška", "Korisnik nije pronađen.", "OK");
                return;
            }

            // Provjera tačnosti korisničkog imena i lozinke
            if (user.Password == password) // Direktno upoređivanje lozinke
            {
                await DisplayAlert("Uspjeh", "Prijava uspješna! Dobrodošli.", "OK");
                if (user.Role == "Admin")
                {
                    await Navigation.PushAsync(new HomePageWithCRUD(user.Role, user));
                }
                else
                {
                    await Navigation.PushAsync(new HomePage(user.Role, user));
                }
            }
            else
            {
                await DisplayAlert("Greška", "Pogrešno korisničko ime ili lozinka.", "OK");
            }
        }
        catch (Exception ex)
        {
            // U slučaju greške, prikazujemo poruku
            await DisplayAlert("Greška", $"Došlo je do greške: {ex.Message}", "OK");
        }
        finally
        {
            // Na kraju, sakrivamo indikator učitavanja
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    // Funkcija za nastavak kao gost
    private async void OnContinueAsGuestClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HomePage("Guest", null));
    }

    // Validacija unesenih podataka
    private bool ValidateInputs(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Korisničko ime je obavezno!");
            return false;
        }

        if (!IsValidEmail(username))
        {
            ShowError("Molimo unesite važeću email adresu.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Lozinka je obavezna!");
            return false;
        }

        return true;
    }

    // Provjera validnosti email adrese
    private bool IsValidEmail(string email)
    {
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    // Funkcija za prikazivanje greške
    private void ShowError(string message)
    {
        DisplayAlert("Greška", message, "OK");
    }

    // Funkcija za navigaciju na stranicu za registraciju
    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}