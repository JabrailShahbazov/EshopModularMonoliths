namespace Catalog.Products.Dtos;

public record ProductDto(Guid Id,
                        string Name,
                        string Description,
                        decimal Price,
                        string ImageFile, 
                        List<string> Category);