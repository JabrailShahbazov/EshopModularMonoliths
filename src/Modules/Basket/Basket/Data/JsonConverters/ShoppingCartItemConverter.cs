using System.Text.Json;
using System.Text.Json.Serialization;

namespace Basket.Data.JsonConverters;

public class ShoppingCartItemConverter : JsonConverter<ShoppingCartItem>
{
    public override ShoppingCartItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;

        // Try to get id
        Guid id = Guid.NewGuid();
        if (rootElement.TryGetProperty("id", out var idElement) || rootElement.TryGetProperty("Id", out idElement))
        {
            id = idElement.GetGuid();
        }

        // Try to get shoppingCartId
        Guid shoppingCartId = Guid.Empty;
        if (rootElement.TryGetProperty("shoppingCartId", out var cartIdElement) || rootElement.TryGetProperty("ShoppingCartId", out cartIdElement))
        {
            shoppingCartId = cartIdElement.GetGuid();
        }

        // Try to get productId
        Guid productId = Guid.Empty;
        if (rootElement.TryGetProperty("productId", out var productIdElement) || rootElement.TryGetProperty("ProductId", out productIdElement))
        {
            productId = productIdElement.GetGuid();
        }

        // Try to get quantity
        int quantity = 0;
        if (rootElement.TryGetProperty("quantity", out var quantityElement) || rootElement.TryGetProperty("Quantity", out quantityElement))
        {
            quantity = quantityElement.GetInt32();
        }

        // Try to get color
        string color = string.Empty;
        if (rootElement.TryGetProperty("color", out var colorElement) || rootElement.TryGetProperty("Color", out colorElement))
        {
            color = colorElement.GetString() ?? string.Empty;
        }

        // Try to get price
        decimal price = 0;
        if (rootElement.TryGetProperty("price", out var priceElement) || rootElement.TryGetProperty("Price", out priceElement))
        {
            price = priceElement.GetDecimal();
        }

        // Try to get productName
        string productName = string.Empty;
        if (rootElement.TryGetProperty("productName", out var productNameElement) || rootElement.TryGetProperty("ProductName", out productNameElement))
        {
            productName = productNameElement.GetString() ?? string.Empty;
        }

        return new ShoppingCartItem(id, shoppingCartId, productId, quantity, color, price, productName);
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCartItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id.ToString());
        writer.WriteString("shoppingCartId", value.ShoppingCartId.ToString());
        writer.WriteString("productId", value.ProductId.ToString());
        writer.WriteNumber("quantity", value.Quantity);
        writer.WriteString("color", value.Color);
        writer.WriteNumber("price", value.Price);
        writer.WriteString("productName", value.ProductName);

        writer.WriteEndObject();
    }
}