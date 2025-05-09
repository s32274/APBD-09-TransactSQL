using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Tutorial9.Services;

public class ProductService : IProductService
{
    private readonly IConfiguration _configuration;
    public ProductService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task DoSomethingAsync()
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.CommandText = @"INSERT INTO ";
            command.Parameters.AddWithValue();
            command.Parameters.AddWithValue();

            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
            command.CommandText = @"INSERT INTO )";
            command.Parameters.AddWithValue();
            command.Parameters.AddWithValue();

            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
       
    }
}