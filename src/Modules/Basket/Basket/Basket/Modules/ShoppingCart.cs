

namespace Basket.Basket.Modules;

public class ShoppingCart : Aggregate<Guid>
{
    private readonly List<ShoppingCartItem> _items = [];
    
    public string UserName { get; private set; } = default!;

    public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
    
    public decimal TotalPrice => _items.Sum(item => item.Price * item.Quantity);

    public static ShoppingCart Create(Guid id, string userName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        
        var shoppingCart = new ShoppingCart
        {
            Id = id,
            UserName = userName
        };
        
        return shoppingCart;
    }
    
    public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);
        
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var newItem = new ShoppingCartItem(Id, productId, quantity, color, price, productName);
            
            _items.Add(newItem);
        }
    }
    
    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            _items.Remove(item);
        }
    }
}