using Microsoft.Maui.Controls;
using SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using app.Models;

namespace app
{
    public partial class RegisterPage : ContentPage
    {
        private SQLiteConnection _database;

        // Konstruktor koji inicijalizuje bazu podataka
        public RegisterPage()
        {
            InitializeComponent();
            InitializeDatabase(); // Inicijalizacija baze podataka
        }

        // Funkcija za inicijalizaciju baze podataka
        private void InitializeDatabase()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<User>(); // Kreiranje tabele za korisnike
        }

        // Funkcija koja se poziva kada korisnik klikne na dugme za registraciju
        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            string username = UsernameEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();
            string confirmPassword = ConfirmPasswordEntry.Text?.Trim();

            // Provjera da li su uneseni ispravni podaci
            if (!ValidateInputs(username, password, confirmPassword))
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                return;
            }

            // Provjera da li korisnik već postoji u bazi
            var existingUser = _database.Table<User>().FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {
                ShowError("Korisničko ime već postoji."); // Prikazivanje greške ako korisnik već postoji
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                return;
            }

            // Kreiranje novog korisnika i unos u bazu
            var newUser = new User
            {
                Username = username,
                Password = password // Direktno čuvanje lozinke bez heširanja (preporučuje se implementacija heširanja)
            };
            _database.Insert(newUser);

            await DisplayAlert("Uspješno", "Registracija je uspješna!", "OK"); // Obavještenje o uspješnoj registraciji
            ClearFields(); // Čišćenje unesenih podataka

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;

            // Navigacija na stranicu za prijavu nakon uspješne registracije
            await Navigation.PushAsync(new LoginPage());
        }

        // Funkcija za validaciju unesenih podataka
        private bool ValidateInputs(string username, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Korisničko ime je obavezno!"); // Greška ako korisničko ime nije uneseno
                return false;
            }

            if (!IsValidEmail(username))
            {
                ShowError("Unesite važeću email adresu."); // Greška ako email nije validan
                return false;
            }

            if (username.Length < 3)
            {
                ShowError("Korisničko ime mora imati minimalno 3 karaktera!"); // Greška ako korisničko ime nije dovoljno dugo
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Lozinka je obavezna!"); // Greška ako lozinka nije unesena
                return false;
            }

            if (password.Length < 6)
            {
                ShowError("Lozinka mora imati minimalno 6 karaktera!"); // Greška ako lozinka nije dovoljno duga
                return false;
            }

            if (password != confirmPassword)
            {
                ShowError("Lozinke se ne poklapaju."); // Greška ako lozinke ne odgovaraju
                return false;
            }

            return true;
        }

        // Funkcija koja provjerava da li je email validan
        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Regularni izraz za validaciju email adrese
            return Regex.IsMatch(email, emailPattern);
        }

        // Funkcija za prikazivanje greške u Label-u
        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true; // Prikazivanje greške
        }

        // Funkcija za čišćenje unesenih podataka
        private void ClearFields()
        {
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;
            ErrorLabel.IsVisible = false; // Sakrivanje greške
        }

        // Funkcija koja se poziva kada korisnik klikne na tekst Login (prijava)
        private async void OnLoginTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new LoginPage()); // Navigacija na stranicu za prijavu
        }
    }
}