using Microsoft.AspNetCore.Mvc;
using Tutorial9.Exceptions;
using Tutorial9.Services;

namespace Tutorial9.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
        this._dbService = dbService;
    }

    [HttpPut]
    public async Task AddProductToWarehouseAsync
    (
        int idProduct, int idWarehouse, int amount, CancellationToken cancellationToken
    )
    {
        try
        {
            await _dbService.AddProductToWarehouseAsync(
                idProduct, idWarehouse, amount, cancellationToken
            );


        }
        catch (NotFoundException e)
        {
            
        }
    }
}