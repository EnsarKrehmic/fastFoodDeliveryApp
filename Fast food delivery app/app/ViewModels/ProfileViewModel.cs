using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SQLite;
using app.Models;
using Microsoft.Maui.Controls;

namespace app.ViewModels
{
    // ViewModel za profil korisnika, implementira INotifyPropertyChanged za obavještavanje o promjenama svojstava
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private SQLiteConnection _database; // SQLite konekcija za rad s bazom podataka
        private User _currentUser; // Trenutni korisnik čiji se podaci uređuju

        // Svojstva za korisničko ime i lozinku
        public string Username { get; set; }
        public string Password { get; set; }

        // Komanda za spremanje promjena
        public ICommand SaveCommand { get; }

        // Konstruktor za inicijalizaciju trenutnog korisnika i komande
        public ProfileViewModel(User currentUser)
        {
            _currentUser = currentUser;
            Username = _currentUser.Username; // Postavljanje korisničkog imena
            Password = _currentUser.Password; // Postavljanje lozinke

            // Kreiranje komande koja poziva metodu za spremanje
            SaveCommand = new Command(OnSave);

            InitializeDatabase(); // Inicijalizacija baze podataka
        }

        // Metoda za inicijalizaciju baze podataka
        private void InitializeDatabase()
        {
            // Definisanje puta za bazu podataka
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");

            // Konekcija s bazom i kreiranje tabele korisnika ako ne postoji
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<User>();
        }

        // Metoda koja se poziva kada korisnik želi spremiti promjene
        private async void OnSave()
        {
            // Provjera da li je lozinka prazna
            if (string.IsNullOrWhiteSpace(Password))
            {
                // Prikaz poruke o grešci ako lozinka nije unesena
                await Application.Current.MainPage.DisplayAlert("Greška", "Lozinka ne može biti prazna.", "U redu");
                return;
            }

            // Ažuriranje lozinke korisnika
            _currentUser.Password = Password;
            _database.Update(_currentUser); // Spremanje promjena u bazu

            // Prikaz poruke o uspjehu
            await Application.Current.MainPage.DisplayAlert("Uspjeh", "Profil je uspješno ažuriran.", "U redu");
        }

        // Implementacija INotifyPropertyChanged za obavještavanje o promjenama svojstava
        public event PropertyChangedEventHandler PropertyChanged;

        // Metoda za obavještavanje o promjeni svojstva
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}