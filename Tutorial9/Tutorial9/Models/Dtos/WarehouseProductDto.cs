using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class WarehouseProductDto
{
    [Required]
    public int IdProduct;
    
    [Required]
    public int IdWarehouse;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount of added product must exceed 0")]
    public int Amount;
    
    [Required]
    public DateTime CreatedAt;
}