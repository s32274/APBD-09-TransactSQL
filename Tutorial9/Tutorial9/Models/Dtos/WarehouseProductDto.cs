using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class WarehouseProductDto
{
    [Required]
    public int IdProduct { get; set; }
    
    [Required]
    public int IdWarehouse { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount of added product must exceed 0")]
    public int Amount { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}