using SQLite;
using app.Helpers;
using app.Models;
using System.Collections.Generic;

namespace app
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent(); // Inicijalizacija komponenti stranice
            LoadUsers(); // Učitavanje korisnika prilikom pokretanja stranice
        }

        // Funkcija za učitavanje svih korisnika iz baze podataka
        private void LoadUsers()
        {
            // Učitaj sve korisnike iz baze
            List<User> users = DatabaseHelper.Database.Table<User>().ToList();

            // Provjera da li lista nije prazna
            if (users.Count > 0)
            {
                UsersListView.ItemsSource = users; // Postavljanje korisnika na listu za prikaz
            }
            else
            {
                // Ako nema korisnika, prikazujemo poruku
                DisplayAlert("Informacija", "Nema korisnika u bazi.", "OK");
            }
        }
    }
}