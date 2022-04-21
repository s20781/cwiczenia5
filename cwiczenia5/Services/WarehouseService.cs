using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using cwiczenia5.Models;
using cwiczenia5.Services;
using Microsoft.Extensions.Configuration;

namespace cw5.Services
{
    public class WarehouseService : IWarehouseService
    {

        private readonly IConfiguration _configuration;

        public WarehouseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }






        public async Task<int> CompeleteTheOrder(int orderId, int warehouseId, int productId, int amount, DateTime createdAt)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                await connection.OpenAsync();
                var tran =  await  connection.BeginTransactionAsync();
                command.Transaction = (SqlTransaction) tran;


                try
                {

                    command.Parameters.Clear();
                    command.CommandText = $"Update Orders SET FulfilledAt=@1 where IdOrder= {orderId}";
                    command.Parameters.AddWithValue("@1", DateTime.Now);

                    await command.ExecuteNonQueryAsync();


                    double price = await GetTheProductPrice(productId);
                   

                    command.Parameters.Clear();
                    command.CommandText = "Insert into Product_Warehouse (IdWarehouse, IdProduct, IdOrder,Amount,Price,CreatedAt) values (@1,@2,@3,@4,@5,@6)";
                    command.Parameters.AddWithValue("@1", warehouseId);
                    command.Parameters.AddWithValue("@2",productId);
                    command.Parameters.AddWithValue("@3", orderId);
                    command.Parameters.AddWithValue("@4", amount);
                    command.Parameters.AddWithValue("@5", price*amount);
                    command.Parameters.AddWithValue("@6", DateTime.Now);

                    await command.ExecuteNonQueryAsync();


                    await tran.CommitAsync();
                }catch (SqlException ex)
                {
                    tran.Rollback();
                }catch (Exception ex)
                {
                    tran.Rollback();
                }
            }
            return 0;
        }



               

                


        public async Task<bool> DoesProductExist(int productId)
        {

            
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
                using ( var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT COUNT(*) from Product where IdProduct = {productId}";
                await connection.OpenAsync();
               

                int exist = (int) await command.ExecuteScalarAsync();

                if(exist == 0)
                {
                    throw new Exception("Nie istnieje Produkt o danym Id");
                    return false;
                }

            }
            return true;

        }

        public async Task<bool> DoesWarehouseExist(int warehouseId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT COUNT(*) from Warehouse where IdWarehouse = {warehouseId}";
                await connection.OpenAsync();


                int exist = (int)await command.ExecuteScalarAsync();

                if (exist == 0)
                {
                    throw new Exception("Nie istnieje warehouse o podanym Id");
                    return false;
                }

                
            }
            return true;

        }

        public async Task<double> GetTheProductPrice(int productId)
        {
            double price = 0;
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
                using(var command = new SqlCommand())
            {
                command.Connection= connection;
                command.CommandText = $"SELECT Price from Product where IdProduct = {productId}";
                await connection.OpenAsync();

                

                SqlDataReader dataReader = await command.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    price = double.Parse(dataReader["Price"].ToString());
                }
                dataReader.Close();

            }
            return price;
        }

        public async Task<int> GetTheValidOrderId(int warehouseId, int productId, int amount, DateTime createdAt)
        {
            int orderId = 0;
            
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT COUNT(*) from Orders where IdProduct = {productId}";
                await connection.OpenAsync();

                int exist = (int)await command.ExecuteScalarAsync();

                if (exist == 0)
                {
                    throw new Exception();
                }
            }

            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT amount from Orders where IdProduct = {productId}";
                await connection.OpenAsync();
                int orderAmount = 0;

                SqlDataReader dataReader = await command.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    orderAmount = int.Parse(dataReader["Amount"].ToString());
                }

                if (orderAmount != amount)
                {
                    throw new Exception("amount sie nie zgadza");
                }
                dataReader.Close();
            }
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection= connection;
                command.CommandText = $"SELECT CreatedAt from Orders where IdProduct = {productId}";
                await connection.OpenAsync();
                DateTime orderCreatedAt = DateTime.MinValue;

                SqlDataReader dataReader2 = await command.ExecuteReaderAsync();
                while (await dataReader2.ReadAsync())
                {
                    orderCreatedAt = DateTime.Parse(dataReader2["CreatedAt"].ToString());
                }
                dataReader2.Close();

                if (DateTime.Compare(orderCreatedAt, createdAt)>0)
                {
                    throw new Exception("Data sie nie zgadza");
                }
            }
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"SELECT IdOrder from Orders where IdProduct = {productId}";
                await connection.OpenAsync();


                SqlDataReader dataReader3 = await command.ExecuteReaderAsync();
                
                while (await dataReader3.ReadAsync())
                {
                    orderId = int.Parse(dataReader3["IdOrder"].ToString());
                }
                dataReader3.Close();

            }
            

         return orderId;

            
            
        }

                        

                    

        public async Task<int> StoredProcedure(int warehouseId, int productId, int amount, DateTime createdAt)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<WarehousePost>> GetWarehouse()
        {
            var warehouseProducts = new List<WarehousePost>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = $"Select * from product_warehouse";
                await connection.OpenAsync();

                SqlDataReader dataReader = await command.ExecuteReaderAsync();
                while (await dataReader.ReadAsync())
                {
                    var WarehousePost = new WarehousePost
                    {
                        
                        IdWarehouse = int.Parse(dataReader["IdWarehouse"].ToString()),
                        IdProduct = int.Parse(dataReader["IdProduct"].ToString()),
                        
                        Amount = int.Parse(dataReader["Amount"].ToString()),
                       
                        CreatedAt = DateTime.Parse(dataReader["CreatedAt"].ToString())

                    };
                    warehouseProducts.Add(WarehousePost);
                }
            }



            return (warehouseProducts);
        }
    }
}
