using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class ProductDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "IdProduct musi być większe od 0")]
    public int IdProduct;
    
    [Required]
    public int IdWarehouse;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Liczba dodanych produktów musi być większa od 0")]
    public int amount;
    
    [Required]
    public DateTime CreatedAt;
}