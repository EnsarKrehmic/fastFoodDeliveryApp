namespace app;

using app.Models;
using app.ViewModels;

public partial class ProfilePage : ContentPage
{
    private readonly User _currentUser; // Trenutni korisnik čiji je profil prikazan

    // Konstruktor koji inicijalizuje stranicu sa podacima o korisniku
    public ProfilePage(User currentUser)
    {
        InitializeComponent();
        LoadUserProfile(); // Učitavanje podataka o korisniku
        _currentUser = currentUser;
        BindingContext = new ProfileViewModel(_currentUser); // Postavljanje ViewModel-a za profil
    }

    // Metoda za učitavanje podataka o korisniku i prikaz na ekranu
    private void LoadUserProfile()
    {
        if (_currentUser != null)
        {
            // Prikazivanje korisničkog imena u odgovarajućim labelama
            EmailLabel.Text = _currentUser.Username;
        }
        else
        {
            // Ako korisnik nije prijavljen, prikazuju se defaultne vrijednosti
            NameLabel.Text = "Nepoznat korisnik";
            EmailLabel.Text = "Bez e-maila";
        }
    }

    // Metoda koja se poziva kada korisnik klikne na dugme za odjavu
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        // Navigacija ka stranici za prijavu
        await Navigation.PushAsync(new LoginPage());
    }
}