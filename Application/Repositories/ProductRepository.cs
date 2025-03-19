using Application.Interfaces;
using Dapper;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ProductRepository> _logger;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddAsync(Product entity)
        {
            entity.AddedOn = DateTime.Now;
            var sql = "INSERT INTO Products (Name, Description, Barcode, Rate, AddedOn) VALUES (@Name, @Description, @Barcode, @Rate, @AddedOn)";
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with Id {Id}", id);
                throw;
            }
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            var sql = "SELECT * FROM Products";
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.QueryAsync<Product>(sql);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Products WHERE Id = @Id";
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with Id {Id}", id);
                throw;
            }
        }

        public async Task<int> UpdateAsync(Product entity)
        {
            entity.ModifiedOn = DateTime.Now;
            var sql = "UPDATE Products SET Name = @Name, Description = @Description, Barcode = @Barcode, Rate = @Rate, ModifiedOn = @ModifiedOn WHERE Id = @Id";
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with Id {Id}", entity.Id);
                throw;
            }
        }
    }
}