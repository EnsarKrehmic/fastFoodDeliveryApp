using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace app.ViewModels
{
    // ViewModel za početnu stranicu koja upravlja vidljivošću elemenata na osnovu uloge korisnika
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private bool _isAdmin; // Promjenjiva koja označava da li je korisnik administrator
        private bool _isLoggedUser; // Promjenjiva koja označava da li je korisnik prijavljen

        // Svojstvo za vezivanje s atributom IsVisible u XAML-u, vidljivost za administratore
        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                if (_isAdmin != value)
                {
                    _isAdmin = value; // Ažuriranje vrijednosti
                    OnPropertyChanged(); // Obavještavanje o promjeni svojstva
                }
            }
        }

        // Svojstvo za vezivanje s atributom IsVisible u XAML-u, vidljivost za prijavljene korisnike
        public bool IsLoggedUser
        {
            get => _isLoggedUser;
            set
            {
                if (_isLoggedUser != value)
                {
                    _isLoggedUser = value; // Ažuriranje vrijednosti
                    OnPropertyChanged(); // Obavještavanje o promjeni svojstva
                }
            }
        }

        // Konstruktor koji prima ulogu korisnika i postavlja odgovarajuće vrijednosti
        public HomePageViewModel(string userRole)
        {
            SetUserRole(userRole); // Postavljanje uloge korisnika
        }

        // Postavljanje uloge korisnika i određivanje da li je korisnik administrator ili prijavljeni korisnik
        private void SetUserRole(string userRole)
        {
            // Provjera uloge korisnika i postavljanje vrijednosti za IsAdmin i IsLoggedUser
            IsAdmin = userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
            IsLoggedUser = userRole.Equals("LoggedUser", StringComparison.OrdinalIgnoreCase);
        }

        // Implementacija INotifyPropertyChanged za obavještavanje o promjenama svojstava
        public event PropertyChangedEventHandler PropertyChanged;

        // Metoda koja obavještava o promjeni svojstva
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}