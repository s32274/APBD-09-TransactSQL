namespace Tutorial9.Services;

public interface IDbService
{
    public Task<int> AddProductToWarehouseAsync
    (
        int idProduct, int idWarehouse, int amount, CancellationToken cancellationToken
    );

}