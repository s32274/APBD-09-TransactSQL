using System.ComponentModel.DataAnnotations;

namespace Tutorial9.Model;

public class ProductDto
{
    public int IdProduct;
    public int IdWarehouse;
    public int amount;
    public DateTime CreatedAt;
}