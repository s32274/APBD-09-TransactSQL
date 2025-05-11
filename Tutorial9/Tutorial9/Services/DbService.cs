using Microsoft.Data.SqlClient;
using Tutorial9.Exceptions;

namespace Tutorial9.Services;

public class DbService : IDbService
{
    private readonly String _connectionString;
    
    public DbService()
    {
        this._connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;";
    }
    
    public async Task<int> AddProductToWarehouseAsync
    (
        int idProduct, int idWarehouse, int amount, DateTime createdAt, CancellationToken cancellationToken
    )
    {
        if (amount < 1)
            throw new Exception("Wartość Amount (= " + amount + ") powinna być większa od 0.");
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            // 1) "Sprawdzamy, czy produkt o podanym identyfikatorze istnieje."
            command.Parameters.Clear();
            command.CommandText = "SELECT 1 FROM Product WHERE IdProduct = @IdProduct";
            command.Parameters.AddWithValue("@IdProduct", idProduct);

            var productIdRes = await command.ExecuteScalarAsync();
            if (productIdRes is null)
                throw new NotFoundException("Product with ID - " + idProduct + " - not found.");

            // "Następnie sprawdzamy, czy magazyn o podanym identyfikatorze istnieje."
            command.Parameters.Clear();
            command.CommandText = "SELECT 1 FROM Warehouse WHERE IdProduct = @IdWarehouse";
            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);

            var warehouseIdRes = await command.ExecuteScalarAsync();
            if (warehouseIdRes is null)
                throw new NotFoundException("Warehouse with ID - " + idWarehouse + " - not found.");
            
            // 2. "Możemy dodać produkt do magazynu tylko wtedy, gdy istnieje
            // zamówienie zakupu produktu w tabeli Order. Dlatego sprawdzamy, czy w
            // tabeli Order istnieje rekord z IdProduktu i Ilością (Amount), które
            // odpowiadają naszemu żądaniu."\
            // Data utworzenia zamówienia powinna
            // być wcześniejsza niż data utworzenia w żądaniu."
            command.Parameters.Clear();
            command.CommandText = @"SELECT 1 FROM Order 
                                    WHERE IdProduct = @IdProduct 
                                    AND Amount = @Amount
                                    AND CreatedAt < @CreatedAt";
            
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@CreatedAt", createdAt);

            var orderData = await command.ExecuteScalarAsync();
            if (orderData is null)
                throw new NotFoundException(
                    "Order: IdProduct = " + idProduct + "; Amount = " + amount +
                    " doesn't exist or has insufficient amount of product"
                );
            
            // 3. "Sprawdzamy, czy to zamówienie zostało przypadkiem zrealizowane.
            // Sprawdzamy, czy nie ma wiersza z danym IdOrder w tabeli
            // Product_Warehouse."
            command.Parameters.Clear();
            command.CommandText = "SELECT IdOrder FROM Order WHERE IdProduct = @IdProduct AND Amount = @Amount";
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@Amount", amount);

            var idOrder = await command.ExecuteScalarAsync();
            
            command.Parameters.Clear();
            command.CommandText = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";

            command.Parameters.AddWithValue("@IdOrder", idOrder);

            var orderExists = await command.ExecuteScalarAsync();
            if (!(orderExists is null))
            {
                throw new Exception("Order with ID " + idOrder + " already exists.");
            }
            
            // 4. "Aktualizujemy kolumnę FullfilledAt zamówienia na aktualną datę i
            // godzinę. (UPDATE)"
            command.Parameters.Clear();
            command.CommandText = "UPDATE Order SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
            command.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
            command.Parameters.AddWithValue("@IdOrder", idOrder);

            await command.ExecuteNonQueryAsync();
            
            // 5. "Wstawiamy rekord do tabeli Product_Warehouse. Kolumna Price
            // powinna odpowiadać cenie produktu pomnożonej przez kolumnę Amount
            // z naszego zamówienia. Ponadto wstawiamy wartość CreatedAt zgodnie
            // z aktualnym czasem. (INSERT)"
            command.Parameters.Clear();
            command.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            var productPrice = (int) await command.ExecuteScalarAsync();
            
            command.Parameters.Clear();
            command.CommandText = @"INSERT INTO 
                                    Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)
                                    VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";

            command.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
            command.Parameters.AddWithValue("@IdProduct", idProduct);
            command.Parameters.AddWithValue("@IdOrder", idOrder);
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Price", amount * productPrice);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            await command.ExecuteNonQueryAsync();
            
            // 6. "W wyniku operacji zwracamy wartość klucza głównego wygenerowanego
            // dla rekordu wstawionego do tabeli Product_Warehouse."
            command.Parameters.Clear();
            command.CommandText = "SELECT SCOPE_IDENTITY()";
            
            var idProductWarehouse = await command.ExecuteScalarAsync();
            
            await transaction.CommitAsync();

            return (int)idProductWarehouse;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
       
    }
}