using Microsoft.AspNetCore.Mvc;
using Tutorial9.Exceptions;
using Tutorial9.Model;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPut]
    public async Task<IActionResult> AddProductToWarehouseAsync(
        [FromBody] WarehouseProductDto dto, 
        CancellationToken cancellationToken)
    {
        try
        {
            var idProductWarehouse = await _dbService.AddProductToWarehouseAsync(
                    dto.IdProduct, dto.IdWarehouse, dto.Amount, dto.CreatedAt, cancellationToken
            );

            return Ok(idProductWarehouse);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}