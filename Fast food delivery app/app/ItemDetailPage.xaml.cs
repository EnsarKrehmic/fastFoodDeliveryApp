using SQLite;
using app.Models;

namespace app;

public partial class ItemDetailPage : ContentPage
{
    private SQLiteConnection _database;
    private readonly Item _item;
    private readonly User _currentUser;

    public ItemDetailPage(Item item, User currentUser)
    {
        InitializeComponent();
        _item = item;
        _currentUser = currentUser;
        BindingContext = item;

        InitializeDatabase();
        LoadReviews();

        // Prikazivanje sekcije za recenzije samo ako je korisnik prijavljen
        ReviewSection.IsVisible = _currentUser != null;
    }

    private void InitializeDatabase()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
        _database = new SQLiteConnection(dbPath);
        _database.CreateTable<CartItem>();
        _database.CreateTable<Review>();
    }

    private void LoadReviews()
    {
        var reviews = _database.Table<Review>().Where(r => r.ItemId == _item.Id).ToList();
        ReviewsCollectionView.ItemsSource = reviews;
    }

    private async void OnAddToCartTapped(object sender, EventArgs e)
    {
        var existingCartItem = _database.Table<CartItem>().FirstOrDefault(c => c.Name == _item.Name);

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += 1;
            _database.Update(existingCartItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                Name = _item.Name,
                Price = _item.Price,
                Quantity = 1
            };
            _database.Insert(cartItem);
        }

        await DisplayAlert("Uspješno", "Artikal je dodan u korpu.", "OK");
    }

    private async void OnSubmitReviewClicked(object sender, EventArgs e)
    {
        if (int.TryParse(ReviewRatingEntry.Text, out int rating) && rating >= 1 && rating <= 5)
        {
            var review = new Review
            {
                ItemId = _item.Id,
                Username = _currentUser.Username,
                Comment = ReviewCommentEntry.Text,
                Rating = rating
            };
            _database.Insert(review);
            LoadReviews();

            await DisplayAlert("Uspješno", "Recenzija je poslana.", "OK");
        }
        else
        {
            await DisplayAlert("Greška", "Molimo unesite validnu ocjenu između 1 i 5.", "OK");
        }
    }
}