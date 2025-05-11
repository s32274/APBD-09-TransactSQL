namespace Tutorial9.Services;

public interface IDbService
{
    public Task<int> AddProductToWarehouseAsync
    (
        int IdProduct, int idWarehouse, int amount, DateTime createdAt, CancellationToken cancellationToken
    );

}