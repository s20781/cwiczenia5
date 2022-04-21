using cwiczenia5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia5.Services
{
    public interface IWarehouseService
    {

        public Task<bool> DoesProductExist(int productId);
        public Task<bool> DoesWarehouseExist(int productId);
        public Task<int> GetTheValidOrderId(int warehouseId, int productId, int amount, DateTime createdAt);
        public Task<int> CompeleteTheOrder(int orderId, int warehouseId, int productId, int amount, DateTime createdAt);
        public Task<double> GetTheProductPrice(int productId);
        public Task<int> StoredProcedure(int warehouseId, int productId, int amount, DateTime createdAt);
        public Task<IEnumerable<WarehousePost>> GetWarehouse();
    }
}
