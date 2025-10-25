using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Basket.Data.JsonConverters;

public class ShoppingCartConverter :JsonConverter<ShoppingCart>
{
    public override ShoppingCart? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;

        // Try to get id (support both camelCase and PascalCase)
        Guid id = Guid.NewGuid();
        if (rootElement.TryGetProperty("id", out var idElement) || rootElement.TryGetProperty("Id", out idElement))
        {
            id = idElement.GetGuid();
        }

        // Try to get userName (support both camelCase and PascalCase)
        string userName = string.Empty;
        if (rootElement.TryGetProperty("userName", out var userNameElement) || rootElement.TryGetProperty("UserName", out userNameElement))
        {
            userName = userNameElement.GetString() ?? string.Empty;
        }

        if (string.IsNullOrEmpty(userName))
        {
            throw new JsonException("UserName property is required for ShoppingCart deserialization.");
        }

        var shoppingCart = ShoppingCart.Create(id, userName);

        // Try to get items (support both camelCase and PascalCase)
        if (rootElement.TryGetProperty("items", out var itemsElement) || rootElement.TryGetProperty("Items", out itemsElement))
        {
            var items = itemsElement.Deserialize<List<ShoppingCartItem>>(options);
            
            if (items != null)
            {
                var itemsField = typeof(ShoppingCart).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                itemsField?.SetValue(shoppingCart, items);
            }
        }

        return shoppingCart;
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCart value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("id", value.Id.ToString());
        writer.WriteString("userName", value.UserName);

        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, options);

        writer.WriteEndObject();
    }
}